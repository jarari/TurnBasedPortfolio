using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;

public class PartySetupUIManager : MonoBehaviour
{
    public static PartySetupUIManager Instance; // �̱��� �ν��Ͻ�

    public GameObject MainUI;              // ���� UI ������Ʈ
    public GameObject PartySetupWindow;    // ��Ƽ �� â ������Ʈ
    public List<GameObject> CharacterSlots;// ĳ���� ���� ����Ʈ
    public GameObject CharacterListWindow; // ĳ���� ��� â ������Ʈ
    public GameObject ConfirmButton;       // Ȯ�� ��ư
    public List<GameObject> Character;     // ĳ���� ����Ʈ

    public List<Texture> CharacterRenderTexture; // ĳ���� ���� �ؽ�ó ����Ʈ
    public Texture CharacterSlotTexture; // ĳ���� ���� �ؽ�ó

    private void Awake()
    {
        if (Instance == null)
            Instance = this; // �ν��Ͻ� ����
        else
            Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
    }

    private void Start()
    {
        CharacterListWindow.SetActive(false); // ĳ���� ��� â ��Ȱ��ȭ

        // �⺻ ĳ���� �߰�
        PartyManager.Instance.AddCharacterToParty("C1");
        SelectCharacter("C1");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ClosePartySetupWindow();
        if (Input.GetKeyDown(KeyCode.Return)) ToggleCharacterListWindow();
        if (CharacterListWindow.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleCharacter("C1");
            if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleCharacter("C2");
            if (Input.GetKeyDown(KeyCode.Alpha3)) ToggleCharacter("C3");
            if (Input.GetKeyDown(KeyCode.Alpha4)) ToggleCharacter("C4");
        }
    }

    public void ClosePartySetupWindow()
    {
        if (PartySetupWindow.activeSelf) // ��Ƽ �� â�� Ȱ��ȭ ������ ��
        {
            PartySetupWindow.SetActive(false); // ��Ƽ �� â ��Ȱ��ȭ
            CharacterListWindow.SetActive(false); // ĳ���� ��� â ��Ȱ��ȭ
            MainUIManager.Instance.CurrentWindow = MainUI; // ���� â�� ���� UI�� ����
        }
    }

    public void ToggleCharacterListWindow()
    {
        CharacterListWindow.SetActive(!CharacterListWindow.activeSelf);
        ConfirmButton.SetActive(CharacterListWindow.activeSelf);
    }

    public void ToggleCharacter(string ID)
    {
        if (PartyManager.Instance.IsCharacterInParty(ID)) // ĳ���Ͱ� �̹� ��Ƽ�� ������
        {
            DeselectCharacter(ID); // ĳ���� ���� ����
            PartyManager.Instance.RemoveCharacterFromParty(ID); // ���Կ��� ĳ���� ����
        }
        else // ĳ���Ͱ� ��Ƽ�� ������
        {
            if (PartyManager.Instance.IsPartyFull()) return; // ��Ƽ�� ���� ���� �� �Ұ�
            PartyManager.Instance.AddCharacterToParty(ID); // ���Կ� ĳ���� �߰�
            SelectCharacter(ID); // ĳ���� ����
        }
    }

    private void SelectCharacter(string ID)
    {
        int partyNumber = PartyManager.Instance.GetPartyNumber(ID); // ��Ƽ�� ���� ĳ������ ��ȣ ��������
        int uNum = GetUniqueNumber(ID); // ĳ���� ���� ��ȣ ��������

        for (int i = 0; i < Character.Count; i++) // ĳ���� ����Ʈ�� ��ȸ
        {
            if (i == uNum) // ��ȣ�� ��ġ�ϴ� ĳ���� ������Ʈ�� ã����
            {
                Transform borderLine = Character[i].transform.Find("BorderLine"); // ĳ���� ������Ʈ�� �ڽ� ������Ʈ �߿��� �̸��� "BorderLine"�� ������Ʈ�� ã��
                if (borderLine != null)
                    borderLine.gameObject.SetActive(true); // �׵θ� Ȱ��ȭ

                Transform number = Character[i].transform.Find("Number"); // ĳ���� ������Ʈ�� �ڽ� ������Ʈ �߿��� �̸��� "Number"�� ������Ʈ�� ã��
                if (number != null)
                {
                    number.gameObject.SetActive(true); // ��ȣ Ȱ��ȭ
                    Text numberText = number.GetComponentInChildren<Text>(); // Number ������Ʈ�� �ڽ� ������Ʈ �߿��� �ؽ�Ʈ ������Ʈ�� ã��
                    if (numberText != null)
                    {
                        numberText.text = (partyNumber + 1).ToString(); // ĳ���� ���� ������ ����
                    }
                }
                AddCharacterToSlot(partyNumber, ID); // ���Կ� ĳ���� �߰�
            }
        }
    }

    private void DeselectCharacter(string ID)
    {
        int partyNumber = PartyManager.Instance.GetPartyNumber(ID); // ��Ƽ�� ���� ĳ������ ��ȣ ��������
        int uNum = GetUniqueNumber(ID); // ĳ���� ���� ��ȣ ��������

        for (int i = 0; i < Character.Count; i++) // ĳ���� ����Ʈ�� ��ȸ
        {
            if (i == uNum) // ��ȣ�� ��ġ�ϴ� ĳ���� ������Ʈ�� ã����
            {
                Transform borderLine = Character[i].transform.Find("BorderLine"); // ĳ���� ������Ʈ�� �ڽ� ������Ʈ �߿��� �̸��� "BorderLine"�� ������Ʈ�� ã��
                if (borderLine != null)
                    borderLine.gameObject.SetActive(false); // �׵θ� ��Ȱ��ȭ
                Transform number = Character[i].transform.Find("Number"); // ĳ���� ������Ʈ�� �ڽ� ������Ʈ �߿��� �̸��� "Number"�� ������Ʈ�� ã��
                if (number != null)
                {
                    number.gameObject.SetActive(false); // ��ȣ ��Ȱ��ȭ
                }
                RemoveCharacterFromSlot(partyNumber); // ���Կ��� ĳ���� ����
            }
        }
    }
    private int GetUniqueNumber(string ID)
    {
        int num;
        switch (ID)
        {
            case "C1":
                num = 0;
                break;
            case "C2":
                num = 1;
                break;
            case "C3":
                num = 2;
                break;
            case "C4":
                num = 3;
                break;
            default:
                num = -1;
                break;
        }
        return num; // ĳ���� ��ȣ ��ȯ
    }

    public void AddCharacterToSlot(int CharacterNumber, string ID)
    {
        int index = CharacterRenderTexture.FindIndex(texture => texture.name == ID); // ���� �ؽ�ó ����Ʈ���� ID�� ��ġ�ϴ� �ؽ�ó�� �ε����� ã��
        
        RawImage slotImage = CharacterSlots[CharacterNumber].GetComponent<RawImage>();
        if (slotImage != null)
        {
            slotImage.texture = CharacterRenderTexture[index]; // ���� �̹����� ĳ���� ���� �ؽ�ó�� ����
        }
    }



    public void RemoveCharacterFromSlot(int CharacterNumber)
    {
        RawImage slotImage = CharacterSlots[CharacterNumber].GetComponent<RawImage>();
        if (slotImage != null)
        {
            slotImage.texture = CharacterSlotTexture; // ���� �̹����� ���� �ؽ�ó�� ����
        }
    }
}
