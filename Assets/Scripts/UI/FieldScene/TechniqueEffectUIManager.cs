using UnityEngine;

public class TechniqueEffectUIManager : MonoBehaviour
{
    public GameObject MainUI;                // 메인 UI 오브젝트
    public GameObject TechniqueEffectWindow; // 비술 효과 창 오브젝트

    void Update()
    {
        // 창 닫기 단축키 (ESC)
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌렀을 때
            CloseTechniqueEffectWindow(); // 비술 효과 창 닫기
    }

    public void CloseTechniqueEffectWindow()
    {
        if (TechniqueEffectWindow.activeSelf) // 비술 효과 창이 활성화 상태일 때
        {
            TechniqueEffectWindow.SetActive(false); // 비술 효과 창 비활성화
            MainUIManager.Instance.CurrentWindow = MainUI; // 현재 창을 메인 UI로 설정
        }
    }
}
