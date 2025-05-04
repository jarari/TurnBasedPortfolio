using UnityEngine;

namespace TurnBased.Entities.Field
{ 
    public class PlayerAttack
    {
        /// <summary>
        /// ���ʹ̸� ���� ������ �Լ�
        /// </summary>        
        /// <param name="anim">�÷��̾��� �ִϸ�����</param>
        public void Attack(Animator anim)
        {
            // �ִϸ��̼��� ����
            anim.StopPlayback();
            
            // �ִϸ��̼� Ʈ���Ÿ� �����Ѵ�
            anim.SetTrigger("ToAttack");           
        }

    }

}