using UnityEngine;
using TurnBased.Battle;
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
        public bool ram = false;
                
        protected override void Start()
        {
            base.Start();
            // ���۽� ���ʹ��� ���¸� Nomal�� �Ѵ�
            e_State = EnemyState.Nomal;            
            
        }

        // ������ ���� �Ǿ�����
        private IEnumerator WaitAttackEnd() {
            yield return null;
            EndTurn();
        }

        // ���� �޾�����
        public override void TakeTurn() {
            // �θ� Ŭ�������� TakeTurn ���� �� ����
            base.TakeTurn();

            // ���ε��� 0���ϰ� �Ǿ�����
            if (Data.stats.CurrentToughness <= 0)
            {
                // ���� ������
                EndTurn();
            }
            // ���ε��� 0���ϰ� �ƴҶ�
            else
            { 
                // �����ϴ� �Լ�
                DoAttack();
            }
        }

        #region �ൿ�ϴ� �Լ� (��ų, ����, �ñر�, ����Ʈ�� ����)

        // ��ų�� ����Ҷ�
        public override void CastSkill() {
            // �θ� Ŭ�������� CastSkill ������ ����
            base.CastSkill();

            // ���ʹ��� ���¿� ���� ������ ������
            switch (e_State)
            {
                case EnemyState.Nomal:
                    break;
                case EnemyState.Rampage:
                    break;
            }

        }
        // �ñر�
        public override void CastUlt() {
            base.CastUlt();
        }
        // ������ �����Ҷ�
        public override void DoAttack() {
            base.DoAttack();
            Debug.Log("Enemy Attack");

            // ���ʹ��� ���¿� ���� ������ ������
            switch (e_State)
            {
                case EnemyState.Nomal:
                    break;
                case EnemyState.Rampage:
                    break;
            }
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


        // �������� �޾�����
        // �÷��̾��� ���ݷ��� ���� damage�� �ӽ÷� ���� ���� ���濹��
        public void Damaged(float damage)
        {
            // ���� ä�¿��� ���ݹ��� ĳ������ ���ݷ¸�ŭ ä���� ����
            Data.stats.CurrentHP -= damage;
            
            // ���� ä���� 0���� Ŭ��
            if (Data.stats.CurrentHP > 0)
            { 
            }
            // ���� ä���� 0���ϰ� �Ǿ�����
            else
            {
                // ������ �ٷ� �Լ�
                Dead();
            }
            
            // ���� ���ʹ� ������ �����϶�
            if (e_Type == EnemyType.Boss)
            { 
                // ���ʹ��� ���� ä���� ��ä ä���� ���� ���ϰ� �ǰ� ����ȭ �Ұ��� false�϶�
                if (Data.stats.CurrentHP <= (Data.stats.MaxHP) / 2 && ram == false)
                {
                    // ĳ������ ���¸� ����ȭ �� �����Ѵ�
                    e_State = EnemyState.Rampage;
                    // �Ұ��� �����Ѵ�
                    ram = true;
                }
            }

        }

        // ������ �ٷ��Լ�
        public void Dead()
        {
            // ĳ���� ���� ��Ȱ��ȭ
            SetVisible(false);
        }

    }
}
