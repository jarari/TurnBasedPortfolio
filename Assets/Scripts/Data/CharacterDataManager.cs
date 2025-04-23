using System;
using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance; // 싱글톤 인스턴스

    private List<CharacterData> characters = new List<CharacterData>(); // 캐릭터 데이터 리스트

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
        LoadCharacterData(); // 캐릭터 데이터 로드
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
        return characters[index]; // 해당 인덱스의 캐릭터 데이터 반환
    }

    public int GetCharacterCount()
    {
        return characters.Count; // 캐릭터 데이터 개수 반환
    }

    [System.Serializable]
    private class CharacterDataArrayWrapper
    {
        public List<CharacterData> Characters; // JSON 배열을 리스트로 매핑
    }

    [System.Serializable]
    public class CharacterData
    {
        public string ID; // 캐릭터 ID
        public string Name; // 캐릭터 이름
        public string Attribute; // 캐릭터 속성
        public int Health; // 체력
        public int Attack; // 공격력
        public int Defense; // 방어력
        public int Speed; // 속도
        public int BreakEffect; // 격파 특수효과
        public int CriticalRate; // 치명타 확률
        public int CriticalDamage; // 치명타 피해
        public string AttributeImagePath; // 속성 이미지 경로
        public string BasicAttackImagePath; // 일반 공격 이미지 경로
        public string SkillImagePath; // 전투 스킬 이미지 경로
        public string UltimateImagePath; // 필살기 이미지 경로
        public string TalentImagePath; // 특성 이미지 경로
        public string TechniqueImagePath; // 비술 이미지 경로
    }
}
