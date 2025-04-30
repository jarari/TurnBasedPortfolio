using UnityEngine;

namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// 필드의 에너미 스크립트
    /// </summary>
    public class F_Enemy : MonoBehaviour
    {

        #region 에너미의 상태

        // 에너미 상태
        public enum F_EnemyState { Idle,Move,Attack,Damage }
        // ↑로 만든 변수
        public F_EnemyState f_state;

        #endregion

        #region 에너미 추적 관련
        
        // 에너미 스피드
        public float speed = 4.0f;

        // 에너미 탐지 거리
        public float findDistnace = 4.0f;

        // 추적할 플레이어를 담을 변수
        public GameObject target;
                
        // 플레이어 공격가능 범위
        public float attDistance = 2.0f;

        #endregion

        #region 인스턴스를 생성할 친구들

        // 플레이어를 탐지할 클래스
        protected EnemyDetector detecter;

        #endregion

        private void OnEnable()
        {
            // 플레이어가 에너미와 가까워졌을때 이벤트를 구독
            PlayerController.OnPlayerNearEnemy += findPlayer;
        }

        private void OnDisable()
        {
            // 이벤트 구독 해제
            PlayerController.OnPlayerNearEnemy -= findPlayer;            
        }

        private void Start()
        {
            // 시작시 에너미의 상태를 기본으로 한다
            f_state = F_EnemyState.Idle;

            // 탐지기의 인스턴스를 생성한다
            detecter = new EnemyDetector();
        }

        private void Update()
        {
            //findPlayer();

            switch (f_state)
            {
                case F_EnemyState.Idle:
                    F_Idle();
                    break;
                case F_EnemyState.Move:
                    F_Move();
                    break;
                case F_EnemyState.Attack:
                    F_Attack();
                    break;                                
            }
        }


        public virtual void F_Idle()  { }
        public virtual void F_Move() { }
        public virtual void F_Attack() { }

        public virtual void findPlayer() 
        {
            // 태그가 플레이어인 오브젝트를 탐지 범위 내에 있는지 찾아낸다
            target = detecter.Detect(transform.position, findDistnace, "Player");

            // 있다면
            if (target != null)
            {
                Debug.Log("타겟 발견 : " + target.name);
            }
        
        }

    }

}