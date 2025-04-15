using UnityEngine;

public class CombatUIManager : MonoBehaviour
{
    // 인스턴스
    public static CombatUIManager Instance;
    
    public GameObject CombatUI;           // 전투 UI 오브젝트
    public GameObject PauseWindow;        // 일시 정지 창 오브젝트
    public GameObject CharacterWindow;    // 캐릭터 창 오브젝트
    public GameObject AllyCharacterList;  // 아군 캐릭터 목록 오브젝트
    public GameObject EnemyCharacterList; // 적 캐릭터 목록 오브젝트
    public GameObject BasicAttackUI;      // 일반 공격 UI 오브젝트
    public GameObject BasicAttackUIBorder;// 일반 공격 UI 테두리 오브젝트
    public GameObject SkillUI;            // 전투 스킬 UI 오브젝트
    public GameObject SkillUIBorder;      // 전투 스킬 UI 테두리 오브젝트
    public GameObject UltimateUI;         // 필살기 UI 오브젝트

    public GameObject CurrentWindow;     // 현재 열려 있는 창

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 인스턴스 설정
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스 삭제
        }
    }

    void Start()
    {
        // 시작 시 모든 창 닫기
        PauseWindow.SetActive(false); // 휴대폰 창 비활성화
        CharacterWindow.SetActive(false); // 캐릭터 창 비활성화

        CurrentWindow = CombatUI; // 현재 창을 메인 UI로 설정
    }

    void Update()
    {
        if (CurrentWindow == CombatUI) // 현재 창이 전투 UI일 때
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌렀을 때
                OpenPauseWindow(); // 일시 정지 창 열기
            if (Input.GetKeyDown(KeyCode.C)) // C 키를 눌렀을 때
                OpenAllyCharacterWindow(); // 아군 캐릭터 창 열기
            if (Input.GetKeyDown(KeyCode.Z)) // Z 키를 눌렀을 때
                OpenEnemyCharacterWindow(); // 적 캐릭터 창 열기
            if (Input.GetKeyDown(KeyCode.Q)) // Q 키를 눌렀을 때
                SelectObject(BasicAttackUI); // 일반 공격 UI 선택
            if (Input.GetKeyDown(KeyCode.E)) // E 키를 눌렀을 때
                SelectObject(SkillUI); // 전투 스킬 UI 선택
        }
    }

    public void OpenWindow(GameObject window, bool isAlly = true)
    {
        if (!window.activeSelf) // 창이 비활성화 상태일 때
        {
            if (window == CharacterWindow) // 캐릭터 창일 때
            {
                if (isAlly) // 아군 캐릭터 창을 열 때
                {
                    AllyCharacterList.SetActive(true); // 아군 캐릭터 리스트 활성화
                    EnemyCharacterList.SetActive(false); // 적 캐릭터 리스트 비활성화
                }
                else // 적 캐릭터 창을 열 때
                {
                    AllyCharacterList.SetActive(false); // 아군 캐릭터 리스트 비활성화
                    EnemyCharacterList.SetActive(true); // 적 캐릭터 리스트 활성화
                }
            }
            CurrentWindow.SetActive(false); // 현재 창 비활성화
            window.SetActive(true); // 창 활성화
            CurrentWindow = window; // 현재 창을 연 창으로 설정
        }
    }

    public void OpenPauseWindow()
    {
        OpenWindow(PauseWindow);
    }

    public void OpenAllyCharacterWindow()
    {
        OpenWindow(CharacterWindow, true);
    }

    public void OpenEnemyCharacterWindow()
    {
        OpenWindow(CharacterWindow, false);
    }

    public void SelectObject(GameObject AttackObject)
    {
        if (AttackObject == BasicAttackUI) // 일반 공격 UI일 때
        {
            UltimateUI.SetActive(false); // 필살기 UI 비활성화
            BasicAttackUIBorder.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.78f, 0f); // 일반 공격 UI 테두리 색상 변경
            SkillUIBorder.GetComponent<UnityEngine.UI.Image>().color = Color.white; // 전투 스킬 UI 테두리 색상 변경
        }
        else if (AttackObject == SkillUI) // 전투 스킬 UI일 때
        {
            UltimateUI.SetActive(false); // 필살기 UI 비활성화
            BasicAttackUIBorder.GetComponent<UnityEngine.UI.Image>().color = Color.white; // 일반 공격 UI 테두리 색상 변경
            SkillUIBorder.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.78f, 0f); // 전투 스킬 UI 테두리 색상 변경
        }
        else if (AttackObject == UltimateUI) // 필살기 UI일 때
        {
            UltimateUI.SetActive(true); // 필살기 UI 활성화
            BasicAttackUI.SetActive(false); // 일반 공격 UI 비활성화
            SkillUI.SetActive(false); // 전투 스킬 UI 비활성화
        }
    }
}
