using UnityEngine;


namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// ���ʹ� ���ݽ� ��Ʈ�� �ñ׳��� ������ Ŭ����
    /// </summary>
    public class EnemySignal : F_Enemy
    {
        /// <summary>
        /// ���� �ִϸ��̼� ������ �ñ׳��� ������ ����Ǵµ�
        /// </summary>
        /// <param name="a">�̰� F_Enemy�ʿ��� ���� ���� ���� ���� �ִ��� Ȯ���� ��</param>
        /// <returns></returns>
        public override bool hit_signal(bool a)
        {            
            // ���� �� �ñ׳��� �����ϴ� Ÿ�ֿ̹� �÷��̾ ���� ���� ������ �ִٸ�
            if (a == true)
                // true�� ������
                return true;
            // ���� ���� ������ ���ٸ�
            else
                // false�� ������
                return false;
        }
    }

}