using UnityEngine;

public class PhoneUIManager : MonoBehaviour
{
    public GameObject PhoneWindow; // 휴대폰 창 오브젝트

    void Update()
    {
        // 창 닫기 단축키 (ESC)
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌렀을 때
            ClosePhoneWindow(); // 휴대폰 창 닫기
    }

    public void ClosePhoneWindow()
    {
        if (PhoneWindow.activeSelf) // 휴대폰 창이 활성화 상태일 때
            PhoneWindow.SetActive(false); // 휴대폰 창 비활성화
    }
}