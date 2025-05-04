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

        Animator anim;

        #endregion

        #region 에너미 추적 관련
        
        // 에너미 탐지 거리
        public float findDistnace = 6.0f;

        // 추적할 플레이어를 담을 변수
        public GameObject target;
                
        // 플레이어 공격가능 범위
        public float attDistance = 1.0f;

        // 에너미의 캐릭터 컨트롤러를 담을 변수
        CharacterController cc;

        // 이벤트 중복 수신방지 변수
        bool prevention = false;

        #endregion

        #region 인스턴스를 생성할 친구들 (클래스)

        // 플레이어를 탐지할 클래스
        protected EnemyDetector detecter;
        protected EnemyMove Move;
        protected EnemyAttack attack;
        protected EnemySignal signal;

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

            // 캐릭터 컨트롤러를 가져온다
            cc = this.GetComponent<CharacterController>();

            // 자식오브젝트의 에니메이터를 가져온다
            anim = GetComponentInChildren<Animator>();

            // 각 클래스들의 인스턴스를 생성한다
            detecter = new EnemyDetector();
            Move = new EnemyMove();
            attack = new EnemyAttack();
            signal = new EnemySignal();
        }

        private void Update()
        {
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


        public void F_Idle()  
        {
        }

        public void F_Move() 
        {
            if (target == null)
                return;

            // 에너미를 플레이어를 향해 움직인다            
            cc.Move(Move.FE_MoveVector(target, this.gameObject, true));

            // 에너미가 플레이어를 바라보게 한다
            this.transform.forward = Move.FE_MoveVector(target, this.gameObject, false);

            // 플레이어와의 공격 가능한 거리를 계산하는 불값을 켜고
            bool A_switch = Move.FE_SwitchMove(target.transform.position, this.gameObject, attDistance);
            
            // 플레이어가 공격 가능한 범위 근처에 온다면
            if (A_switch == true)
            {
                // 상태를 공격으로 바꾼다
                f_state = F_EnemyState.Attack;
                Debug.Log("상태 갱신 : Move -> Attack");
                // 애니메이션 트리거를 켠다
                anim.SetTrigger("ToAttack");
            }

        }
        public void F_Attack()
        {
            // 플레이어와의 거리를 계산하는 불값을 켜고
            bool A_switch = Move.FE_SwitchMove(target.transform.position, this.gameObject, attDistance);

            // 플레이어가 공격 범위에서 멀어졌다면
            if (A_switch == false)
            {
                // 에너미의 상태를 이동으로 바꾼다
                f_state = F_EnemyState.Move;
                Debug.Log("상태 갱신 : Attack -> Move");
                // 애니메이션 트리거를 켠다
                anim.SetTrigger("ToMove");
            }

            bool battle = hit_signal(A_switch);

            // 만약 에너미의 공격이 플레이어에게 히트 했을 경우
            if (battle == true)
            {
                // 전투 씬으로 전환할 함수를 실행한다
                attack.ChangeScene();
            }
            // 히트 하지 못했을 경우
            else
                return;
        }

        public virtual void findPlayer(GameObject player) 
        {
            target = player; // 플레이어를 타겟으로 설정

            if (target != null) // 타겟이 null이 아닐때
            {
                if (Vector3.Distance(transform.position, target.transform.position) <= findDistnace) // 타겟과의 거리가 탐지거리보다 작거나 같을때
                {
                    // 타겟이 있는 상태에서 중복 방지 불값이 false라면
                    if (prevention == false)
                    { 
                        // 현재 상태를 무브로 바꾼다
                        f_state = F_EnemyState.Move;
                        // 애니메이션의 트리거를 켠다
                        anim.SetTrigger("ToMove");

                        // 중복 방지 를 켠다
                        prevention = true;
                    }
                }
                else if (Vector3.Distance(transform.position, target.transform.position) > findDistnace) // 타겟과의 거리가 탐지 거리보다 멀때
                {
                    if (prevention == true)
                    { 
                        // 에너미상태를 전환 한다
                        f_state = F_EnemyState.Idle;
                        // 애니메이션의 트리거를 켠다
                        anim.SetTrigger("ToIdle");
                    
                        // 중복 방지 를 켠다
                        prevention = false;
                    }
                }

            }
            
        
        }

        // 공격 애니메이션 진행시 시그널을 받을 함수
        public virtual bool hit_signal(bool a) { return false;}

    }

}