using UnityEngine;

public class PartySetupUIManager : MonoBehaviour
{
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
            PartySetupWindow.SetActive(false); // 파티 편성 창 비활성화
    }
}
