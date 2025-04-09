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
            PrepareUlt,
            CastUlt
        }

        public enum MeshLayer {
            Default,
            SkillTimeine
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


        protected virtual void Awake() {
            Data = Instantiate(_baseData);
            Data.stats.CurrentToughness = Data.stats.MaxToughness;
            Data.stats.CurrentHP = Data.stats.MaxHP;
        }

        protected virtual void Start() {
            SetVisible(true);
        }

        /// <summary>
        /// �� ���� �� ����
        /// </summary>
        public virtual void TakeTurn() {
            WantCmd = true;
            if (Data.team == CharacterTeam.Player) {
                if (WantState == CharacterState.PrepareAttack) {
                    PrepareAttack();
                }
                else if (WantState == CharacterState.PrepareSkill) {
                    PrepareSkill();
                }
            }
            OnTurnStart?.Invoke(this);
        }

        /// <summary>
        /// �� ���� �� ����
        /// </summary>
        public virtual void EndTurn() {
            CurrentState = CharacterState.Idle;
            TurnManager.instance.EndTurn();
            OnTurnEnd?.Invoke(this);
        }

        /// <summary>
        /// �ִϸ��̼� �̺�Ʈ ó��. �̺�Ʈ�� �̺�Ʈ��.�߰������� ���� (��: SkillCast.1)
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
        /// �Ϲ� ���� �غ��ڼ�
        /// </summary>
        public virtual void PrepareAttack() {
            CurrentState = CharacterState.PrepareAttack;
        }
        /// <summary>
        /// �Ϲ� ���� �ߵ�
        /// </summary>
        public virtual void DoAttack() {
            CurrentState = CharacterState.DoAttack;
            WantState = CharacterState.PrepareAttack;
            WantCmd = false;
        }
        /// <summary>
        /// �߰� ���� �ߵ�
        /// </summary>
        public virtual void DoExtraAttack() {
            CurrentState = CharacterState.DoExtraAttack;
        }
        /// <summary>
        /// ��ų ���� �غ��ڼ�
        /// </summary>
        public virtual void PrepareSkill() {
            CurrentState = CharacterState.PrepareSkill;
        }
        /// <summary>
        /// ��ų ���� �ߵ�
        /// </summary>
        public virtual void CastSkill() {
            CurrentState = CharacterState.CastSkill;
            WantState = CharacterState.PrepareSkill;
            WantCmd = false;
        }
        /// <summary>
        /// �ñر� �غ��ڼ�
        /// </summary>
        public virtual void PrepareUlt() {
            CurrentState = CharacterState.PrepareUlt;
        }
        /// <summary>
        /// �ñر� �ߵ�
        /// </summary>
        public virtual void CastUlt() {
            CurrentState = CharacterState.CastUlt;
            WantCmd = false;
        }
        /// <summary>
        /// ĳ���� �� Ȱ��ȭ/��Ȱ��ȭ
        /// </summary>
        /// <param name="visibility"></param>
        public virtual void SetVisible(bool visibility) {
            meshParent?.SetActive(visibility);
            IsVisible = visibility;
            OnVisibilityChange?.Invoke(this, visibility);
        }

        public virtual void SetMeshLayer(MeshLayer layer) {
            int layerID = 0;
            if (layer == MeshLayer.SkillTimeine) {
                layerID = 6;
            }
            foreach (var child in meshParent.GetComponentsInChildren<Transform>(true)) {
                child.gameObject.layer = layerID;
            }
        }
    }
}
