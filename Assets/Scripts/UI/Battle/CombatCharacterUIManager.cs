using UnityEngine;
using System.Collections;

public class CombatCharacterUIManager : MonoBehaviour
{
    public GameObject CombatUI;           // 전투 UI 오브젝트
    public GameObject CharacterWindow;    // 캐릭터 창 오브젝트
    public GameObject AllyCharacterList;  // 아군 캐릭터 목록 오브젝트
    public GameObject EnemyCharacterList; // 적 캐릭터 목록 오브젝트

    void Update()
    {
        if (CombatUIManager.Instance.CurrentWindow == CharacterWindow) // 현재 창이 캐릭터 창일 때
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌렀을 때
                StartCoroutine(CloseCharacterWindowCoroutine()); // 캐릭터 창 닫기
            if (Input.GetKeyDown(KeyCode.Tab)) // Tab 키를 눌렀을 때
                ToggleAllyEnemySwitch(); // 아군/적 캐릭터 리스트 전환
        }
    }

    public IEnumerator CloseCharacterWindowCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        if (CharacterWindow.activeSelf) // 캐릭터 창이 활성화 상태일 때
        {
            CharacterWindow.SetActive(false); // 캐릭터 창 비활성화
            CombatUIManager.Instance.CurrentWindow = CombatUI; // 현재 창을 메인 UI로 설정
            CombatUI.SetActive(true); // 전투 UI 활성화
        }
    }

    public void CloseCharacterWindow()
    {
        StartCoroutine(CloseCharacterWindowCoroutine());
    }

    public void ToggleAllyEnemySwitch()
    {
        if (CharacterWindow.activeSelf) // 캐릭터 창이 활성화 상태일 때
        {
            if (AllyCharacterList.activeSelf) // 아군 캐릭터 리스트가 활성화 상태일 때
            {
                AllyCharacterList.SetActive(false); // 아군 캐릭터 리스트 비활성화
                EnemyCharacterList.SetActive(true); // 적 캐릭터 리스트 활성화
            }
            else // 아군 캐릭터 리스트가 비활성화 상태일 때
            {
                AllyCharacterList.SetActive(true); // 아군 캐릭터 리스트 활성화
                EnemyCharacterList.SetActive(false); // 적 캐릭터 리스트 비활성화
            }
        }
    }
}
