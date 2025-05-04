using UnityEngine;

namespace TurnBased.Entities.Field
{ 
    public class PlayerAttack
    {
        /// <summary>
        /// 에너미를 향해 공격할 함수
        /// </summary>        
        /// <param name="anim">플레이어의 애니메이터</param>
        public void Attack(Animator anim)
        {
            // 애니메이션을 정지
            anim.StopPlayback();
            
            // 애니메이션 트리거를 실행한다
            anim.SetTrigger("ToAttack");           
        }

    }

}