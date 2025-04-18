using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    private int[] partyCharacterNumbers = new int[3]; // ��Ƽ�� ���� ĳ������ ��ȣ�� ������ �迭

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // �ν��Ͻ� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
            ClosePartySetupWindow(); // ��Ƽ �� â �ݱ�
        if (Input.GetKeyDown(KeyCode.Return)) ToggleCharacterListWindow();
        if (CharacterListWindow.activeSelf) // ĳ���� ��� â�� Ȱ��ȭ ������ ��
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCharacter(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCharacter(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCharacter(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectCharacter(3);
        }
    }

    public void ClosePartySetupWindow()
    {
        if (PartySetupWindow.activeSelf) // ��Ƽ �� â�� Ȱ��ȭ ������ ��
        {
            PartySetupWindow.SetActive(false); // ��Ƽ �� â ��Ȱ��ȭ
            MainUIManager.Instance.CurrentWindow = MainUI; // ���� â�� ���� UI�� ����
        }
    }

    public void ToggleCharacterListWindow()
    {
        if (!CharacterListWindow.activeSelf) // ĳ���� ��� â�� ��Ȱ��ȭ ������ ��
        {
            CharacterListWindow.SetActive(true); // ĳ���� ��� â Ȱ��ȭ
            ConfirmButton.SetActive(true); // Ȯ�� ��ư Ȱ��ȭ
        }
        else if (CharacterListWindow.activeSelf) // ĳ���� ��� â�� Ȱ��ȭ ������ ��
        {
            CharacterListWindow.SetActive(false); // ĳ���� ��� â ��Ȱ��ȭ
            ConfirmButton.SetActive(false); // Ȯ�� ��ư ��Ȱ��ȭ
        }
    }

    public void SelectCharacter(int CharacterIndex)
    {
        for (int i = 0; i < Character.Count; i++)
        {
            if (i == CharacterIndex)
            {
                // �ڽ� ������Ʈ �߿��� �̸��� "BorderLine"�� ������Ʈ�� ã�� Ȱ��ȭ
                Transform borderLine = Character[i].transform.Find("BorderLine");
                if (borderLine != null)
                {
                    if (borderLine.gameObject.activeSelf) // �׵θ� �̹����� Ȱ��ȭ ������ ��
                    {
                        DeselectCharacter(i); // ���� ����
                        return; // ���� ���� �� ����
                    }
                    if (partyCharacterNumbers[0] != 0 && partyCharacterNumbers[1] != 0 && partyCharacterNumbers[2] != 0) // ��Ƽ�� ���� á�� ��
                        return;
                    borderLine.gameObject.SetActive(true); // �׵θ� �̹��� Ȱ��ȭ
                }
                // �ڽ� ������Ʈ �߿��� �̸��� "Number"�� ������Ʈ�� ã��
                // Number�� �ڽ� ������Ʈ�� �ؽ�Ʈ ������Ʈ�� �ؽ�Ʈ�� ĳ���͸� ������ ������� �����ϰ� Number ������Ʈ�� Ȱ��ȭ
                Transform number = Character[i].transform.Find("Number");
                if (number != null)
                {
                    number.gameObject.SetActive(true); // Number ������Ʈ Ȱ��ȭ
                    Text numberText = number.GetComponentInChildren<Text>();
                    if (numberText != null)
                    {
                        // �ؽ�Ʈ�� ĳ���� ���� ������ ����
                        numberText.text = (partyCharacterNumbers[0] == 0) ? "1" :
                                          (partyCharacterNumbers[1] == 0) ? "2" : "3";
                        int characterNumber = int.Parse(numberText.text); // ĳ���� ��ȣ ��������
                        // ��Ƽ �迭�� ĳ���� ��ȣ ����
                        for (int j = 0; j < partyCharacterNumbers.Length; j++)
                        {
                            if (partyCharacterNumbers[j] == 0) // ����ִ� ���Կ� ĳ���� ��ȣ ����
                            {
                                partyCharacterNumbers[j] = characterNumber; // ĳ���� ��ȣ ����
                                break; // �ݺ��� ����
                            }
                        }
                        // ���Կ� ĳ���� �߰�
                        AddCharacterToParty(characterNumber - 1, CharacterIndex); // ���Կ� ĳ���� �߰�
                    }
                }
            }
        }
    }

    public void DeselectCharacter(int CharacterIndex)
    {
        // �ε����� �ش��ϴ� ĳ���� ������Ʈ�� �׵θ� �̹��� Ȱ��ȭ
        for (int i = 0; i < Character.Count; i++)
        {
            if (i == CharacterIndex)
            {
                // �ڽ� ������Ʈ �߿��� �̸��� "BorderLine"�� ������Ʈ�� ã�� Ȱ��ȭ
                Transform borderLine = Character[i].transform.Find("BorderLine");
                if (borderLine != null)
                {
                    borderLine.gameObject.SetActive(false); // �׵θ� �̹��� ��Ȱ��ȭ
                }
                // �ڽ� ������Ʈ �߿��� �̸��� "Number"�� ������Ʈ�� ã��
                // Number�� �ڽ� ������Ʈ�� �ؽ�Ʈ ������Ʈ�� �ؽ�Ʈ�� ĳ���͸� ������ ������� �����ϰ� Number ������Ʈ�� Ȱ��ȭ
                Transform number = Character[i].transform.Find("Number");
                if (number != null)
                {
                    number.gameObject.SetActive(false); // Number ������Ʈ ��Ȱ��ȭ
                    Text numberText = number.GetComponentInChildren<Text>();
                    if (numberText != null)
                    {
                        int characterNumber = int.Parse(numberText.text); // ĳ���� ��ȣ ��������
                        for (int j = 0; j < partyCharacterNumbers.Length; j++)
                        {
                            if (partyCharacterNumbers[j] == characterNumber) // ĳ���� ��ȣ�� ��ġ�� ��
                            {
                                partyCharacterNumbers[j] = 0; // ��Ƽ �迭���� ĳ���� ��ȣ ����
                                break;
                            }
                        }
                        numberText.text = "0"; // �ؽ�Ʈ�� ĳ���� ���� ������ ����
                        // ���Կ��� ĳ���� ����
                        RemoveCharacterFromParty(characterNumber - 1); // ���Կ��� ĳ���� ����
                    }
                }
            }
        }
    }

    public void AddCharacterToParty(int CharacterNumber, int CharacterIndex)
    {
        RawImage slotImage = CharacterSlots[CharacterNumber].GetComponent<RawImage>();
        if (slotImage != null)
        {
            slotImage.texture = CharacterRenderTexture[CharacterIndex]; // ���� �̹����� ĳ���� ���� �ؽ�ó�� ����
        }
    }



    public void RemoveCharacterFromParty(int CharacterNumber)
    {
        RawImage slotImage = CharacterSlots[CharacterNumber].GetComponent<RawImage>();
        if (slotImage != null)
        {
            slotImage.texture = CharacterSlotTexture; // ���� �̹����� ���� �ؽ�ó�� ����
        }
    }
}
