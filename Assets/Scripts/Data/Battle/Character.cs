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
            Groggy,
            Damage
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
        public Action<Character, string, string> OnAnimationEvent;
        public Action<Character, bool> OnVisibilityChange;
        public Action<Character, Character, DamageResult> OnDamage;
        public Action<Character, Character, float> OnRestoreHealth;

        public CharacterData Data { get; private set; }

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


        protected virtual void Awake() {
            Data = Instantiate(_baseData);
            Data.stats.CurrentToughness = Data.stats.MaxToughness;
            Data.stats.CurrentHP = Data.stats.MaxHP;
        }

        protected virtual void Start() {
            SetVisible(true);
        }

        /// <summary>
        /// 턴 시작 시 실행
        /// </summary>
        public virtual void TakeTurn() {
            WantCmd = true;
            if (Data.team == CharacterTeam.Player)
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
        public virtual void DoExtraAttack() {
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
            // 턴 큐에서 캐릭터 제거
            TurnManager.instance.RemoveCharacter(this);

            Debug.Log("Dead 함수 실행");
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
            // 캐릭터의 현재 상태를 데미지로 처리
            CurrentState = CharacterState.Damage;

            if (Data.stats.MaxToughness > 0) {
                // 만약 강인도가 있다면
                if (Data.stats.CurrentToughness > 0) {

                    // 채력을 최종적으로 받는 데미지의 반으로 받고
                    Data.stats.CurrentHP -= (result.FinalDamage / 2);

                    // 플레이어가 약점 속성으로 때린다면
                    if (CombatManager.CheckElementMatch(attacker.Data.stats.ElementType, Data.stats.Weakness)) {
                        // 에너미의 강인도는 플레이어의 공격력만큼 깎인다
                        Data.stats.CurrentToughness -= result.NormalAttack;

                        // 강인도가 만약 0이하가 되었다면
                        if (Data.stats.CurrentToughness <= 0) {
                            // 현재 강인도를 0으로 만든다
                            Data.stats.CurrentToughness = 0;
                            // 그로기를 다룰 함수를 실행한다
                            Groggy();
                        }
                    }

                    // 채력이 만약 0이하가 되었다면
                    if (Data.stats.CurrentHP <= 0) {
                        // 죽음을 다룰 함수를 실행한다
                        Dead();
                    }
                }
                // 만약 강인도가 없다면
                else {
                    // 에너미는 최종적으로 받는 데미지를 모두 받는다.
                    Data.stats.CurrentHP -= result.FinalDamage;

                    // 채력이 만약 0이하가 되었다면
                    if (Data.stats.CurrentHP <= 0) {
                        // 죽음을 다룰 함수를 실행한다
                        Dead();
                    }
                }
            }
            OnDamage?.Invoke(this, attacker, result);
        }

        public virtual void RestoreHealth(Character healer, float value) {
            Data.stats.CurrentHP = Mathf.Min(Data.stats.CurrentHP + value, Data.stats.MaxHP);
            OnRestoreHealth?.Invoke(this, healer, value);
        }

        public virtual void ProcessCamChanged() { }

        public virtual void ProcessCamGain() { }
    }
}
