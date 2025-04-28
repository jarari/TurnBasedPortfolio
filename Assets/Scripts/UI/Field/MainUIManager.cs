using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Instance;       // �ν��Ͻ�

    public GameObject MainUI;                   // ���� UI ������Ʈ
    public GameObject PhoneWindow;              // �޴��� â ������Ʈ
    public GameObject CharacterWindow;          // ĳ���� â ������Ʈ
    public GameObject PartySetupWindow;         // ��Ƽ �� â ������Ʈ
    public GameObject TechniqueEffectWindow;    // ��� ȿ�� â ������Ʈ
    public GameObject CurrentWindow;            // ���� ���� �ִ� â

    public List<GameObject> partyMember;        // ��Ƽ�� ������Ʈ ����Ʈ

    public GameObject InterActionButton;        // ��ȣ�ۿ� ��ư ������Ʈ

    public Button Attack;                       // ���� ��ư ������Ʈ
    public Button Technique;                    // ��� ��ư ������Ʈ
    public Button Run;                          // �޸��� ��ư ������Ʈ
    
    public AudioClip Select;                    // ���� ȿ����
    public AudioClip Confirm;                   // Ȯ�� ȿ����
    public AudioClip Cancel;                    // ��� ȿ����

    private AudioSource audioSource;            // ����� �ҽ�
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

        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource ������Ʈ �߰�
    }

    void Start()
    {
        // ��� â �ݱ�
        PhoneWindow.SetActive(false); // �޴��� â ��Ȱ��ȭ
        CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ
        PartySetupWindow.SetActive(false); // ��Ƽ �� â ��Ȱ��ȭ
        TechniqueEffectWindow.SetActive(false); // ��� ȿ�� â ��Ȱ��ȭ
        InterActionButton.SetActive(false); // ��ȣ�ۿ� ��ư ��Ȱ��ȭ

        CurrentWindow = MainUI; // ���� â�� ���� UI�� ����

        Cursor.visible = false; // ���콺 Ŀ�� �����

        Attack.GetComponent<Image>().enabled = false; // ���� ��ư ������Ʈ �̹��� ��Ȱ��ȭ
        Technique.GetComponent<Image>().enabled = false; // ��� ��ư ������Ʈ �̹��� ��Ȱ��ȭ
        Run.GetComponent<Image>().enabled = false; // �޸��� ��ư ������Ʈ �̹��� ��Ȱ��ȭ

    }

    void Update()
    {
        HandleCursor(); // ���콺 Ŀ�� ó��
        OnKeyInput(); // ����Ű �Է� ó��
        UpdatePartyUI(); // ��Ƽ UI ������Ʈ
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
        if (CurrentWindow == MainUI && !Cursor.visible) // ���� â�� ���� UI�̰� ���콺 Ŀ���� ������ ���� ��
        {
            // if (Input.GetKeyDown(KeyCode.Escape)) OpenWindow(PhoneWindow); // �޴��� â ����
            if (Input.GetKeyDown(KeyCode.C)) OpenWindow(CharacterWindow); // ĳ���� â ����
            if (Input.GetKeyDown(KeyCode.L)) OpenWindow(PartySetupWindow); // ��Ƽ �� â ����
            // if (Input.GetKeyDown(KeyCode.B)) OpenWindow(BackpackWindow); // ���� â ����
            if (Input.GetKeyDown(KeyCode.U)) OpenWindow(TechniqueEffectWindow); // ��� ȿ�� â ����
            if (Input.GetKeyDown(KeyCode.E)) TechniqueButtonFlash(); // ��� ��ư Ŭ�� �� ��� ��ư �̹��� ������ ȿ��
            if (Input.GetMouseButtonDown(0)) AttackButtonFlash(); // ���� ��ư Ŭ�� �� ���� ��ư �̹��� ������ ȿ��
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift)) ToggleRunButton(); // �޸��� ��ư ���
        }
    }

    public void TechniqueButtonFlash()
    {
        TechniquePointManager.Instance.UseTechnique(); // ��� ����Ʈ ���
        StartCoroutine(TechniqueButtonCoroutine()); // ��� ��ư �̹����� Ȱ��ȭ�ϰ� 0.1�� �� ��Ȱ��ȭ
    }

    private IEnumerator TechniqueButtonCoroutine()
    {
        Technique.GetComponent<Image>().enabled = true; // ��� ��ư �̹��� Ȱ��ȭ
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        Technique.GetComponent<Image>().enabled = false; // ��� ��ư �̹��� ��Ȱ��ȭ
    }

    public void AttackButtonFlash()
    {
        StartCoroutine(AttackButtonCoroutine()); // ���� ��ư �̹����� Ȱ��ȭ�ϰ� 0.1�� �� ��Ȱ��ȭ
    }

    private IEnumerator AttackButtonCoroutine()
    {
        Attack.GetComponent<Image>().enabled = true; // ���� ��ư �̹��� Ȱ��ȭ
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        Attack.GetComponent<Image>().enabled = false; // ���� ��ư �̹��� ��Ȱ��ȭ
    }

    public void ToggleRunButton()
    {
        if (!Run.GetComponent<Image>().enabled) // �޸��� ��ư ������Ʈ�� �̹����� ��Ȱ��ȭ ������ ��
            Run.GetComponent<Image>().enabled = true; // �޸��� ��ư ������Ʈ�� �̹����� Ȱ��ȭ
        else // �޸��� ��ư ������Ʈ�� �̹����� Ȱ��ȭ ������ ��
            Run.GetComponent<Image>().enabled = false; // �޸��� ��ư ������Ʈ�� �̹����� ��Ȱ��ȭ
    }
    public void OpenWindow(GameObject window)
    {
        if (!window.activeSelf) // â�� ��Ȱ��ȭ ������ ��
        {
            window.SetActive(true); // â Ȱ��ȭ
            CurrentWindow = window; // ���� â�� �� â���� ����
            audioSource.PlayOneShot(Select); // ���� ȿ���� ���
        }
    }

    public void UpdatePartyUI()
    {
        string[] party = PartyManager.Instance.GetParty();
        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] != null)
            {
                // ��Ƽ�� ������Ʈ�� Ȱ��ȭ
                partyMember[i].SetActive(true);
                
                Text characterNameText = partyMember[i].transform.Find("CharacterName").GetComponent<Text>(); // ��Ƽ�� ������Ʈ�� �ڽ� ������Ʈ �߿��� �̸��� "CharacterName"�� ������Ʈ�� ã��
                characterNameText.text = CharacterDataManager.GetCharacterName(party[i]); // ĳ���� �̸��� �����ͼ� �ؽ�Ʈ ����

                Image characterImage = partyMember[i].transform.Find("CharacterImage").GetComponent<Image>(); // ��Ƽ�� ������Ʈ�� �ڽ� ������Ʈ �߿��� �̸��� "CharacterImage"�� ������Ʈ�� ã��
                Sprite sprite = Resources.Load<Sprite>(CharacterDataManager.GetCharacterImagePath(party[i])); // ĳ���� �̹��� ��θ� �����ͼ� ��������Ʈ �ε�
                characterImage.sprite = sprite; // ĳ���� �̹��� ����

                Image ultimateImage = partyMember[i].transform.Find("Ultimate").GetComponent<Image>(); // ��Ƽ�� ������Ʈ�� �ڽ� ������Ʈ �߿��� �̸��� "Ultimate"�� ������Ʈ�� ã��
                Sprite ultimateSprite = Resources.Load<Sprite>(CharacterDataManager.GetCharacterImagePath(party[i])); // ĳ���� �̹��� ��θ� �����ͼ� ��������Ʈ �ε�
                // ultimateImage.sprite = ultimateSprite; // ĳ���� �̹��� ����
            }
            else
            {
                // ��Ƽ�� ������Ʈ�� ��Ȱ��ȭ
                partyMember[i].SetActive(false);
            }
        }
    }
}
