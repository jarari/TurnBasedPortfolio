using UnityEngine;

namespace TurnBased.Entities.Field
{ 
    public class PlayerAttack
    {
        private bool wait_att = false;

        /// <summary>
        /// 에너미를 향해 공격할 함수
        /// </summary>
        /// <param name="enemy">에너미</param>
        /// <param name="anim">플레이어의 애니메이터</param>
        public void Attack(GameObject enemy, Animator anim)
        {
            // 애니메이션을 정지
            anim.StopPlayback();
            
            // 애니메이션 트리거를 실행한다
            anim.SetTrigger("ToAttack");                    
            
            // 근처에 탐지된 에너미가 있다면
            if (enemy != null)
            {
                // 에너미를 바라보게하고
                anim.gameObject.transform.forward = enemy.transform.position;                              
            }          
        }

    }

}