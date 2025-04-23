using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIManager : MonoBehaviour
{
    public static CharacterUIManager Instance; // �̱��� �ν��Ͻ�
    
    public GameObject MainUI;          // ���� UI ������Ʈ
    public GameObject CharacterWindow; // ĳ���� â ������Ʈ
    public GameObject DetailUI;        // �� ���� UI ������Ʈ
    public GameObject SkillUI;         // ��ų ���� UI ������Ʈ

    public Text NameText;   // ĳ���� �̸��� ǥ���� �ؽ�Ʈ
    public Image AttributeImage; // ĳ���� �Ӽ��� ǥ���� �̹���

    [System.Serializable]
    public class StatUI
    {
        public Text StatName;  // ���� �̸� �ؽ�Ʈ
        public Text StatValue; // ���� �� �ؽ�Ʈ
    }

    public List<Sprite> AttributeImages; // �Ӽ� �̹��� ����Ʈ

    public List<StatUI> StatUIList; // ���� UI ����Ʈ

    private int selectedCharacter = 0; // ���õ� ĳ���� �ε���

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
    private void Start()
    {
        // ������ �ε�
        CharacterDataManager.Instance.LoadCharacterData();

        // �⺻ ĳ���� UI ������Ʈ
        UpdateCharacterUI(selectedCharacter);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseCharacterWindow(); // ESC Ű�� ������ �� ĳ���� â �ݱ� 
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleDetailAndSkill(); // Tab Ű�� ������ �� �� ������ ��ų ���� ��ȯ
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCharacter(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCharacter(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCharacter(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectCharacter(4);
    }

    public void CloseCharacterWindow()
    {
        if (CharacterWindow.activeSelf) // ĳ���� â�� Ȱ��ȭ ������ ��
        {
            CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ
            MainUIManager.Instance.CurrentWindow = MainUI; // ���� â�� ���� UI�� ����
        }
    }

    public void ToggleDetailAndSkill()
    {
        if (CharacterWindow.activeSelf) // ĳ���� â�� Ȱ��ȭ ������ ��
        {
            if (!DetailUI.activeSelf) // �� ������ ��Ȱ��ȭ ������ ��
            {
                DetailUI.SetActive(true); // �� ���� Ȱ��ȭ
                SkillUI.SetActive(false); // ���� ��Ȱ��ȭ
            }
            else if (DetailUI.activeSelf) // �� ������ Ȱ��ȭ ������ ��
            {
                DetailUI.SetActive(false); // �� ���� ��Ȱ��ȭ
                SkillUI.SetActive(true); // ���� Ȱ��ȭ
            }
        }
    }

    public void SelectCharacter(int CharacterIndex)
    {
        if (CharacterWindow.activeSelf)
        {
            // �� ������ ��ų ������ ������ ĳ������ �ε����� ����
            selectedCharacter = CharacterIndex - 1;
            UpdateCharacterUI(selectedCharacter);
        }
    }

    public void UpdateCharacterUI(int characterIndex)
    {
        var character = CharacterDataManager.Instance.GetCharacterData(characterIndex); // ĳ���� ������ ��������
        if (character == null)
            return;

        // ĳ���� �̸��� �Ӽ� ���
        NameText.text = character.Name;

        // �Ӽ� �̹��� �ε� �� ����
        Sprite attributeSprite = AttributeImages.FirstOrDefault(sprite => sprite.name == character.Attribute); // �Ӽ� �̸��� ��ġ�ϴ� ��������Ʈ ã��
        if (attributeSprite != null)
            AttributeImage.sprite = attributeSprite;

        // ���� ������ ����
        var stats = new Dictionary<string, string>
        {
            { "HP", character.Health.ToString() },
            { "ATK", character.Attack.ToString() },
            { "DEF", character.Defense.ToString() },
            { "�ӵ�", character.Speed.ToString() },
            { "���� Ư��ȿ��", $"{character.BreakEffect}%" },
            { "ġ��Ÿ Ȯ��", $"{character.CriticalRate}%" },
            { "ġ��Ÿ ����", $"{character.CriticalDamage}%" }            
        };

        // ���� UI ������Ʈ
        for (int i = 0; i < StatUIList.Count && i < stats.Count; i++)
        {
            var stat = stats.ElementAt(i);
            StatUIList[i].StatName.text = stat.Key;   // ���� �̸� ����
            StatUIList[i].StatValue.text = stat.Value; // ���� �� ����
        }
    }
}
