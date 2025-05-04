using UnityEngine;
using System;
using NUnit.Framework.Internal.Commands;
using System.Collections;

namespace TurnBased.Entities.Field
{ 

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5.0f; // 기본 이동 속도
        public float runSpeed = 10.0f; // 달리기 속도
        public Transform cameraTransform; // 카메라 Transform
        public float detectRange = 10.0f; // 탐지 범위
    
        private CharacterController characterController; // 캐릭터 컨트롤러
        private Animator animator; // Animator 컴포넌트
        private bool isRunning = false; // 달리기 상태 여부
    
        public GameObject[] characterPrefabs; // 캐릭터 프리팹 배열
        private GameObject currentCharacter; // 현재 활성화된 캐릭터
        private int currentCharacterIndex = 0; // 현재 활성화된 캐릭터 인덱스

        #region 플레이어 공격 관련 변수들

        // 플레이어의 공격을 담당할 스크립트를 담을 변수
        protected PlayerAttack playerAttack;

        // 탐지할 에너미
        public GameObject enemy;
        // 공격 중복 방지 값
        public bool wait_att = false;
    
        public static event Action<GameObject> OnPlayerNearEnemy; // 적 탐지 이벤트

        #endregion

        void Start()
        {
            // 캐릭터 컨트롤러 컴포넌트 가져오기
            characterController = GetComponent<CharacterController>();
    
            // 초기 캐릭터 생성
            ChangeCharacter(currentCharacterIndex);

            // 플레이어 공격을 담당할 스크립트를 생성한다
            playerAttack = new PlayerAttack();
        }
    
        void Update()
        {
            DetectEnemies(); // 적 탐지
    
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCharacter(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCharacter(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeCharacter(2);
    
            #region 플레이어 이동 관련

        // 달리기 상태 토글
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) // 쉬프트 키 또는 마우스 우클릭
            isRunning = !isRunning;

        // 현재 속도 설정
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        // 입력 값 가져오기
        float horizontal = Input.GetAxis("Horizontal"); // A, D 키 (왼쪽, 오른쪽)
        float vertical = Input.GetAxis("Vertical");     // W, S 키 (앞, 뒤)

        // 카메라 기준 이동 방향 계산
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // y축 방향 제거 (수평 이동만 고려)
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * vertical + right * horizontal) * currentSpeed; // 이동 방향 계산

        // y축 값을 0으로 고정
        moveDirection.y = 0;

        characterController.Move(moveDirection * Time.deltaTime); // 이동 적용

        // 이동 방향이 0이 아닐 때만 회전
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // 부드럽게 회전
        }

            #endregion

            #region 플레이어 공격

            if (Input.GetMouseButtonDown(0))
            {
                // 에너미를 공격할 함수를 실행한다
                playerAttack.Attack(enemy, animator);                
            }

            #endregion
    
            #region 애니메이션

            // 애니메이션 처리
            if (animator != null)
            {
                Vector3 anim_dir = new Vector3(horizontal, 0, vertical);
                anim_dir = anim_dir.normalized;

                if (moveDirection != Vector3.zero)
                {
                    if (isRunning)
                        animator.SetFloat("MoveMotion", anim_dir.magnitude); // 달리기
                    else if (!isRunning)
                    {
                        // 만약 MoveMotion의 값이 0.7f를 넘긴다면
                        if (anim_dir.magnitude > 0.7f)
                        {
                            animator.SetFloat("MoveMotion", 0.7f);  // 수치를 고정시킨다
                        }
                        // 0.7f를 넘기지 않는다면
                        else
                        {
                            animator.SetFloat("MoveMotion", anim_dir.magnitude);   // 걷기
                        }
                    }
                }
                else
                    animator.SetFloat("MoveMotion", 0); // 대기
            }

        else
            Debug.Log("애니메이터가 없습니다");        
         
        #endregion
    }
                
        /// <summary>
        /// 프리펩에 있는 플레이어 인스턴스시켜 자식오브젝트로 만듦
        /// </summary>
        /// <param name="newIndex"></param>
        private void ChangeCharacter(int newIndex)
        {
            if (currentCharacter != null) Destroy(currentCharacter); // 이미 활성화된 캐릭터가 있으면 파괴
            currentCharacter = Instantiate(characterPrefabs[newIndex], transform.position, Quaternion.identity, transform); // 새 캐릭터 생성
            currentCharacterIndex = newIndex; // 현재 캐릭터 인덱스 업데이트
            currentCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0); // 로테이션 초기화
            animator = currentCharacter.GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        }
        
        /// <summary>
        /// 에너미 탐지 함수
        /// </summary>
        private void DetectEnemies()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRange); // 탐지 범위 내의 콜라이더 가져오기
    
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy")) // 콜라이더 태그가 "Enemy"인 경우
                {
                    // 에너미 오브젝트를 가져온다
                    enemy = hitCollider.gameObject;
                    OnPlayerNearEnemy(this.gameObject); // 이벤트 호출
                    Debug.Log("적 감지: " + hitCollider.name);
                }
            }
        }


    }

}