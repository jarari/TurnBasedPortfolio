using UnityEngine;

namespace TurnBased.Entities.Field { 

    /// <summary>
    /// ���ʹ��� �������� ����� Ŭ����
    /// </summary>
    public class EnemyMove
    {
        float speed = 1.0f;
        
        /// <summary>
        /// �÷��̾ ������ �Լ�
        /// </summary>
        /// <param name="target">������ �÷��̾�</param>
        /// <param name="enemy">���ʹ�</param>
        /// <param name="cc">���ʹ��� ĳ���� ��Ʈ�ѷ�</param>
        public void FE_Move(Vector3 target, CharacterController cc, GameObject enemy)
        {
            // �̵� ������ �÷��̾� ��ġ�� �����Ѵ�
            Vector3 dir = (target - enemy.transform.position).normalized;
                        
            // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��� �����Ѵ�
            cc.Move(dir * speed * Time.deltaTime);
        }

        public void FE_Rotate(Vector3 target, CharacterController cc, GameObject enemy)
        {
            // ȸ�� ������ �÷��̾�� ���Ѵ�
            Vector3 dir = (target - cc.transform.position).normalized;
            // y�� ȸ�� ����
            dir.y = 0;
            // Ÿ�� ������ �ٶ󺸴� ȸ�� ����
            Quaternion targetRotate = Quaternion.LookRotation(dir);
            // ���� ȸ������ Ÿ�� ȸ������ ������ �ӵ��� ȸ��
            cc.transform.rotation = Quaternion.RotateTowards(cc.transform.rotation, targetRotate, 720 * Time.deltaTime);

        }

        /// <summary>
        /// ���ʹ̰� ������ ������ �Ÿ� ���� �´ٸ� �Ұ��� ��ȯ ���� �˸� �Լ�
        /// </summary>
        /// <param name="target">������ �÷��̾�</param>
        /// <param name="enemy">���ʹ�</param>
        /// <param name="distance">������ ������ �Ÿ�</param>
        /// <returns></returns>
        public bool FE_SwitchMove(Vector3 target, GameObject enemy, float distance)
        {
            // ���ʹ̿� �÷��̾� ������ �Ÿ��� ���� �Ÿ����� �۴ٸ�
            if (Vector3.Distance(enemy.transform.position, target) < distance)
            {
                // true�� ��ȯ
                return true;
            }
            // �ƴ϶��
            else
                // false�� ��ȯ
                return false;
        }
    }

}