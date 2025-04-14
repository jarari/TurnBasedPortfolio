using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;
using static TurnBased.Entities.Battle.TestEnemyCharacter;


namespace TurnBased.Entities.Battle { 
    
    // ���� ����
    public class BossEnemy : Character, IDamageApply
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // �Ϲ� ���� �ִϸ��̼�
        public PlayableDirector skillAttack;        // ��ų ���� �ִϸ��̼�
        public PlayableDirector UltAttack;          // �ʻ�� ���� �ִϸ��̼�
        public PlayableDirector Groggy;             // �׷α� �ִϸ��̼�

        // ==( ����ȭ�� ���ݴ� ����� ���� ����... )==

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        #region ���ʹ��� ����, ����

        // ���ʹ��� ���� (�븻, ����)
        public enum EnemyState { Nomal, Rampage }
        // ��� ���� ����
        public EnemyState e_State;

        // ����ȭ ���¸� ������ �Ұ�
        bool ram = false;

        // ä�� ������ �־��...? (������������?)

        #endregion

        // Ÿ������ �÷��̾ ���� ����
        public Character ch_player;
        public Character[] ch_players;

        // ������ ��ġ�� ȸ������ ���� ����
        public Vector3 EnPosition;
        public Vector3 EnRotate;

        // ĳ������ ������ ���� ���¸� ���� ����
        private CharacterState _lastAttack;

        // �����Ŀ� �ִϸ��̼��� ���� ���� ��ȯ�� ó���ϴ� �ڷ�ƾ
        private IEnumerator DelayReturnFromAttack()
        {
            // �Ͻ� ���� ���� ���� �����ӿ��� ������
            yield return null;
            // ������ ���� ���°� DoAttack �� ���
            if (_lastAttack == CharacterState.DoAttack)
            {
                // �Ϲ� ���� �ִϸ��̼��� ������ �����Ų��
                normalAttack.time = normalAttack.duration;
                // Ÿ�Ӷ����� ���� �ð��� �°� ���¸� ������Ʈ
                normalAttack.Evaluate();
            }
            // ������ ���� ���°� CastSkill �� ���
            else if (_lastAttack == CharacterState.CastSkill)
            {
                // ��ų ���� �ִϸ��̼��� ������ �����Ų��
                skillAttack.time = skillAttack.duration;
                // Ÿ�Ӷ����� ���� �ð��� �°� ���¸� ������Ʈ
                skillAttack.Evaluate();
            }
            else if (_lastAttack == CharacterState.CastUlt)
            {
                // �ʻ�� ���� �ִϸ��̼��� ������ �����Ų��
                UltAttack.time = UltAttack.duration;
                // Ÿ�Ӷ����� ���� �ð��� �°� ���¸� ������Ʈ
                UltAttack.Evaluate();
            }

        }

