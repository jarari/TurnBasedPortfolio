using System.Collections.Generic;
using TurnBased.Battle;
using TurnBased.Data;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance; // 싱글톤 인스턴스

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

        LoadCharacterData(); // 캐릭터 데이터 로드
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
    //    return characters[index]; // 해당 인덱스의 캐릭터 데이터 반환
    //}

    public int GetCharacterCount()
    {
        //return characters.Count; // 캐릭터 데이터 개수 반환
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
    //    public List<CharacterData> Characters; // JSON 배열을 리스트로 매핑
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
    //    public string ID; // 캐릭터 ID
    //    public string Name; // 캐릭터 이름
    //    public string Attribute; // 캐릭터 속성
    //    public int Health; // 체력
    //    public int Attack; // 공격력
    //    public int Defense; // 방어력
    //    public int Speed; // 속도
    //    public int BreakEffect; // 격파 특수효과
    //    public int CriticalRate; // 치명타 확률
    //    public int CriticalDamage; // 치명타 피해
    //    public string CharacterImagePath; // 캐릭터 이미지 경로
    //    public string CharacterRenderTexturePath; // 캐릭터 렌더 텍스쳐 경로
    //    public string AttributeImagePath; // 속성 이미지 경로
    //    public string BasicAttackImagePath; // 일반 공격 이미지 경로
    //    public string SkillImagePath; // 전투 스킬 이미지 경로
    //    public string UltimateImagePath; // 필살기 이미지 경로
    //    public string TalentImagePath; // 특성 이미지 경로
    //    public string TechniqueImagePath; // 비술 이미지 경로
    //}
}
