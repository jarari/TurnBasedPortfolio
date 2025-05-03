using UnityEngine;


namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// 에너미 공격시 히트를 시그널을 전달할 클래스
    /// </summary>
    public class EnemySignal : F_Enemy
    {
        /// <summary>
        /// 어택 애니메이션 진행중 시그널을 받으면 실행되는데
        /// </summary>
        /// <param name="a">이건 F_Enemy쪽에서 넣을 공격 범위 내에 있는지 확인할 값</param>
        /// <returns></returns>
        public override bool hit_signal(bool a)
        {            
            // 만약 이 시그널을 수신하는 타이밍에 플레이어가 공격 가능 범위에 있다면
            if (a == true)
                // true를 리턴함
                return true;
            // 공격 가능 범위에 없다면
            else
                // false를 리턴함
                return false;
        }
    }

}