        // �ִϸ��̼� �̺�Ʈ�� �߻�������� ó���ϴ� �Լ�
        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload)
        {
            if (animEvent == "NomalAttackEnd" || animEvent == "SkillAttackEnd" || animEvent == "UltAttackEnd")
            {
                // ������ �ִϸ��̼� ó�� �ڷ�ƾ�� ȣ����
                StartCoroutine(DelayReturnFromAttack());
                // �����ϱ� ���� Ʋ���� ȸ������ ������� �����´�
                transform.eulerAngles = EnRotate;
                // ���� �����Ѵ�
                EndTurn();
            }
        }
        protected override void Awake()
        {
            base.Awake();
            // �̺�Ʈ�� �߻����� ��� ����� �Լ��� �߰��Ѵ�
            OnAnimationEvent += OnAnimationEvent_Impl;
        }

        protected override void Start()
        {
            base.Start();
            // ���۽� ���ʹ��� ���¸� Nomal�� �Ѵ�
            e_State = EnemyState.Nomal;
            // ���۽� ���ʹ��� ���� ��ġ�� ȸ������ �����Ѵ�
            EnPosition = transform.position;
            EnRotate = transform.eulerAngles;
        }

        // ���� �޾�����
        public override void TakeTurn()
        {
            // �θ� Ŭ�������� TakeTurn ���� �� ����
            base.TakeTurn();
       
            // �Ұ��� �̿��� �ѹ��� ȣ�� �ǵ����Ѵ�
            // ���ʹ��� ���� ä���� ��ä ä���� ���� ���ϰ� �ǰ� ����ȭ �Ұ��� false�϶�
            if (Data.stats.CurrentHP <= (Data.stats.MaxHP / 2) && ram == false)
            {
                // ĳ������ ���¸� ����ȭ �� �����Ѵ�
                e_State = EnemyState.Rampage;
                // ����ȭ�� ���ݷ��� 1.5���Ѵ�
                Data.stats.Attack += (Data.stats.Attack / 2);

                // �Ұ��� �����Ѵ�
                ram = true;
            }            

            // �����ϴ� �Լ�
            DoAttack();

            // ���� ������
            EndTurn();

        }

        #region �ൿ�ϴ� �Լ� (��ų, ����, �ñر�, ����Ʈ�� ����)

        // ��ų�� ����Ҷ�
        public override void CastSkill()
        {
            // �θ� Ŭ�������� CastSkill ������ ����
            base.CastSkill();
            Debug.Log(gameObject.name + " �� ��ų �ߵ�!");
            // �÷��̾� ��ü�� ����
            ChPlayer_M();

            // �ټ��� �÷��̾��� ��� �÷��̾ �����´�
            Character target_M = ch_players[ch_players.Length / 2];
            // ���ʹ̸� ��� �÷��̾ ���ϰ� �Ѵ�
            transform.forward = target_M.gameObject.transform.position;

            // ��ų ���� �ִϸ��̼� ���
            //skillAttack.Play();

            // ������ ������ ��ų�������� �˸���
            _lastAttack = CharacterState.CastSkill;
        }
        // �ñر�
        public override void CastUlt()
        {
            base.CastUlt();
            Debug.Log(gameObject.name + " �� �ʻ�� �ߵ�!");

            // �ټ��� �÷��̾��� ��� �÷��̾ �����´�
            Character target_M = ch_players[ch_players.Length / 2];
            // ���ʹ̸� ��� �÷��̾ ���ϰ� �Ѵ�
            transform.forward = target_M.gameObject.transform.position;


            // �ʻ�� ���� �ִϸ��̼� ���
            //UltAttack.Play();

            // ������ ������ �ʻ������ ������
            _lastAttack = CharacterState.CastUlt;
        }
        // ������ �����Ҷ�
        public override void DoAttack()
        {
            base.DoAttack();
            Debug.Log("Enemy Attack");

            // Ÿ������ ���� �÷��̾� ĳ���͸� �����ϰ� ����
            ChPlayer_S();
            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            transform.forward = ch_player.gameObject.transform.position;

            // ���� ������ �����ϴ� ������Ʈ�� �̸� (�׽�Ʈ��)
            Debug.Log(gameObject.name + " �� ����! ");


            // �ڱ��ڽ��� ĳ���͸� �����´�
            Character ch = GetComponent<Character>();
            // �������� ����ϴ� �Լ��� ȣ���ϰ�
            DamageResult result = CombatManager.DoDamage(ch, ch_player);

            Debug.Log(ch_player.name + " ���� " + result.FinalDamage + " ������");
            // ( �÷��̾��� �������Լ��� ȣ�� �Ǵ� ���⼭ ���)

            // �Ϲݰ��� �ִϸ��̼��� ����
            //normalAttack.Play();

            // ������ ������ �Ϲݰ������� ������
            _lastAttack = CharacterState.DoAttack;

        }

        // ����Ʈ�� ������ �Ҷ�
        public override void DoExtraAttack()
        {
            base.DoExtraAttack();
        }

        #endregion

        #region �غ��ϴ� �Լ� (����, ��ų, �ñر�)

        // ������ �غ��ϴ� �Լ�
        public override void PrepareAttack()
        {
            base.PrepareAttack();
        }
        // ��ų�� �غ��ϴ� �Լ�
        public override void PrepareSkill()
        {
            base.PrepareSkill();
        }
        // �ñر⸦ �غ��ϴ� �Լ�
        public override void PrepareUlt()
        {
            base.PrepareUlt();
        }

        #endregion


        // Ȥ�� ���� ���� ������ �Լ�
        public void DamageApply(Character pl)
        {
            // �ڱ��ڽ��� ĳ���͸� �����´�
            Character ch = GetComponent<Character>();
            // �������� ����ϴ� �Լ��� ȣ�� (������ , ������)
            DamageResult result = CombatManager.DoDamage(pl, ch);

            // ���� ���ε��� �ִٸ�
            if (ch.Data.stats.CurrentToughness > 0)
            {
                // ä���� ���������� �޴� �������� ������ �ް�
                this.Data.stats.CurrentHP -= (result.FinalDamage / 2);

                // ä���� ���� 0���ϰ� �Ǿ��ٸ�
                if (ch.Data.stats.CurrentHP <= 0)
                {
                    // ������ �ٷ� �ڷ�ƾ�� �����Ѵ�
                    StartCoroutine(DeadProcess());
                }

                // �÷��̾ ���� �Ӽ����� �����ٸ�
                if (Element_Check(pl, ch) == true)
                {
                    // ���ʹ��� ���ε��� �÷��̾��� ���ݷ¸�ŭ ���δ�
                    this.Data.stats.CurrentToughness -= result.NormalAttack;

                    // ���ε��� ���� 0���ϰ� �Ǿ��ٸ�
                    if (ch.Data.stats.CurrentToughness <= 0)
                    {
                        // �׷α⸦ �ٷ� �ڷ�ƾ�� �����Ѵ�
                        StartCoroutine(GroggyProcess());
                    }

                }

            }
            // ���� ���ε��� ���ٸ�
            else
            {
                // ���ʹ̴� ���������� �޴� �������� ��� �޴´�.
                this.Data.stats.CurrentHP -= result.FinalDamage;

                // ä���� ���� 0���ϰ� �Ǿ��ٸ�
                if (ch.Data.stats.CurrentHP <= 0)
                {
                    // ������ �ٷ� �ڷ�ƾ�� �����Ѵ�
                    StartCoroutine(DeadProcess());
                }

            }

        }

        // ������ �ٷ� �ڷ�ƾ
        IEnumerator DeadProcess()
        {
            yield return null;
            // ĳ���� ���� ��Ȱ��ȭ
            SetVisible(false);
        }

        // �׷α� �ڷ�ƾ
        IEnumerator GroggyProcess()
        {
            yield return null;
            // ���� ���ǵ�� ������ �������� �Ѵ�
            Data.stats.Speed = (Data.stats.Speed) / 2;
            Data.stats.Defense = (Data.stats.Defense) / 2;
        }

        // �Ʊ� ĳ����ĭ�� �ִ� �÷��̾� ĳ���͸� �����ϰ� �ϳ��� �����´�
        public void ChPlayer_S()
        {
            // ������ ���ڸ� �̾�
            int ran = Random.Range(0, 2);
            // ĳ���� �޴����� ��ϵ� ��� �Ʊ� ĳ���� ����Ʈ�� �����´� (�Ŀ� ������ ĳ���͸� ������ ����)
            var player = CharacterManager.instance.GetAllyCharacters();
            // ������ �Ʊ� ĳ���͸� ����Ʈ���� �����´�
            ch_player = player[ran];
        }
        // �Ʊ� ĳ����ĭ�� �ִ� �÷��̾� ĳ���͸� ��� �����´�
        public void ChPlayer_M()
        {
            // ĳ���� �޴����� ��ϵ� ��� �Ʊ� ĳ���� ����Ʈ�� �����´� (�Ŀ� ������ ĳ���͸� ������ ����)
            var player = CharacterManager.instance.GetAllyCharacters();
            // �޾ƿ� ����Ʈ�� ���� ��ŭ for���� ������
            for (int i = 0; i < player.Count; i++)
            {
                ch_players[i] = player[i];
            }

        }

        // �÷��̾��� �Ӽ��� ���ʹ��� ���� �Ӽ��� ��ġ�ϴ��� Ȯ���� �Լ�
        public bool Element_Check(Character player, Character enemy)
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