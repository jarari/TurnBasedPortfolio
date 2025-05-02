using UnityEngine;
using System.Linq;

namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// 플레이어를 탐지하는 기능만 담당하는 클래스
    /// </summary>
    public class EnemyDetector
    {
        /// <summary>
        /// 현재 위치에서 정해진 범위내에 있는 오브젝트중 특정 태그를 가진 오브젝트중의 첫번째를 탐지한다
        /// </summary>
        /// <param name="origin">탐지를 시작할 위치</param>
        /// <param name="range">탐지 할 범위</param>
        /// <param name="targetTag">탐지 할 태그</param>
        /// <returns>탐지된 GameObejct 없다면 null을 반환</returns>
        public GameObject Detect(Vector3 origin, float range, string targetTag)
        {
            // 현재 범위 내에 모든 콜라이더를 탐지
            Collider[] hits = Physics.OverlapSphere(origin, range);
            
            // hits 배열에서, 태그가 targetTag인 첫번째 collider의 게임 오브젝트를 반환
            var targetColl = hits.FirstOrDefault(collider => collider.CompareTag(targetTag));

            // 태그가 맞는 콜라이더가 있다면 해당 게임 오브젝트를 반환, 없으면 null을 반환
            return targetColl?.gameObject;
        }
      
    }
    

}