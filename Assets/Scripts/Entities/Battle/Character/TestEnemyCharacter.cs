using UnityEngine;
using TurnBased.Battle;
using System.Collections;

namespace TurnBased.Entities.Battle {
    public class TestEnemyCharacter : Character {

        // ���ʹ��� ���� (�븻, ����)
        public enum EnemyState { Nomal, Rampage }
        // ��� ���� ����
        public EnemyState e_State;
        // ���� ���� ������Ʈ�� �����´�
                
        private void Start()
        {
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

            // �����ϴ� �Լ�
            DoAttack();
        }
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

        // �׷α� ���� �Լ�
        public void Groggy()
        {
            // ���ε��� 0���ϰ� �Ǿ�����
            if (Data.stats.CurrentToughness <= 0)
            { 
            }
        }
              

    }
}
