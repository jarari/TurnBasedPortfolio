using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;

namespace TurnBased.Entities.Battle {
    public class TestEnemyCharacter : Character {

        [Header("Timelines")]
        public PlayableDirector normalAttack;    // �Ϲ� ���� �ִϸ��̼�
        public PlayableDirector skillAttack;        // ��ų ���� �ִϸ��̼�
        public PlayableDirector UltAttack;          // �ʻ�� ���� �ִϸ��̼�
        public PlayableDirector Groggy;             // �׷α� �ִϸ��̼�

        // ==( ����ȭ�� ���ݴ� ����� ���� ����... )==

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        #region ���ʹ��� ����, ����

        // ���ʹ��� ���� ( �Ϲ� ��, ����)
        public enum EnemyType { Minion,Boss }
        // ��� ���� ����
        public EnemyType e_Type;

        // ���ʹ��� ���� (�븻, ����)
        public enum EnemyState { Nomal, Rampage }
        // ��� ���� ����
        public EnemyState e_State;

        // ����ȭ ���¸� ������ �Ұ�
        bool ram = false;

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
        public override void TakeTurn() {
            // �θ� Ŭ�������� TakeTurn ���� �� ����
            base.TakeTurn();
             
            
            // ���� ���ʹ� ������ �����϶�
            if (e_Type == EnemyType.Boss)
            {
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
            }

            // �����ϴ� �Լ�
            DoAttack();
            
            // ���� ������
            EndTurn();
 
        }

        #region �ൿ�ϴ� �Լ� (��ų, ����, �ñر�, ����Ʈ�� ����)

        // ��ų�� ����Ҷ�
        public override void CastSkill() {
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
        public override void CastUlt() {
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
        public override void DoAttack() {
            base.DoAttack();
            Debug.Log("Enemy Attack");
            
            // Ÿ������ ���� �÷��̾� ĳ���͸� �����ϰ� ����
            ChPlayer_S();
            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            transform.forward = ch_player.gameObject.transform.position;
            
            // ���� ������ �����ϴ� ������Ʈ�� �̸� (�׽�Ʈ��)
            Debug.Log(gameObject.name + " �� ����! ");

            // ( ���߿� ������ ���� Ŭ�������� ��������� ȣ������) //

            // �Ϲݰ��� �ִϸ��̼��� ����
            //normalAttack.Play();

            // ������ ������ �Ϲݰ������� ������
            _lastAttack = CharacterState.DoAttack;
            
        }

        // ����Ʈ�� ������ �Ҷ�
        public override void DoExtraAttack() {
            base.DoExtraAttack();
        }

        #endregion


        #region �غ��ϴ� �Լ� (����, ��ų, �ñر�)

        // ������ �غ��ϴ� �Լ�
        public override void PrepareAttack() {
            base.PrepareAttack();
        }
        // ��ų�� �غ��ϴ� �Լ�
        public override void PrepareSkill() {
            base.PrepareSkill();
        }
        // �ñر⸦ �غ��ϴ� �Լ�
        public override void PrepareUlt() {
            base.PrepareUlt();
        }

        #endregion
                

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

    }
}
