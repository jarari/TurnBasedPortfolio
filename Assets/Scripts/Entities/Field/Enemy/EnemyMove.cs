using UnityEngine;

namespace TurnBased.Entities.Field { 

    /// <summary>
    /// ���ʹ��� �������� ����� Ŭ����
    /// </summary>
    public class EnemyMove
    {
        float speed = 3.0f;
        
        /// <summary>
        /// ĳ���� ��Ʈ�ѷ��� ������ ���͸� ����ϴ� �Լ�
        /// </summary>
        /// <param name="player">������ ���</param>
        /// <param name="enemy">�ڱ� �ڽ�</param>
        /// <param name="yn">���͸��� ��ȯ���� �ӵ�,�ð����� ��������� �����Ѵ�</param>
        /// <returns></returns>
        public Vector3 FE_MoveVector(GameObject player, GameObject enemy, bool yn)
        {
            // ������ ���� �ڽ��� �Ÿ��� ������ ����ȭ ��Ű��
            Vector3 dir = (player.transform.position - enemy.transform.position).normalized;


            if (yn == true)
            {
                // �Ÿ��� �ӵ� �׸��� �ð��� ���Ѱ��� ��ȯ�Ѵ�
                return dir * speed * Time.deltaTime;
            }
            else
            {
                // ���͸��� ��ȯ�Ѵ�
                return dir;
            }
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
            // ���ʹ̿� �÷��̾� ������ �Ÿ��� ���� �Ÿ����� ���ų� �۴ٸ�
            if (Vector3.Distance(enemy.transform.position, target) <= distance)
            {
                // true�� ��ȯ
                return true;
            }
            // �ƴ϶��
            else if (Vector3.Distance(enemy.transform.position, target) > distance)
                // false�� ��ȯ
                return false;
            else
                return false;
        }
    }

}