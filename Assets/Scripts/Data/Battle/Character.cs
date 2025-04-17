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
        /// �� ���� �� ����
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
            // ���� ���ʹ��϶�
            else
            {
                // ���� ���� ���̿��� �� ���� ���°� ���� �غ���
                if (WantState == CharacterState.PrepareAttack)
                {
                    // ���� �غ� �Լ��� ȣ��
                    PrepareAttack();
                }
                // ���� ���� ���� ���� �� ���� ���°� ��ų �غ���
                else if (WantState == CharacterState.PrepareSkill)
                {
                    // ��ų �غ� �Լ��� ȣ��
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
        /// �ñر� Q �غ��ڼ�
        /// </summary>
        public virtual void PrepareUltAttack() {
            CurrentState = CharacterState.PrepareUltAttack;
        }
        /// <summary>
        /// �ñر� Q �ߵ�
        /// </summary>
        public virtual void CastUltAttack() {
            CurrentState = CharacterState.CastUltAttack;
            WantCmd = false;
        }
        /// <summary>
        /// �ñر� E �غ��ڼ�
        /// </summary>
        public virtual void PrepareUltSkill() {
            CurrentState = CharacterState.PrepareUltSkill;
        }
        /// <summary>
        /// �ñر� E �ߵ�
        /// </summary>
        public virtual void CastUltSkill() {
            CurrentState = CharacterState.CastUltSkill;
            WantCmd = false;
        }
        /// <summary>
        /// ĳ���� �� Ȱ��ȭ/��Ȱ��ȭ
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
        // ��� �غ��Լ�
        public virtual void PrepareDead() {
            // ĳ������ ���� ���¸� ����غ���·� ó��
            CurrentState = CharacterState.PrepareDead;
        }
        // ĳ���� ���
        public virtual void Dead() {
            // ĳ������ ���� ���¸� ������� ó��
            CurrentState = CharacterState.Dead;
            // ���� ���¸� ����غ��
            WantState = CharacterState.PrepareDead;
            // ��ɴ�⸦ ���� ������ ��ȯ
            WantCmd = false;
        }
        // �׷α� �غ��Լ�
        public virtual void PrepareGroggy()
        {
            // ���� ���¸� �׷α� �غ���·� ó��
            CurrentState = CharacterState.PrepareGroggy;
        }

        // ���ʹ� �׷α�
        public virtual void Groggy() {
            // ĳ������ ���� ���¸� �׷α�� ó��
            CurrentState = CharacterState.Groggy;
            // ���� ���¸� �׷α� �غ���·�
            WantState = CharacterState.PrepareGroggy;
            // ��ɴ�⸦ ���� ������ ��ȯ
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

        // ������ �Լ� (�������� ������ �����´�)
        public virtual void Damage(Character pl) {
            // ĳ������ ���� ���¸� �������� ó��
            CurrentState = CharacterState.Damage;

        }

        // �÷��̾��� �Ӽ��� ���ʹ��� ���� �Ӽ��� ��ġ�ϴ��� Ȯ���� �Լ�
        // --- �̰� ������ ���ʹ� �ʿ��� ��� ---
        public virtual bool Element_Check(Character player, Character enemy)
        {
            // ���ʹ��� ���� ������ŭ for���� ������
            for (int i = 0; i < enemy.Data.stats.Weakness.Count; i++)
            {
                // �÷��̾��� ���� Ÿ���� ���ʹ��� ���� Ÿ�԰� ���ٸ�
                if (player.Data.stats.ElementType == enemy.Data.stats.Weakness[i])
                {
                    // �ִٸ� true �� ��ȯ�Ѵ�
                    return true;
                }
            }
            // ���ٸ� false �� ��ȯ�Ѵ�
            return false;
        }

    }
}
