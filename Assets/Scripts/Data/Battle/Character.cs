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

        public virtual void TakeUltTurn() {
            WantCmd = true;
            PrepareUltAttack();
            OnUltTurn?.Invoke(this);
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
        /// <param name="visibility">������ �Ⱥ�����</param>
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
        /// ��� �غ��Լ�
        /// </summary>
        public virtual void PrepareDead() {
            // ĳ������ ���� ���¸� ����غ���·� ó��
            CurrentState = CharacterState.PrepareDead;
        }
        /// <summary>
        /// ĳ���� ���
        /// </summary>
        public virtual void Dead() {
            // ĳ������ ���� ���¸� ������� ó��
            CurrentState = CharacterState.Dead;
            // ���� ���¸� ����غ��
            WantState = CharacterState.PrepareDead;
            // ��ɴ�⸦ ���� ������ ��ȯ
            WantCmd = false;
            // �� ť���� ĳ���� ����
            TurnManager.instance.RemoveCharacter(this);

            Debug.Log("Dead �Լ� ����");
        }
        /// <summary>
        /// �׷α� �غ��Լ�
        /// </summary>
        public virtual void PrepareGroggy()
        {
            // ���� ���¸� �׷α� �غ���·� ó��
            CurrentState = CharacterState.PrepareGroggy;
        }

        /// <summary>
        /// �׷α� ���� ����
        /// </summary>
        public virtual void Groggy() {
            // ĳ������ ���� ���¸� �׷α�� ó��
            CurrentState = CharacterState.Groggy;
            // ���� ���¸� �׷α� �غ���·�
            WantState = CharacterState.PrepareGroggy;
            // ��ɴ�⸦ ���� ������ ��ȯ
            WantCmd = false;
        }

        /// <summary>
        /// �޽� ���̾� ����
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
        /// ������ �Լ� (��� �� ����� �̿��ؼ� ����)
        /// </summary>
        /// <param name="attacker"></param>
        public virtual void Damage(Character attacker, DamageResult result) {
            // ĳ������ ���� ���¸� �������� ó��
            CurrentState = CharacterState.Damage;

            if (Data.stats.MaxToughness > 0) {
                // ���� ���ε��� �ִٸ�
                if (Data.stats.CurrentToughness > 0) {

                    // ä���� ���������� �޴� �������� ������ �ް�
                    Data.stats.CurrentHP -= (result.FinalDamage / 2);

                    // �÷��̾ ���� �Ӽ����� �����ٸ�
                    if (CombatManager.CheckElementMatch(attacker.Data.stats.ElementType, Data.stats.Weakness)) {
                        // ���ʹ��� ���ε��� �÷��̾��� ���ݷ¸�ŭ ���δ�
                        Data.stats.CurrentToughness -= result.NormalAttack;

                        // ���ε��� ���� 0���ϰ� �Ǿ��ٸ�
                        if (Data.stats.CurrentToughness <= 0) {
                            // ���� ���ε��� 0���� �����
                            Data.stats.CurrentToughness = 0;
                            // �׷α⸦ �ٷ� �Լ��� �����Ѵ�
                            Groggy();
                        }
                    }

                    // ä���� ���� 0���ϰ� �Ǿ��ٸ�
                    if (Data.stats.CurrentHP <= 0) {
                        // ������ �ٷ� �Լ��� �����Ѵ�
                        Dead();
                    }
                }
                // ���� ���ε��� ���ٸ�
                else {
                    // ���ʹ̴� ���������� �޴� �������� ��� �޴´�.
                    Data.stats.CurrentHP -= result.FinalDamage;

                    // ä���� ���� 0���ϰ� �Ǿ��ٸ�
                    if (Data.stats.CurrentHP <= 0) {
                        // ������ �ٷ� �Լ��� �����Ѵ�
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
