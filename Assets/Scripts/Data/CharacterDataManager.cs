using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance; // �̱��� �ν��Ͻ�

    private List<CharacterData> characters = new List<CharacterData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadCharacterData()
    {
        // Resources �������� JSON ���� �б�
        TextAsset jsonFile = Resources.Load<TextAsset>("CharacterData");
        if (jsonFile != null)
        {
            characters = JsonUtility.FromJson<ListWrapper<CharacterData>>(jsonFile.text).Items;
        }
        else
        {
            Debug.LogError("CharacterData.json ������ ã�� �� �����ϴ�.");
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
