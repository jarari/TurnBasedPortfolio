using System.Collections.Generic;
using TurnBased.Battle;
using TurnBased.Data;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance; // �̱��� �ν��Ͻ�

    [SerializeField]
    private CharacterTable _characterTable;

    public CharacterTable CharacterTable {
        get {
            return _characterTable;
        }
    }

    private Dictionary<string, CharacterData> _characterDict = new Dictionary<string, CharacterData>();

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
        //TextAsset jsonFile = Resources.Load<TextAsset>("CharacterData");
        //if (jsonFile == null)
        //    return;

        //characters = JsonUtility.FromJson<CharacterDataArrayWrapper>(jsonFile.text).Characters;


        foreach (var entry in _characterTable.entries) {
            if (!_characterDict.ContainsKey(entry.name)) {
                _characterDict.Add(entry.name, entry.characterData);
            }
        }
    }

    //public CharacterData GetCharacterDataByIndex(int index)
    //{
    //    return characters[index]; // �ش� �ε����� ĳ���� ������ ��ȯ
    //}

    public int GetCharacterCount()
    {
        //return characters.Count; // ĳ���� ������ ���� ��ȯ
        return _characterDict.Count;
    }

    public CharacterData GetCharacterData(string name) {
        CharacterData data;
        if (_characterDict.TryGetValue(name, out data)) {
            return data;
        }
        return null;
    }

    //[System.Serializable]
    //private class CharacterDataArrayWrapper
    //{
    //    public List<CharacterData> Characters; // JSON �迭�� ����Ʈ�� ����
    //}


    public static CharacterData GetCharacterDataByID(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character : null;
        return Instance.GetCharacterData(ID);
    }

    public static string GetCharacterName(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.Name : null;
        return GetCharacterDataByID(ID).Name;
    }

    public static string GetCharacterImagePath(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.CharacterImagePath : null;
        return GetCharacterDataByID(ID).CharacterImagePath;
    }

    public static string GetCharacterRenderTexturePath(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.CharacterRenderTexturePath : null;
        return GetCharacterDataByID(ID).CharacterRenderTexturePath;
    }

    public static string GetCharacterAttribute(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.Attribute : null;
        return GetCharacterDataByID(ID).Attribute;
    }

    public static string GetCharacterAttributeImagePath(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.AttributeImagePath : null;
        return GetCharacterDataByID(ID).AttributeImagePath;
    }

    public static string GetCharacterBasicAttackImagePath(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.BasicAttackImagePath : null;
        return GetCharacterDataByID(ID).BasicAttackImagePath;
    }

    public static string GetCharacterSkillImagePath(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.SkillImagePath : null;
        return GetCharacterDataByID(ID).SkillImagePath;
    }

    public static string GetCharacterUltimateImagePath(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.UltimateImagePath : null;
        return GetCharacterDataByID(ID).UltimateImagePath;
    }

    public static string GetCharacterTalentImagePath(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.TalentImagePath : null;
        return GetCharacterDataByID(ID).TalentImagePath;
    }

    public static string GetCharacterTechniqueImagePath(string ID)
    {
        //CharacterData character = Instance.characters.Find(c => c.ID == ID);
        //return character != null ? character.TechniqueImagePath : null;
        return GetCharacterDataByID(ID).TechniqueImagePath;
    }


    //[System.Serializable]
    //public class CharacterData
    //{
    //    public string ID; // ĳ���� ID
    //    public string Name; // ĳ���� �̸�
    //    public string Attribute; // ĳ���� �Ӽ�
    //    public int Health; // ü��
    //    public int Attack; // ���ݷ�
    //    public int Defense; // ����
    //    public int Speed; // �ӵ�
    //    public int BreakEffect; // ���� Ư��ȿ��
    //    public int CriticalRate; // ġ��Ÿ Ȯ��
    //    public int CriticalDamage; // ġ��Ÿ ����
    //    public string CharacterImagePath; // ĳ���� �̹��� ���
    //    public string CharacterRenderTexturePath; // ĳ���� ���� �ؽ��� ���
    //    public string AttributeImagePath; // �Ӽ� �̹��� ���
    //    public string BasicAttackImagePath; // �Ϲ� ���� �̹��� ���
    //    public string SkillImagePath; // ���� ��ų �̹��� ���
    //    public string UltimateImagePath; // �ʻ�� �̹��� ���
    //    public string TalentImagePath; // Ư�� �̹��� ���
    //    public string TechniqueImagePath; // ��� �̹��� ���
    //}
}
