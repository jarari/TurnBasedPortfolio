using UnityEngine;
using TurnBased.Data;
using System;
using TurnBased.Battle.Managers;

namespace TurnBased.Battle {
    public abstract class Character : MonoBehaviour {
        public GameObject meshParent;

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
            Hidden
        }

        public Action<Character> OnTurnStart;
        public Action<Character> OnTurnEnd;
        public Action<Character, string, string> OnAnimationEvent;
        public Action<Character, bool> OnVisibilityChange;

        public CharacterData Data { get; private set; }

        public CharacterState CurrentState { get; protected set; }
        public bool WantCmd { get; set; }
        public CharacterState WantState { get; protected set; } = CharacterState.PrepareAttack;
        public bool IsVisible { get; set; }
        public MeshLayer CurrentMeshLayer { get; protected set; }

        protected MeshLayer _previousLayer;


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
        /// <param name="visibility"></param>
        public virtual void SetVisible(bool visibility) {
            if (visibility && CurrentMeshLayer == MeshLayer.Hidden) {
                SetMeshLayer(MeshLayer.Default);
            }
            else if (!visibility) {
                _previousLayer = CurrentMeshLayer;
                SetMeshLayer(MeshLayer.Hidden);
            }
            IsVisible = visibility;
            OnVisibilityChange?.Invoke(this, visibility);
        }
        // 사망 준비함수
        public virtual void PrepareDead() {
            // 캐릭터의 현재 상태를 사망준비상태로 처리
            CurrentState = CharacterState.PrepareDead;
        }
        // 캐릭터 사망
        public virtual void Dead() {
            // 캐릭터의 현재 상태를 사망으로 처리
            CurrentState = CharacterState.Dead;
            // 취할 상태를 사망준비로
            WantState = CharacterState.PrepareDead;
            // 명령대기를 하지 않음을 반환
            WantCmd = false;
        }
        // 그로기 준비함수
        public virtual void PrepareGroggy()
        {
            // 현재 상태를 그로기 준비상태로 처리
            CurrentState = CharacterState.PrepareGroggy;
        }

        // 에너미 그로기
        public virtual void Groggy() {
            // 캐릭터의 현재 상태를 그로기로 처리
            CurrentState = CharacterState.Groggy;
            // 취할 상태를 그로기 준비상태로
            WantState = CharacterState.PrepareGroggy;
            // 명령대기를 하지 않음을 반환
            WantCmd = false;
        }


        public virtual void SetMeshLayer(MeshLayer layer) {
            int layerID = 0;
            CurrentMeshLayer = layer;
            if (layer == MeshLayer.SkillTimeine) {
                layerID = 6;
            }
            else if (layer == MeshLayer.Hidden) {
                layerID = 7;
            }
            foreach (var child in meshParent.GetComponentsInChildren<Transform>(true)) {
                child.gameObject.layer = layerID;
            }
        }

        // 데미지 함수 (때린놈의 정보를 가져온다)
        public virtual void Damage(Character pl) {
            // 캐릭터의 현재 상태를 데미지로 처리
            CurrentState = CharacterState.Damage;

        }

        // 플레이어의 속성이 에너미의 약점 속성과 일치하는지 확인할 함수
        // --- 이건 맞을때 에너미 쪽에서 계산 ---
        public virtual bool Element_Check(Character player, Character enemy)
        {
            // 에너미의 약점 갯수만큼 for문을 돌린다
            for (int i = 0; i < enemy.Data.stats.Weakness.Count; i++)
            {
                // 플레이어의 공격 타입이 에너미의 약점 타입과 같다면
                if (player.Data.stats.ElementType == enemy.Data.stats.Weakness[i])
                {
                    // 있다면 true 를 반환한다
                    return true;
                }
            }
            // 없다면 false 를 반환한다
            return false;
        }

    }
}
