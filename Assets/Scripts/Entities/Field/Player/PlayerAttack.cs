using UnityEngine;

namespace TurnBased.Entities.Field
{ 
    public class PlayerAttack
    {
        private bool wait_att = false;

        /// <summary>
        /// ���ʹ̸� ���� ������ �Լ�
        /// </summary>
        /// <param name="enemy">���ʹ�</param>
        /// <param name="anim">�÷��̾��� �ִϸ�����</param>
        public void Attack(GameObject enemy, Animator anim)
        {
            // �ִϸ��̼��� ����
            anim.StopPlayback();
            
            // �ִϸ��̼� Ʈ���Ÿ� �����Ѵ�
            anim.SetTrigger("ToAttack");                    
            
            // ��ó�� Ž���� ���ʹ̰� �ִٸ�
            if (enemy != null)
            {
                // ���ʹ̸� �ٶ󺸰��ϰ�
                anim.gameObject.transform.forward = enemy.transform.position;                              
            }          
        }

    }

}