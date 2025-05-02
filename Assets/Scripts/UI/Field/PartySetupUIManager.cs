using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;

public class PartySetupUIManager : MonoBehaviour
{
    public static PartySetupUIManager Instance; // 싱글톤 인스턴스

    public GameObject MainUI;              // 메인 UI 오브젝트
    public GameObject PartySetupWindow;    // 파티 편성 창 오브젝트
    public List<GameObject> CharacterSlots;// 캐릭터 슬롯 리스트
    public GameObject CharacterListWindow; // 캐릭터 목록 창 오브젝트
    public GameObject ConfirmButton;       // 확인 버튼
    public List<GameObject> Character;     // 캐릭터 리스트

    public Texture CharacterSlotTexture; // 캐릭터 슬롯 텍스처

    public AudioClip Select;                    // 선택 효과음
    public AudioClip Confirm;                   // 확인 효과음
    public AudioClip Cancel;                    // 취소 효과음

    private AudioSource audioSource;            // 오디오 소스

    private void Awake()
    {
        if (Instance == null)
            Instance = this; // 인스턴스 설정
        else
            Destroy(gameObject); // 중복된 인스턴스 삭제

        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 컴포넌트 추가
    }

    private void Start()
    {
        CharacterListWindow.SetActive(false); // 캐릭터 목록 창 비활성화

        // 기본 캐릭터 추가
        PartyManager.Instance.AddCharacterToParty("C1");
        SelectCharacter("C1");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ClosePartySetupWindow();
        if (CharacterListWindow.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleCharacter("C1");
            if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleCharacter("C2");
            if (Input.GetKeyDown(KeyCode.Alpha3)) ToggleCharacter("C3");
            if (Input.GetKeyDown(KeyCode.Alpha4)) ToggleCharacter("C4");
        }
    }

    public void ClosePartySetupWindow()
    {
        if (PartySetupWindow.activeSelf) // 파티 편성 창이 활성화 상태일 때
        {
            PartySetupWindow.SetActive(false); // 파티 편성 창 비활성화
            CharacterListWindow.SetActive(false); // 캐릭터 목록 창 비활성화
            MainUIManager.Instance.CurrentWindow = MainUI; // 현재 창을 메인 UI로 설정
            audioSource.PlayOneShot(Cancel); // 취소 효과음 재생
        }
    }

    public void ToggleCharacterListWindow()
    {
        if (CharacterListWindow.activeSelf)PlayerController.Instance.SetPartyMemberName(); // 플레이어 컨트롤러에 파티원 이름 정보 보내기
        CharacterListWindow.SetActive(!CharacterListWindow.activeSelf);
        ConfirmButton.SetActive(CharacterListWindow.activeSelf);
        audioSource.PlayOneShot(Confirm); // 확인 효과음 재생
    }

    public void ToggleCharacter(string ID)
    {
        if (PartyManager.Instance.IsCharacterInParty(ID)) // 캐릭터가 이미 파티에 있으면
        {
            DeselectCharacter(ID); // 캐릭터 선택 해제
            PartyManager.Instance.RemoveCharacterFromParty(ID); // 슬롯에서 캐릭터 제거
            audioSource.PlayOneShot(Cancel); // 취소 효과음 재생
        }
        else // 캐릭터가 파티에 없으면
        {
            if (PartyManager.Instance.IsPartyFull()) return; // 파티가 가득 차면 편성 불가
            PartyManager.Instance.AddCharacterToParty(ID); // 슬롯에 캐릭터 추가
            SelectCharacter(ID); // 캐릭터 선택
            audioSource.PlayOneShot(Select); // 선택 효과음 재생
        }
    }

    private void SelectCharacter(string ID)
    {
        int partyNumber = PartyManager.Instance.GetPartyNumber(ID); // 파티에 편성된 캐릭터의 번호 가져오기
        int uNum = GetUniqueNumber(ID); // 캐릭터 고유 번호 가져오기

        for (int i = 0; i < Character.Count; i++) // 캐릭터 리스트를 순회
        {
            if (i == uNum) // 번호가 일치하는 캐릭터 오브젝트를 찾으면
            {
                Transform borderLine = Character[i].transform.Find("BorderLine"); // 캐릭터 오브젝트의 자식 오브젝트 중에서 이름이 "BorderLine"인 오브젝트를 찾기
                if (borderLine != null)
                    borderLine.gameObject.SetActive(true); // 테두리 활성화

                Transform number = Character[i].transform.Find("Number"); // 캐릭터 오브젝트의 자식 오브젝트 중에서 이름이 "Number"인 오브젝트를 찾기
                if (number != null)
                {
                    number.gameObject.SetActive(true); // 번호 활성화
                    Text numberText = number.GetComponentInChildren<Text>(); // Number 오브젝트의 자식 오브젝트 중에서 텍스트 컴포넌트를 찾기
                    if (numberText != null)
                    {
                        numberText.text = (partyNumber + 1).ToString(); // 캐릭터 선택 순서로 변경
                    }
                }
                AddCharacterToSlot(partyNumber, ID); // 슬롯에 캐릭터 추가
            }
        }
    }

    private void DeselectCharacter(string ID)
    {
        int partyNumber = PartyManager.Instance.GetPartyNumber(ID); // 파티에 편성된 캐릭터의 번호 가져오기
        int uNum = GetUniqueNumber(ID); // 캐릭터 고유 번호 가져오기

        for (int i = 0; i < Character.Count; i++) // 캐릭터 리스트를 순회
        {
            if (i == uNum) // 번호가 일치하는 캐릭터 오브젝트를 찾으면
            {
                Transform borderLine = Character[i].transform.Find("BorderLine"); // 캐릭터 오브젝트의 자식 오브젝트 중에서 이름이 "BorderLine"인 오브젝트를 찾기
                if (borderLine != null)
                    borderLine.gameObject.SetActive(false); // 테두리 비활성화
                Transform number = Character[i].transform.Find("Number"); // 캐릭터 오브젝트의 자식 오브젝트 중에서 이름이 "Number"인 오브젝트를 찾기
                if (number != null)
                {
                    number.gameObject.SetActive(false); // 번호 비활성화
                }
                RemoveCharacterFromSlot(partyNumber); // 슬롯에서 캐릭터 제거
            }
        }
    }
    private int GetUniqueNumber(string ID)
    {
        int num;
        switch (ID)
        {
            case "C1":
                num = 0;
                break;
            case "C2":
                num = 1;
                break;
            case "C3":
                num = 2;
                break;
            case "C4":
                num = 3;
                break;
            default:
                num = -1;
                break;
        }
        return num; // 캐릭터 번호 반환
    }

    public void AddCharacterToSlot(int CharacterNumber, string ID)
    {
        RenderTexture renderTexture = Resources.Load<RenderTexture>(CharacterDataManager.GetCharacterRenderTexturePath(ID));

        RawImage slotImage = CharacterSlots[CharacterNumber].GetComponent<RawImage>();
        if (slotImage != null) slotImage.texture = renderTexture; // 슬롯 이미지를 캐릭터 렌더 텍스처로 변경
    }



    public void RemoveCharacterFromSlot(int CharacterNumber)
    {
        RawImage slotImage = CharacterSlots[CharacterNumber].GetComponent<RawImage>();
        if (slotImage != null) slotImage.texture = CharacterSlotTexture; // 슬롯 이미지를 슬롯 텍스처로 변경
    }
}
