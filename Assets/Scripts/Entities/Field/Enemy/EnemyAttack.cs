using UnityEngine;

namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// 에너미의 공격시 Scene전환을 담당할 클래스
    /// </summary>
    public class EnemyAttack
    {
        /// <summary>
        /// 전투 씬으로 전환할 함수
        /// </summary>
        public void ChangeScene()
        {
            Debug.Log("전투씬 전환");
        }
    }

}