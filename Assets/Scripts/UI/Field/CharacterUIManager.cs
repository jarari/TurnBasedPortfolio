using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIManager : MonoBehaviour
{
    public static CharacterUIManager Instance; // �̱��� �ν��Ͻ�
    
    public GameObject MainUI;          // ���� UI ������Ʈ
    public GameObject CharacterWindow; // ĳ���� â ������Ʈ
    public GameObject DetailUI;        // �� ���� UI ������Ʈ
    public GameObject SkillUI;         // ��ų ���� UI ������Ʈ

    public RawImage ChracterRenderTexture; // ĳ���� ���� �ؽ���
    public Text NameText;   // ĳ���� �̸��� ǥ���� �ؽ�Ʈ
    public Image AttributeImage; // ĳ���� �Ӽ��� ǥ���� �̹���

    [System.Serializable]
    public class StatUI
    {
        public Text StatName;  // ���� �̸� �ؽ�Ʈ
        public Text StatValue; // ���� �� �ؽ�Ʈ
    }

    public List<StatUI> StatUIList; // ���� UI ����Ʈ

    public AudioClip Select;                    // ���� ȿ����
    public AudioClip Confirm;                   // Ȯ�� ȿ����
    public AudioClip Cancel;                    // ��� ȿ����

    private AudioSource audioSource;            // ����� �ҽ�

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

        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource ������Ʈ �߰�
    }
    private void Start()
    {
        // ������ �ε�
        CharacterDataManager.Instance.LoadCharacterData();

        // �⺻ ĳ���� UI ������Ʈ
        UpdateCharacterUI("C1");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseCharacterWindow(); // ESC Ű�� ������ �� ĳ���� â �ݱ�
        if (Input.GetKeyDown(KeyCode.Alpha1)) UpdateCharacterUI("C1");
        if (Input.GetKeyDown(KeyCode.Alpha2)) UpdateCharacterUI("C2");
        if (Input.GetKeyDown(KeyCode.Alpha3)) UpdateCharacterUI("C3");
        if (Input.GetKeyDown(KeyCode.Alpha4)) UpdateCharacterUI("C4");
    }

    public void CloseCharacterWindow()
    {
        if (CharacterWindow.activeSelf) // ĳ���� â�� Ȱ��ȭ ������ ��
        {
            CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ
            MainUIManager.Instance.CurrentWindow = MainUI; // ���� â�� ���� UI�� ����
            audioSource.PlayOneShot(Cancel); // ��� ȿ���� ���
        }
    }

    public void SelectDatailMenu()
    {
        if (!DetailUI.activeSelf) // �� ���� UI�� ��Ȱ��ȭ ������ ��
        {
            DetailUI.SetActive(true); // �� ���� Ȱ��ȭ
            SkillUI.SetActive(false); // ���� ��Ȱ��ȭ
            audioSource.PlayOneShot(Select); // ���� ȿ���� ���
        }
    }

    public void SelectSkillMenu()   
    {
        if (!SkillUI.activeSelf) // ��ų ���� UI�� Ȱ��ȭ ������ ��
        {
            DetailUI.SetActive(false); // �� ���� ��Ȱ��ȭ
            SkillUI.SetActive(true); // ���� Ȱ��ȭ
            audioSource.PlayOneShot(Select); // ���� ȿ���� ���
        }
    }

    public void UpdateCharacterUI(string ID)
    {
        var character = CharacterDataManager.GetCharacterDataByID(ID); // ĳ���� ������ ��������
        if (character == null)
            return;

        // ĳ���� �̸� ���
        NameText.text = character.Name;

        // ĳ���� ���� �ؽ��� �ε� �� ����
        RenderTexture renderTexture = Resources.Load<RenderTexture>(CharacterDataManager.GetCharacterRenderTexturePath(ID));
        ChracterRenderTexture.texture = renderTexture; // ���� �ؽ��� ����

        // �Ӽ� �̹��� �ε� �� ����
        // Sprite 
            // AttributeImage.sprite = attributeSprite;

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
        audioSource.PlayOneShot(Select); // ���� ȿ���� ���
    }
}
