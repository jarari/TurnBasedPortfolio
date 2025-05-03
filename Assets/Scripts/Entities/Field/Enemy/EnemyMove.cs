using UnityEngine;

namespace TurnBased.Entities.Field { 

    /// <summary>
    /// 에너미의 움직임을 담당할 클래스
    /// </summary>
    public class EnemyMove
    {
        float speed = 3.0f;
        
        /// <summary>
        /// 플레이어를 추적할 함수
        /// </summary>
        /// <param name="target">추적할 플레이어</param>
        /// <param name="enemy">에너미</param>
        /// <param name="cc">에너미의 캐릭터 컨트롤러</param>
        public void FE_Move(Vector3 target, CharacterController cc, GameObject enemy)
        {
            // 이동 방향을 플레이어 위치로 설정한다
            Vector3 dir = (target - enemy.transform.position).normalized;

            // 에너미가 타겟을 바라보게 한다.
            enemy.transform.forward = target;

            // 캐릭터 컨트롤러를 이용해 이동을 시작한다
            cc.Move(dir * speed * Time.deltaTime);
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
            // 에너미와 플레이어 사이의 거리가 공격 거리보다 작다면
            if (Vector3.Distance(enemy.transform.position, target) < distance)
            {
                // true를 반환
                return true;
            }
            // 아니라면
            else
                // false를 반환
                return false;
        }
    }

}