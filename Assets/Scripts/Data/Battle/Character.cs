using UnityEngine;
using TurnBased.Data;
using System;
using TurnBased.Battle.Managers;
using Unity.Cinemachine;

namespace TurnBased.Battle {
    public abstract class Character : MonoBehaviour {
        public GameObject meshParent;
        public CinemachineCamera ultIdleOverride;

        [Header("Character Data")]
        [SerializeField]
        protected CharacterData _baseData;
        [SerializeField]
        protected Transform _chest;

        public enum CharacterState {
            Idle,
            PrepareAttack,
            DoAttack,
            DoExtraAttack,
            PrepareSkill,
            CastSkill,
            PrepareUltAttack,
            PrepareUltSkill,
            CastUltAttack,
            CastUltSkill,
            PrepareDead,
            Dead,
            PrepareGroggy,
            Groggy
        }

        public enum MeshLayer {
            Default,
            SkillTimeine,
            Hidden,
            UltTimeline
        }

        public Action<Character> OnTurnStart;
        public Action<Character> OnTurnEnd;
        public Action<Character> OnUltTurn;
        public Action<Character> OnExtraAttackTurn;
        public Action<Character> OnTransitionTurn;
        public Action<Character, string, string> OnAnimationEvent;
        public Action<Character, bool> OnVisibilityChange;
        public Action<Character, Character, DamageResult> OnInflictedDamage;
        public Action<Character, Character, DamageResult> OnDamage;
        public Action<Character, Character, float> OnRestoreHealth;
        public Action<Character> OnDeath;
        public Action<Character> OnDeathComplete;

        public CharacterDataInstance Data { get; private set; }

        public CharacterState CurrentState { get; protected set; }
        public bool WantCmd { get; set; }
        public CharacterState WantState { get; protected set; } = CharacterState.PrepareAttack;
        public bool IsVisible { get; set; }
        public MeshLayer CurrentMeshLayer { get; protected set; }
        public bool IsDead {
            get {
                return CurrentState == CharacterState.Dead;
            }
        }
        public AudioSource VOAudioSource { get; private set; }
        public Transform Chest {
            get {
                return _chest != null ? _chest : transform;
            }
        }


