using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using System.Collections;

namespace TurnBased.Entities.Battle {
    public class TestEnemyCharacter : Character {
        
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

        // Ÿ������ �÷��̾ ���� ����
        public Character ch_player;
        public Character[] ch_players;

        // ������ ��ġ�� ȸ������ ���� ����
        public Vector3 EnPosition;
        public Vector3 EnRotate;
                
        protected override void Start()
        {
            base.Start();
            // ���۽� ���ʹ��� ���¸� Nomal�� �Ѵ�
            e_State = EnemyState.Nomal;
            // ���۽� ���ʹ��� ���� ��ġ�� ȸ������ �����Ѵ�
            EnPosition = transform.position;            
            EnRotate = transform.eulerAngles;
        }
                

        // ������ ���� �Ǿ�����
        private IEnumerator WaitAttackEnd() {
            yield return null;
            // �����ϱ� ���� Ʋ���� ȸ������ ������� �����´�
            transform.eulerAngles = EnRotate;
            // ���� ������
            EndTurn();
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

        }
        // �ñر�
        public override void CastUlt() {
            base.CastUlt();
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
                       

            // ������ ���� �ڷ�ƾ�� �����Ѵ�
            StartCoroutine(WaitAttackEnd());
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

        // ���������� ��� �� �������� ���� �Լ�


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
        // �Ʊ� ĳ����ĭ�� �ִ� �÷��̾� ĳ���͸� �����ϰ� ������ ��ŭ �����´�
        public void ChPlayer_M(int x)
        {
            // ������ ���ڸ� �̾�
            int ran = Random.Range(0, 2);
            // ĳ���� �޴����� ��ϵ� ��� �Ʊ� ĳ���� ����Ʈ�� �����´� (�Ŀ� ������ ĳ���͸� ������ ����)
            var player = CharacterManager.instance.GetAllyCharacters();
            
        }

    }
}
