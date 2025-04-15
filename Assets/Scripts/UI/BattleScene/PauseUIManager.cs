using UnityEngine;
using System.Collections;

public class PauseUIManager : MonoBehaviour
{
    public GameObject CombatUI;     // 전투 UI 오브젝트
    public GameObject PauseWindow; // 일시 정지 창 오브젝트

    void Update()
    {
        if (CombatUIManager.Instance.CurrentWindow == PauseWindow && Input.GetKeyDown(KeyCode.Escape)) // 현재 창이 일시 정지 창이고 ESC 키를 눌렀을 때
            StartCoroutine(ClosePauseWindowCoroutine()); // 일시 정지 창 닫기
    }

    public IEnumerator ClosePauseWindowCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        if (PauseWindow.activeSelf) // 일시 정지 창이 활성화 상태일 때
        {
            PauseWindow.SetActive(false); // 일시 정지 창 비활성화
            CombatUIManager.Instance.CurrentWindow = CombatUI; // 현재 창을 메인 UI로 설정
            CombatUI.SetActive(true); // 전투 UI 활성화
        }
    }

    public void ClosePauseWindow()
    {
        StartCoroutine(ClosePauseWindowCoroutine());
    }
}
