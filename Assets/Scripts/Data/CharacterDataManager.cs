using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance; // 싱글톤 인스턴스

    private List<CharacterData> characters = new List<CharacterData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadCharacterData()
    {
        // Resources 폴더에서 JSON 파일 읽기
        TextAsset jsonFile = Resources.Load<TextAsset>("CharacterData");
        if (jsonFile != null)
        {
            characters = JsonUtility.FromJson<ListWrapper<CharacterData>>(jsonFile.text).Items;
        }
        else
        {
            Debug.LogError("CharacterData.json 파일을 찾을 수 없습니다.");
        }
    }

    public CharacterData GetCharacterData(int index)
    {
        if (index < 0 || index >= characters.Count) return null;
        return characters[index];
    }

    public int GetCharacterCount()
    {
        return characters.Count;
    }

    [System.Serializable]
    private class ListWrapper<T>
    {
        public List<T> Items;
    }
}
