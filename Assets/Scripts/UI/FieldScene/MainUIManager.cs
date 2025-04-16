using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public GameObject MainUI;                   // ���� UI ������Ʈ
    public GameObject PhoneWindow;              // �޴��� â ������Ʈ
    public GameObject CharacterWindow;          // ĳ���� â ������Ʈ
    public GameObject PartySetupWindow;         // ��Ƽ �� â ������Ʈ
    public GameObject TechniqueEffectWindow;    // ��� ȿ�� â ������Ʈ

    private GameObject CurrentWindow;           // ���� ���� �ִ� â

    void Start()
    {
        // ���� �� ��� â �ݱ�
        PhoneWindow.SetActive(false); // �޴��� â ��Ȱ��ȭ
        CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ
        PartySetupWindow.SetActive(false); // ��Ƽ �� â ��Ȱ��ȭ
        TechniqueEffectWindow.SetActive(false); // ��� ȿ�� â ��Ȱ��ȭ

        CurrentWindow = MainUI; // ���� â�� ���� UI�� ����
    }

    void Update()
    {
        if (CurrentWindow == MainUI && Input.GetKeyDown(KeyCode.Escape)) // ���� â�� ���� UI�̰� ESC Ű�� ������ ��
            OpenWindow(PhoneWindow); // �޴��� â ����

        if (Input.GetKeyDown(KeyCode.C)) // C Ű�� ������ ��
            OpenWindow(CharacterWindow); // ĳ���� â ����

        if (Input.GetKeyDown(KeyCode.L)) // L Ű�� ������ ��
            OpenWindow(PartySetupWindow); // ��Ƽ �� â ����

        if (Input.GetKeyDown(KeyCode.U)) // U Ű�� ������ ��
            OpenWindow(TechniqueEffectWindow); // ��� ȿ�� â ����
    }

    public void OpenWindow(GameObject window)
    {
        if (!window.activeSelf) // â�� ��Ȱ��ȭ ������ ��
        {
            window.SetActive(true); // â Ȱ��ȭ
            CurrentWindow = window; // ���� â�� �� â���� ����
        }
    }
}
