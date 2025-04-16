using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    public GameObject MainUI;          // ���� UI ������Ʈ
    public GameObject CharacterWindow; // ĳ���� â ������Ʈ

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
            CloseCharacterWindow(); // ĳ���� â �ݱ�
    }

    public void CloseCharacterWindow()
    {
        if (CharacterWindow.activeSelf) // ĳ���� â�� Ȱ��ȭ ������ ��
        {
            CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ
            MainUIManager.Instance.CurrentWindow = MainUI; // ���� â�� ���� UI�� ����
        }
    }
}
