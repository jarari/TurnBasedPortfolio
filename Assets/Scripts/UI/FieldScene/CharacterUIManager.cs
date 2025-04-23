using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIManager : MonoBehaviour
{
    public static CharacterUIManager Instance; // 싱글톤 인스턴스
    
    public GameObject MainUI;          // 메인 UI 오브젝트
    public GameObject CharacterWindow; // 캐릭터 창 오브젝트
    public GameObject DetailUI;        // 상세 정보 UI 오브젝트
    public GameObject SkillUI;         // 스킬 정보 UI 오브젝트

    public Text NameText;   // 캐릭터 이름을 표시할 텍스트
    public Image AttributeImage; // 캐릭터 속성을 표시할 이미지

    [System.Serializable]
    public class StatUI
    {
        public Text StatName;  // 스탯 이름 텍스트
        public Text StatValue; // 스탯 값 텍스트
    }

    public List<Sprite> AttributeImages; // 속성 이미지 리스트

    public List<StatUI> StatUIList; // 스탯 UI 리스트

    private int selectedCharacter = 0; // 선택된 캐릭터 인덱스

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 인스턴스 설정
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스 삭제
        }
    }
    private void Start()
    {
        // 데이터 로드
        CharacterDataManager.Instance.LoadCharacterData();

        // 기본 캐릭터 UI 업데이트
        UpdateCharacterUI(selectedCharacter);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseCharacterWindow(); // ESC 키를 눌렀을 때 캐릭터 창 닫기 
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleDetailAndSkill(); // Tab 키를 눌렀을 때 상세 정보와 스킬 정보 전환
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCharacter(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCharacter(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCharacter(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectCharacter(4);
    }

    public void CloseCharacterWindow()
    {
        if (CharacterWindow.activeSelf) // 캐릭터 창이 활성화 상태일 때
        {
            CharacterWindow.SetActive(false); // 캐릭터 창 비활성화
            MainUIManager.Instance.CurrentWindow = MainUI; // 현재 창을 메인 UI로 설정
        }
    }

    public void ToggleDetailAndSkill()
    {
        if (CharacterWindow.activeSelf) // 캐릭터 창이 활성화 상태일 때
        {
            if (!DetailUI.activeSelf) // 상세 정보가 비활성화 상태일 때
            {
                DetailUI.SetActive(true); // 상세 정보 활성화
                SkillUI.SetActive(false); // 행적 비활성화
            }
            else if (DetailUI.activeSelf) // 상세 정보가 활성화 상태일 때
            {
                DetailUI.SetActive(false); // 상세 정보 비활성화
                SkillUI.SetActive(true); // 행적 활성화
            }
        }
    }

    public void SelectCharacter(int CharacterIndex)
    {
        if (CharacterWindow.activeSelf)
        {
            // 상세 정보와 스킬 정보를 보여줄 캐릭터의 인덱스를 설정
            selectedCharacter = CharacterIndex - 1;
            UpdateCharacterUI(selectedCharacter);
        }
    }

    public void UpdateCharacterUI(int characterIndex)
    {
        var character = CharacterDataManager.Instance.GetCharacterData(characterIndex); // 캐릭터 데이터 가져오기
        if (character == null)
            return;

        // 캐릭터 이름과 속성 출력
        NameText.text = character.Name;

        // 속성 이미지 로드 및 설정
        Sprite attributeSprite = AttributeImages.FirstOrDefault(sprite => sprite.name == character.Attribute); // 속성 이름과 일치하는 스프라이트 찾기
        if (attributeSprite != null)
            AttributeImage.sprite = attributeSprite;

        // 스탯 데이터 매핑
        var stats = new Dictionary<string, string>
        {
            { "HP", character.Health.ToString() },
            { "ATK", character.Attack.ToString() },
            { "DEF", character.Defense.ToString() },
            { "속도", character.Speed.ToString() },
            { "격파 특수효과", $"{character.BreakEffect}%" },
            { "치명타 확률", $"{character.CriticalRate}%" },
            { "치명타 피해", $"{character.CriticalDamage}%" }            
        };

        // 스탯 UI 업데이트
        for (int i = 0; i < StatUIList.Count && i < stats.Count; i++)
        {
            var stat = stats.ElementAt(i);
            StatUIList[i].StatName.text = stat.Key;   // 스탯 이름 설정
            StatUIList[i].StatValue.text = stat.Value; // 스탯 값 설정
        }
    }
}
