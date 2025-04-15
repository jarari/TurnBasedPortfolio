using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public GameObject MainUI;                   // 메인 UI 오브젝트
    public GameObject PhoneWindow;              // 휴대폰 창 오브젝트
    public GameObject CharacterWindow;          // 캐릭터 창 오브젝트
    public GameObject PartySetupWindow;         // 파티 편성 창 오브젝트
    public GameObject TechniqueEffectWindow;    // 비술 효과 창 오브젝트

    private GameObject CurrentWindow;           // 현재 열려 있는 창

    void Start()
    {
        // 시작 시 모든 창 닫기
        PhoneWindow.SetActive(false); // 휴대폰 창 비활성화
        CharacterWindow.SetActive(false); // 캐릭터 창 비활성화
        PartySetupWindow.SetActive(false); // 파티 편성 창 비활성화
        TechniqueEffectWindow.SetActive(false); // 비술 효과 창 비활성화

        CurrentWindow = MainUI; // 현재 창을 메인 UI로 설정
    }

    void Update()
    {
        if (CurrentWindow == MainUI && Input.GetKeyDown(KeyCode.Escape)) // 현재 창이 메인 UI이고 ESC 키를 눌렀을 때
            OpenWindow(PhoneWindow); // 휴대폰 창 열기

        if (Input.GetKeyDown(KeyCode.C)) // C 키를 눌렀을 때
            OpenWindow(CharacterWindow); // 캐릭터 창 열기

        if (Input.GetKeyDown(KeyCode.L)) // L 키를 눌렀을 때
            OpenWindow(PartySetupWindow); // 파티 편성 창 열기

        if (Input.GetKeyDown(KeyCode.U)) // U 키를 눌렀을 때
            OpenWindow(TechniqueEffectWindow); // 비술 효과 창 열기
    }

    public void OpenWindow(GameObject window)
    {
        if (!window.activeSelf) // 창이 비활성화 상태일 때
        {
            window.SetActive(true); // 창 활성화
            CurrentWindow = window; // 현재 창을 연 창으로 설정
        }
    }
}
