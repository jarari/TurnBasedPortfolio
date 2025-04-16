using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    public GameObject CharacterWindow; // 캐릭터 창 오브젝트

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌렀을 때
            CloseCharacterWindow(); // 캐릭터 창 닫기
    }

    public void CloseCharacterWindow()
    {
        if (CharacterWindow.activeSelf) // 캐릭터 창이 활성화 상태일 때
            CharacterWindow.SetActive(false); // 캐릭터 창 비활성화
    }
}
