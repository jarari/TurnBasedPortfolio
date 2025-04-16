using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Instance;       // �ν��Ͻ�

    public GameObject MainUI;                   // ���� UI ������Ʈ
    public GameObject PhoneWindow;              // �޴��� â ������Ʈ
    public GameObject CharacterWindow;          // ĳ���� â ������Ʈ
    public GameObject PartySetupWindow;         // ��Ƽ �� â ������Ʈ
    public GameObject TechniqueEffectWindow;    // ��� ȿ�� â ������Ʈ
    public GameObject CurrentWindow;            // ���� ���� �ִ� â

    public Button Attack;                       // ���� ��ư ������Ʈ
    public Button Technique;                    // ��� ��ư ������Ʈ
    public Button Run;                          // �޸��� ��ư ������Ʈ

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // �ν��Ͻ� ����
            transform.SetParent(null); // ��Ʈ GameObject�� ����
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else // �ν��Ͻ��� �̹� ������ ��
        {
            Destroy(gameObject); // ���� ������Ʈ �ı�
        }
    }

    void Start()
    {
        // ��� â �ݱ�
        PhoneWindow.SetActive(false); // �޴��� â ��Ȱ��ȭ
        CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ
        PartySetupWindow.SetActive(false); // ��Ƽ �� â ��Ȱ��ȭ
        TechniqueEffectWindow.SetActive(false); // ��� ȿ�� â ��Ȱ��ȭ

        CurrentWindow = MainUI; // ���� â�� ���� UI�� ����

        Cursor.visible = false; // ���콺 Ŀ�� �����

        // ��ư �̹��� ��Ȱ��ȭ
        Attack.GetComponent<Image>().enabled = false; // ���� ��ư ������Ʈ �̹��� ��Ȱ��ȭ
        Technique.GetComponent<Image>().enabled = false; // ��� ��ư ������Ʈ �̹��� ��Ȱ��ȭ
        Run.GetComponent<Image>().enabled = false; // �޸��� ��ư ������Ʈ �̹��� ��Ȱ��ȭ
    }

    void Update()
    {
        HandleCursor(); // ���콺 Ŀ�� ó��
        OnKeyInput(); // ����Ű �Է� ó��
    }

    private void HandleCursor()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || CurrentWindow != MainUI) // LeftAlt Ű�� Ȧ���ϰ� �ְų� ���� �����ִ� â�� ���� UI�� �ƴ� ��
        {
            Cursor.visible = true; // ���콺 Ŀ�� ���̱�
        }
        else // �� ���� ���
        {
            Cursor.visible = false; // ���콺 Ŀ�� �����
        }
    }

    public void OnKeyInput()
    {
        if (CurrentWindow == MainUI) // ���� â�� ���� UI�� ��
        {
            // if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
                // OpenWindow(PhoneWindow); // �޴��� â ����
            if (Input.GetKeyDown(KeyCode.C)) // C Ű�� ������ ��
                OpenWindow(CharacterWindow); // ĳ���� â ����
            if (Input.GetKeyDown(KeyCode.L)) // L Ű�� ������ ��
                OpenWindow(PartySetupWindow); // ��Ƽ �� â ����
            // if (Input.GetKeyDown(KeyCode.B)) // B Ű�� ������ ��
                // OpenWindow(BackpackWindow); // ���� â ����
            if (Input.GetKeyDown(KeyCode.U)) // U Ű�� ������ ��
                OpenWindow(TechniqueEffectWindow); // ��� ȿ�� â ����
            if (Input.GetKeyDown(KeyCode.E)) // E Ű�� ������ ��
                StartCoroutine(TechniqueButtonFlash()); // ��� ��ư �̹����� Ȱ��ȭ�ϰ� 0.1�� �� ��Ȱ��ȭ
        }

        if (CurrentWindow == MainUI && !Cursor.visible) // ���� â�� ���� UI�̰� ���콺 Ŀ���� ������ ���� ��
        {
            if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ��
                StartCoroutine(AttackButtonFlash()); // ���� ��ư �̹����� Ȱ��ȭ�ϰ� 0.1�� �� ��Ȱ��ȭ
            if (Input.GetMouseButtonDown(1)) // ���콺 ������ ��ư Ŭ�� ��
            {
                if (!Run.GetComponent<Image>().enabled) // �޸��� ��ư ������Ʈ�� �̹����� ��Ȱ��ȭ ������ ��
                    Run.GetComponent<Image>().enabled = true; // �޸��� ��ư ������Ʈ�� �̹����� Ȱ��ȭ
                else // �޸��� ��ư ������Ʈ�� �̹����� Ȱ��ȭ ������ ��
                    Run.GetComponent<Image>().enabled = false; // �޸��� ��ư ������Ʈ�� �̹����� ��Ȱ��ȭ
            }
        }
    }

    private IEnumerator AttackButtonFlash()
    {
        Attack.GetComponent<Image>().enabled = true; // ���� ��ư �̹��� Ȱ��ȭ
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        Attack.GetComponent<Image>().enabled = false; // ���� ��ư �̹��� ��Ȱ��ȭ
    }

    private IEnumerator TechniqueButtonFlash()
    {
        Technique.GetComponent<Image>().enabled = true; // ��� ��ư �̹��� Ȱ��ȭ
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        Technique.GetComponent<Image>().enabled = false; // ��� ��ư �̹��� ��Ȱ��ȭ
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
