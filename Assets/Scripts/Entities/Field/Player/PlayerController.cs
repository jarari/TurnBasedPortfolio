using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance; // 싱글톤 인스턴스

    public float moveSpeed = 5.0f; // 기본 이동 속도
    public float runSpeed = 10.0f; // 달리기 속도
    public Transform cameraTransform; // 카메라 Transform
    public float detectRange = 10.0f; // 탐지 범뤼

    private CharacterController characterController; // 캐릭터 컨트롤러
    private Animator animator; // Animator 컴포넌트
    private bool isRunning = false; // 달리기 상태 여부

    public GameObject[] characterPrefabs; // 캐릭터 프리팹 배열
    public GameObject[] sortedPrefabs; // 파티원 이름 순서에 맞게 정렬된 프리팹 배열
    private GameObject currentCharacter; // 현재 활성화된 캐릭터
    private int currentCharacterIndex = 0; // 현재 활성화된 캐릭터 인덱스

    public static event Action<GameObject> OnPlayerNearEnemy; // 적 탐지 이벤트

    private string[] party; // 파티 정보
    private string[] partyMemberName; // 파티원 이름 정보

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 싱글톤 인스턴스 설정
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스 삭제
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>(); // CharacterController 컴포넌트 가져오기
        party = PartyManager.Instance.GetParty(); // 파티 정보 가져오기
        SetPartyMemberName(); // 파티원 이름 설정
        ChangeCharacter(currentCharacterIndex); // 초기 캐릭터 생성
    }

    void Update()
    {
        DetectEnemies(); // 적 탐지

        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCharacter(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCharacter(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeCharacter(2);

        // 달리기 상태 토글
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) // 쉬프트 키 또는 마우스 우클릭
            isRunning = !isRunning;

        // 공격
        if (Input.GetMouseButtonDown(0)) // 마우스 좌클릭
        {
            // animator.SetTrigger("Attack"); // 공격 애니메이션 트리거
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

        // 애니메이션 처리
        if (animator != null)
        {
            if (moveDirection != Vector3.zero)
            {
                if (isRunning)
                    animator.SetFloat("MoveMotion", 1); // 달리기
                else if (!isRunning)
                    animator.SetFloat("MoveMotion", 0.95f); // 걷기
            }
            else
                animator.SetFloat("MoveMotion", 0); // 대기
        }
    }

    private void ChangeCharacter(int newIndex)
    {
        if (sortedPrefabs[newIndex] == null) // null이면
        {
            // Debug.Log("파티에 " + (newIndex + 1) + "번 캐릭터가 없습니다");
            return; 
        }
        if (currentCharacter != null) Destroy(currentCharacter); // 이미 활성화된 캐릭터가 있으면 파괴
        currentCharacter = Instantiate(sortedPrefabs[newIndex], transform.position, Quaternion.identity, transform); // 새 캐릭터 생성
        currentCharacterIndex = newIndex; // 현재 캐릭터 인덱스 업데이트
        currentCharacter.transform.localRotation = Quaternion.Euler(0, 0, 0); // 로테이션 초기화
        animator = currentCharacter.GetComponent<Animator>(); // Animator 컴포넌트 가져오기

        Transform cameraPivot = currentCharacter.transform.Find("CameraPivot"); // 캐릭터 프리팹의 자식 오브젝트에서 "CameraPivot" 찾기
        if (cameraPivot != null) MainCameraController.Instance.SetTarget(cameraPivot); // 카메라 타겟을 CameraPivot으로 설정
    }

    private void DetectEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRange); // 탐지 범위 내의 콜라이더 가져오기

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy")) // 콜라이더 태그가 "Enemy"인 경우
            {
                OnPlayerNearEnemy(this.gameObject); // 이벤트 호출
                // Debug.Log("적 감지: " + hitCollider.name);
            }
        }
    }

    public string[] SetPartyMemberName()
    {
        partyMemberName = new string[party.Length]; // 파티원 이름 배열 초기화
        foreach (var character in party) // 파티 배열을 순회
        {
            if (character != null) // 빈 슬롯이 아니면
            partyMemberName[Array.IndexOf(party, character)] = CharacterDataManager.GetCharacterName(character); // 해당 인덱스에 이름 저장
        }
        SortCharacterPrefabsByPartyMemberName(); // 파티원 이름 순서에 맞게 프리팹 정렬
        return partyMemberName; // 파티원 이름 배열 반환
    }
    private void SortCharacterPrefabsByPartyMemberName()
    {
        sortedPrefabs = new GameObject[partyMemberName.Length];// 새로운 정렬된 프리팹 배열 생성

        foreach (var name in partyMemberName)
        {
            if (string.IsNullOrEmpty(name)) continue; // 빈 슬롯은 무시

            GameObject prefab = Array.Find(characterPrefabs, p => p != null && p.name == name); // 이름에 해당하는 프리팹 찾기

            if (prefab != null)
            {
                int index = Array.IndexOf(partyMemberName, name); // 파티원 이름 배열에서의 인덱스
                sortedPrefabs[index] = prefab; // 해당 인덱스에 프리팹 배치
            }
        }
    }
}
