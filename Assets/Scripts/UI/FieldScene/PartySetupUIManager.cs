using UnityEngine;

public class PartySetupUIManager : MonoBehaviour
{
    public GameObject MainUI;           // 메인 UI 오브젝트
    public GameObject PartySetupWindow; // 파티 편성 창 오브젝트

    void Update()
    {
        // 창 닫기 단축키 (ESC)
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌렀을 때
            ClosePartySetupWindow(); // 파티 편성 창 닫기
    }

    public void ClosePartySetupWindow()
    {
        if (PartySetupWindow.activeSelf) // 파티 편성 창이 활성화 상태일 때
        {
            PartySetupWindow.SetActive(false); // 파티 편성 창 비활성화
            MainUIManager.Instance.CurrentWindow = MainUI; // 현재 창을 메인 UI로 설정
        }
    }
}
