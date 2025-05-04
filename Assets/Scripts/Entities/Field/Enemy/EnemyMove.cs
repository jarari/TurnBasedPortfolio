using UnityEngine;

namespace TurnBased.Entities.Field { 

    /// <summary>
    /// 에너미의 움직임을 담당할 클래스
    /// </summary>
    public class EnemyMove
    {
        float speed = 3.0f;
        
        /// <summary>
        /// 캐릭터 컨트롤러가 움직일 벡터를 계산하는 함수
        /// </summary>
        /// <param name="player">추적할 대상</param>
        /// <param name="enemy">자기 자신</param>
        /// <param name="yn">벡터만을 반환할지 속도,시간까지 계산할지를 선택한다</param>
        /// <returns></returns>
        public Vector3 FE_MoveVector(GameObject player, GameObject enemy, bool yn)
        {
            // 추적할 대상과 자신의 거리를 뺀다음 정규화 시키고
            Vector3 dir = (player.transform.position - enemy.transform.position).normalized;


            if (yn == true)
            {
                // 거리와 속도 그리고 시간을 곱한값을 반환한다
                return dir * speed * Time.deltaTime;
            }
            else
            {
                // 벡터만을 반환한다
                return dir;
            }
        }

        /// <summary>
        /// 에너미가 공격이 가능한 거리 까지 온다면 불값을 반환 시켜 알릴 함수
        /// </summary>
        /// <param name="target">추적할 플레이어</param>
        /// <param name="enemy">에너미</param>
        /// <param name="distance">공격이 가능한 거리</param>
        /// <returns></returns>
        public bool FE_SwitchMove(Vector3 target, GameObject enemy, float distance)
        {
            // 에너미와 플레이어 사이의 거리가 공격 거리보다 같거나 작다면
            if (Vector3.Distance(enemy.transform.position, target) <= distance)
            {
                // true를 반환
                return true;
            }
            // 아니라면
            else if (Vector3.Distance(enemy.transform.position, target) > distance)
                // false를 반환
                return false;
            else
                return false;
        }
    }

}