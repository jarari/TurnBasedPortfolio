using System;
using TurnBased.Entities.Field;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 기본 이동 속도
    public float runSpeed = 10f; // 달리기 속도
    public Transform cameraTransform; // 카메라 Transform

    public GameObject[] characterPrefabs; // 캐릭터 프리팹 배열
    private GameObject currentCharacter; // 현재 활성화된 캐릭터

    private CharacterController characterController; // 캐릭터 컨트롤러
    private Animator animator; // Animator 컴포넌트
    private bool isRunning = false; // 달리기 상태 여부

    #region Enemy와의 이벤트가 발생하는지 확인하기 위해 추가한것 -by 정준

    // 에너미와 가까워졌을 때 발생하는 이벤트
    public static event Action OnPlayerNearEnemy;

    // 에너미의 transform
    public GameObject enemy;
    // 플레이어의 탐지 범위
    public float playerRange = 5f;

    // 에너미를 탐지해줄 고마운 친구
    protected EnemyDetector detector;

    #endregion

    void Start()
    {
        // 컴포넌트 가져오기
        characterController = GetComponent<CharacterController>(); // CharacterController
        animator = GetComponent<Animator>(); // Animator

        // ChangeCharacter(0); // 첫 번째 캐릭터로 초기화

        #region  Enemy와의 이벤트가 발생하는지 확인하기 위해 추가한것 -by 정준

        // 탐지기의 인스턴스를 생성한다
        detector = new EnemyDetector();

        #endregion

    }

    void Update()
    {
        KeyboardInput(); // 키보드 입력 처리

        // 달리기 상태 토글
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) // 쉬프트 키 또는 마우스 우클릭
        {
            isRunning = !isRunning;
        }

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

        Vector3 moveDirection = (forward * vertical + right * horizontal) * currentSpeed;

        // 이동 적용
        characterController.Move(moveDirection * Time.deltaTime);

        // 이동 방향이 0이 아닐 때만 회전
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // 부드럽게 회전
        }

        // 애니메이션 처리
        if (animator != null)
        {
            if (moveDirection != Vector3.zero)
            {
                animator.SetBool("IsMoving", true); // 이동 중
            }
            else
            {
                animator.SetBool("IsMoving", false); // 이동 중지
            }

            // 달리기 상태 설정
            animator.SetBool("IsRunning", isRunning);
        }

        #region  Enemy와의 이벤트가 발생하는지 확인하기 위해 추가한것 -by 정준

        // 태그가 에너미인 오브젝트가 범위내에 있는지 찾아낸다
        enemy = detector.Detect(transform.position, playerRange, "Enemy");

        // 플레이어와 에너미 사이의 거리를 계산
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

        // 거리가 탐지 범위 안에 있을경우
        if (distanceToEnemy < playerRange)
        {
            // 이벤트를 발생 시킨다
            OnPlayerNearEnemy?.Invoke();
        }

        #endregion

    }

    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCharacter(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCharacter(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeCharacter(2);
    }

    private void ChangeCharacter(int index)
    {
        if (index < 0 || index >= characterPrefabs.Length) return;

        // 기존 캐릭터 제거
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
        }

        // 새로운 캐릭터 생성
        currentCharacter = Instantiate(characterPrefabs[index], transform.position, transform.rotation);

        // 새로운 캐릭터의 Animator 설정
        animator = currentCharacter.GetComponent<Animator>();

        // 현재 오브젝트의 Transform을 새로운 캐릭터에 동기화
        currentCharacter.transform.parent = transform;

        // 새 캐릭터의 CamPos를 찾아 카메라 타겟으로 설정
        Transform camPos = currentCharacter.transform.GetChild(0); // 첫 번째 자식 가져오기
        Debug.Log("CamPos: " + camPos);
        MainCameraController.Instance.SetTarget(camPos);
    }
}
