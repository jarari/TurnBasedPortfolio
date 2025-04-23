using System;
using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance; // �̱��� �ν��Ͻ�

    private List<CharacterData> characters = new List<CharacterData>(); // ĳ���� ������ ����Ʈ

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCharacterData(); // ĳ���� ������ �ε�
    }

    public void LoadCharacterData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("CharacterData");
        if (jsonFile == null)
            return;

        characters = JsonUtility.FromJson<CharacterDataArrayWrapper>(jsonFile.text).Characters;
    }

    public CharacterData GetCharacterData(int index)
    {
        return characters[index]; // �ش� �ε����� ĳ���� ������ ��ȯ
    }

    public int GetCharacterCount()
    {
        return characters.Count; // ĳ���� ������ ���� ��ȯ
    }

    [System.Serializable]
    private class CharacterDataArrayWrapper
    {
        public List<CharacterData> Characters; // JSON �迭�� ����Ʈ�� ����
    }

    [System.Serializable]
    public class CharacterData
    {
        public string ID; // ĳ���� ID
        public string Name; // ĳ���� �̸�
        public string Attribute; // ĳ���� �Ӽ�
        public int Health; // ü��
        public int Attack; // ���ݷ�
        public int Defense; // ����
        public int Speed; // �ӵ�
        public int BreakEffect; // ���� Ư��ȿ��
        public int CriticalRate; // ġ��Ÿ Ȯ��
        public int CriticalDamage; // ġ��Ÿ ����
        public string AttributeImagePath; // �Ӽ� �̹��� ���
        public string BasicAttackImagePath; // �Ϲ� ���� �̹��� ���
        public string SkillImagePath; // ���� ��ų �̹��� ���
        public string UltimateImagePath; // �ʻ�� �̹��� ���
        public string TalentImagePath; // Ư�� �̹��� ���
        public string TechniqueImagePath; // ��� �̹��� ���
    }
}
