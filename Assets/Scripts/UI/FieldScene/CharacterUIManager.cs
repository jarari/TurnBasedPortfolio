using NUnit.Framework;
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
    public Text StatsText;  // 스탯 정보를 표시할 텍스트
    public Text SkillsText; // 스킬 정보를 표시할 텍스트

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
        var character = CharacterDataManager.Instance.GetCharacterData(characterIndex);
        if (character == null) return;

        // UI 업데이트
        // NameText.text = $"Name: {character.Name}";
    }
}