        protected virtual void Awake() {
            Data = new CharacterDataInstance(_baseData);
            VOAudioSource = GetComponent<AudioSource>();
            if (VOAudioSource == null) {
                VOAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        protected virtual void Start() {
            SetVisible(true);
        }

        /// <summary>
        /// 턴 시작 시 실행
        /// </summary>
        public virtual void TakeTurn() {
            if (IsDead) {
                TurnManager.instance.EndTurn();
                OnTurnEnd?.Invoke(this);
                return;
            }

            WantCmd = true;
            if (Data.Team == CharacterTeam.Player)
            {
                if (WantState == CharacterState.PrepareAttack)
                {
                    PrepareAttack();
                }
                else if (WantState == CharacterState.PrepareSkill)
                {
                    PrepareSkill();
                }
            }
            // 만약 에너미일때
            else 
            {
                if (CurrentState == CharacterState.Groggy) 
                {
                    GroggyReset();
                }
                // 다음 본인 턴이왔을 때 취할 상태가 공격 준비라면
                if (WantState == CharacterState.PrepareAttack)
                {
                    // 공격 준비 함수를 호출
                    PrepareAttack();
                }
                // 다음 본인 턴이 왔을 때 취할 상태가 스킬 준비라면
                else if (WantState == CharacterState.PrepareSkill)
                {
                    // 스킬 준비 함수를 호출
                    PrepareSkill();
                }

            }
            OnTurnStart?.Invoke(this);
        }

        public virtual void TakeUltTurn() {
            WantCmd = true;
            PrepareUltAttack();
            OnUltTurn?.Invoke(this);
        }

        public virtual void TakeExtraAttackTurn() {
            OnExtraAttackTurn?.Invoke(this);
        }

        public virtual void TakeTransitionTurn() {
            OnTransitionTurn?.Invoke(this);
        }

        /// <summary>
        /// 턴 종료 시 실행
        /// </summary>
        public virtual void EndTurn() {
            CurrentState = CharacterState.Idle;
            TurnManager.instance.EndTurn();
            OnTurnEnd?.Invoke(this);
        }

        /// <summary>
        /// 애니메이션 이벤트 처리. 이벤트는 이벤트명.추가정보로 구분 (예: SkillCast.1)
        /// </summary>
        public void ProcessAnimationEvent(string animEvent) {
            string[] args = animEvent.Split(".");
            string argument = args[0];
            string payload = "";
            if (args.Length >= 2) {
                payload = args[1];
            }
            if (argument == "SoundPlay") {
                SoundManager.instance.Play2DSound(payload);
            }
            else if (argument == "VOSoundPlay") {
                SoundManager.instance.PlayVOSound(this, payload);
            }
            else if (argument == "DeathComplete") {
                OnDeathComplete?.Invoke(this);
            }
            Debug.Log("사운드호출");
            OnAnimationEvent?.Invoke(this, argument, payload);
        }

        /// <summary>
        /// 일반 공격 준비자세
        /// </summary>
        public virtual void PrepareAttack() {
            CurrentState = CharacterState.PrepareAttack;
        }
        /// <summary>
        /// 일반 공격 발동
        /// </summary>
        public virtual void DoAttack() {
            CurrentState = CharacterState.DoAttack;
            WantState = CharacterState.PrepareAttack;
            WantCmd = false;
        }
        /// <summary>
        /// 추가 공격 발동
        /// </summary>
        public virtual void DoExtraAttack(Character target) {
            CurrentState = CharacterState.DoExtraAttack;
        }
        /// <summary>
        /// 스킬 공격 준비자세
        /// </summary>
        public virtual void PrepareSkill() {
            CurrentState = CharacterState.PrepareSkill;
        }
        /// <summary>
        /// 스킬 공격 발동
        /// </summary>
        public virtual void CastSkill() {
            CurrentState = CharacterState.CastSkill;
            WantState = CharacterState.PrepareSkill;
            WantCmd = false;
        }
        /// <summary>
        /// 궁극기 Q 준비자세
        /// </summary>
        public virtual void PrepareUltAttack() {
            CurrentState = CharacterState.PrepareUltAttack;
        }
        /// <summary>
        /// 궁극기 Q 발동
        /// </summary>
        public virtual void CastUltAttack() {
            CurrentState = CharacterState.CastUltAttack;
            WantCmd = false;
        }
        /// <summary>
        /// 궁극기 E 준비자세
        /// </summary>
        public virtual void PrepareUltSkill() {
            CurrentState = CharacterState.PrepareUltSkill;
        }
        /// <summary>
        /// 궁극기 E 발동
        /// </summary>
        public virtual void CastUltSkill() {
            CurrentState = CharacterState.CastUltSkill;
            WantCmd = false;
        }
        /// <summary>
        /// 캐릭터 모델 활성화/비활성화
        /// </summary>
        /// <param name="visibility">보일지 안보일지</param>
        public virtual void SetVisible(bool visibility) {
            if (visibility && CurrentMeshLayer == MeshLayer.Hidden) {
                SetMeshLayer(MeshLayer.Default);
                OnVisibilityChange?.Invoke(this, visibility);
            }
            else if (!visibility && CurrentMeshLayer != MeshLayer.Hidden) {
                SetMeshLayer(MeshLayer.Hidden);
                OnVisibilityChange?.Invoke(this, visibility);
            }
            IsVisible = visibility;
        }
        /// <summary>
        /// 사망 준비함수
        /// </summary>
        public virtual void PrepareDead() {
            // 캐릭터의 현재 상태를 사망준비상태로 처리
            CurrentState = CharacterState.PrepareDead;
        }
        /// <summary>
        /// 캐릭터 사망
        /// </summary>
        public virtual void Dead() {
            // 캐릭터의 현재 상태를 사망으로 처리
            CurrentState = CharacterState.Dead;
            // 취할 상태를 사망준비로
            WantState = CharacterState.PrepareDead;
            // 명령대기를 하지 않음을 반환
            WantCmd = false;
            OnDeath?.Invoke(this);
        }
        /// <summary>
        /// 그로기 준비함수
        /// </summary>
        public virtual void PrepareGroggy()
        {
            // 현재 상태를 그로기 준비상태로 처리
            CurrentState = CharacterState.PrepareGroggy;
        }

        /// <summary>
        /// 그로기 상태 진입
        /// </summary>
        public virtual void Groggy() {
            // 캐릭터의 현재 상태를 그로기로 처리
            CurrentState = CharacterState.Groggy;
            // 취할 상태를 그로기 준비상태로
            WantState = CharacterState.PrepareGroggy;
            // 명령대기를 하지 않음을 반환
            WantCmd = false;

            GroggyDebuff();
        }

        /// <summary>
        /// 메쉬 레이어 변경
        /// </summary>
        /// <param name="layer"></param>
        public virtual void SetMeshLayer(MeshLayer layer) {
            int layerID = 0;
            CurrentMeshLayer = layer;
            if (layer == MeshLayer.SkillTimeine) {
                layerID = 6;
            }
            else if (layer == MeshLayer.Hidden) {
                layerID = 7;
            }
            else if (layer == MeshLayer.UltTimeline) {
                layerID = 8;
            }
            foreach (var child in meshParent.GetComponentsInChildren<Transform>(true)) {
                child.gameObject.layer = layerID;
            }
        }

        /// <summary>
        /// 데미지 함수 (계산 후 결과를 이용해서 공격)
        /// </summary>
        /// <param name="attacker"></param>
        public virtual void Damage(Character attacker, DamageResult result) {
            if (IsDead) {
                return;
            }

            if (Data.Toughness.CurrentMax > 0) {
                // 만약 강인도가 있다면
                if (Data.Toughness.Current > 0) {
                    // 플레이어가 약점 속성으로 때린다면
                    if (CombatManager.CheckElementMatch(attacker.Data.ElementType, Data.Weakness)) {
                        // 에너미의 강인도는 플레이어의 공격력만큼 깎인다
                        Data.Toughness.ModifyCurrent(-result.ToughnessDamage);

                        // 강인도가 만약 0이하가 되었다면
                        if (Data.Toughness.Current <= 0) {
                            // 그로기를 다룰 함수를 실행한다
                            Groggy();

                            switch (attacker.Data.ElementType) {
                                case ElementType.Fire:
                                    GetComponent<CharacterBuffSystem>().ApplyBuff("FireDOT", attacker);
                                    break;
                                case ElementType.Quantum:
                                    GetComponent<CharacterBuffSystem>().ApplyBuff("QuantumDOT", attacker);
                                    break;
                            }
                        }
                    }
                }
                // 만약 강인도가 없다면
                else {
                    // 그로기 상태이니 그로기 상태를 계속한다
                    CurrentState = CharacterState.Groggy;
                }
            }
            // 캐릭터는 최종적으로 받는 데미지를 모두 받는다.
            Data.HP.ModifyCurrent(-result.FinalDamage);
            OnDamage?.Invoke(this, attacker, result);

            // 채력이 만약 0이하가 되었다면
            if (Data.HP.Current <= 0) {
                // 죽음을 다룰 함수를 실행한다
                Dead();
            }
        }

        public virtual void RestoreHealth(Character healer, float value) {
            Data.HP.ModifyCurrent(value);
            OnRestoreHealth?.Invoke(this, healer, value);
        }

        public virtual void ProcessCamChanged() { }

        public virtual void ProcessCamGain() { }

        // 그로기시 속도와 방어력을 절반으로 하는 디버프 함수
        public virtual void GroggyDebuff()
        {
            GetComponent<CharacterBuffSystem>().ApplyBuff("GroggyDebuff", this);

            // 속도 감소후 반영
            TurnManager.instance.ProcessAVSpeedChange();
        }

        // 그로기 시 줄어든 속도와 방어력을 원래대로 돌리는 함수
        public virtual void GroggyReset()
        {
            GetComponent<CharacterBuffSystem>().RemoveBuff("GroggyDebuff");

            // 현재 강인도를 최대로 한다
            Data.Toughness.Reset();

            // 속도 변경후 반영
            TurnManager.instance.ProcessAVSpeedChange();
        }

    }
}
