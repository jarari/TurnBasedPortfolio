using System.Collections.Generic;
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

        LoadCharacterData(); // ĳ���� ������ �ε�
    }

    public void LoadCharacterData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("CharacterData");
        if (jsonFile == null)
            return;

        characters = JsonUtility.FromJson<CharacterDataArrayWrapper>(jsonFile.text).Characters;
    }

    public CharacterData GetCharacterDataByIndex(int index)
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


    public static CharacterData GetCharacterDataByID(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character : null;
    }

    public static string GetCharacterName(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.Name : null;
    }

    public static string GetCharacterImagePath(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.CharacterImagePath : null;
    }

    public static string GetCharacterRenderTexturePath(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.CharacterRenderTexturePath : null;
    }

    public static string GetCharacterAttribute(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.Attribute : null;
    }

    public static string GetCharacterAttributeImagePath(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.AttributeImagePath : null;
    }

    public static string GetCharacterBasicAttackImagePath(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.BasicAttackImagePath : null;
    }

    public static string GetCharacterSkillImagePath(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.SkillImagePath : null;
    }

    public static string GetCharacterUltimateImagePath(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.UltimateImagePath : null;
    }

    public static string GetCharacterTalentImagePath(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.TalentImagePath : null;
    }

    public static string GetCharacterTechniqueImagePath(string ID)
    {
        CharacterData character = Instance.characters.Find(c => c.ID == ID);
        return character != null ? character.TechniqueImagePath : null;
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
        public string CharacterImagePath; // ĳ���� �̹��� ���
        public string CharacterRenderTexturePath; // ĳ���� ���� �ؽ��� ���
        public string AttributeImagePath; // �Ӽ� �̹��� ���
        public string BasicAttackImagePath; // �Ϲ� ���� �̹��� ���
        public string SkillImagePath; // ���� ��ų �̹��� ���
        public string UltimateImagePath; // �ʻ�� �̹��� ���
        public string TalentImagePath; // Ư�� �̹��� ���
        public string TechniqueImagePath; // ��� �̹��� ���
    }
}
