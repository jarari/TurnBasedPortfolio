using UnityEngine;


namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// 에너미 공격시 히트를 시그널을 전달할 클래스
    /// </summary>
    public class EnemySignal : MonoBehaviour
    {
        // 부모 오브젝트에서 가져올 에너미 스크립트
        public F_Enemy enemy;

        private void Awake()
        {
            // 에너미 스크립트가 비어있을경우
            if(enemy == null)
                // 부모 오브젝트에서 에너미 스크립트를 가져온다
                enemy = GetComponentInParent<F_Enemy>();

            // 에너미 스크립트를 찾지 못했을 경우
            if (enemy == null)
                Debug.Log("부모 오브젝트에서 F_Enemy를 찾지 못했습니다");
        }

        public void Signal()
        {
            // 에너미가 비어있지 않다면
            if (enemy != null)
                // 부모 오브젝트의 에너미 스크립트에서 히트 시그널을 호출한다
                enemy.hit_signal();

            // 에너미가 비어있다면
            else
                Debug.Log("부모 enemy가 없습니다");

        }
    }

}