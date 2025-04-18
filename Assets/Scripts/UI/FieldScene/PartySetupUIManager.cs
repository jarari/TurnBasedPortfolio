using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PartySetupUIManager : MonoBehaviour
{
    public static PartySetupUIManager Instance; // 싱글톤 인스턴스

    public GameObject MainUI;              // 메인 UI 오브젝트
    public GameObject PartySetupWindow;    // 파티 편성 창 오브젝트
    public List<GameObject> CharacterSlots;// 캐릭터 슬롯 리스트
    public GameObject CharacterListWindow; // 캐릭터 목록 창 오브젝트
    public GameObject ConfirmButton;       // 확인 버튼
    public List<GameObject> Character;     // 캐릭터 리스트

    public List<Texture> CharacterRenderTexture; // 캐릭터 렌더 텍스처 리스트
    public Texture CharacterSlotTexture; // 캐릭터 슬롯 텍스처

    private int[] partyCharacterNumbers = new int[3]; // 파티에 들어온 캐릭터의 번호를 저장할 배열

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌렀을 때
            ClosePartySetupWindow(); // 파티 편성 창 닫기
        if (Input.GetKeyDown(KeyCode.Return)) ToggleCharacterListWindow();
        if (CharacterListWindow.activeSelf) // 캐릭터 목록 창이 활성화 상태일 때
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCharacter(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCharacter(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCharacter(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectCharacter(3);
        }
    }

    public void ClosePartySetupWindow()
    {
        if (PartySetupWindow.activeSelf) // 파티 편성 창이 활성화 상태일 때
        {
            PartySetupWindow.SetActive(false); // 파티 편성 창 비활성화
            MainUIManager.Instance.CurrentWindow = MainUI; // 현재 창을 메인 UI로 설정
        }
    }

    public void ToggleCharacterListWindow()
    {
        if (!CharacterListWindow.activeSelf) // 캐릭터 목록 창이 비활성화 상태일 때
        {
            CharacterListWindow.SetActive(true); // 캐릭터 목록 창 활성화
            ConfirmButton.SetActive(true); // 확인 버튼 활성화
        }
        else if (CharacterListWindow.activeSelf) // 캐릭터 목록 창이 활성화 상태일 때
        {
            CharacterListWindow.SetActive(false); // 캐릭터 목록 창 비활성화
            ConfirmButton.SetActive(false); // 확인 버튼 비활성화
        }
    }

    public void SelectCharacter(int CharacterIndex)
    {
        for (int i = 0; i < Character.Count; i++)
        {
            if (i == CharacterIndex)
            {
                // 자식 오브젝트 중에서 이름이 "BorderLine"인 오브젝트를 찾아 활성화
                Transform borderLine = Character[i].transform.Find("BorderLine");
                if (borderLine != null)
                {
                    if (borderLine.gameObject.activeSelf) // 테두리 이미지가 활성화 상태일 때
                    {
                        DeselectCharacter(i); // 선택 해제
                        return; // 선택 해제 후 종료
                    }
                    if (partyCharacterNumbers[0] != 0 && partyCharacterNumbers[1] != 0 && partyCharacterNumbers[2] != 0) // 파티가 가득 찼을 때
                        return;
                    borderLine.gameObject.SetActive(true); // 테두리 이미지 활성화
                }
                // 자식 오브젝트 중에서 이름이 "Number"인 오브젝트를 찾아
                // Number의 자식 오브젝트인 텍스트 오브젝트의 텍스트를 캐릭터를 선택한 순서대로 변경하고 Number 오브젝트를 활성화
                Transform number = Character[i].transform.Find("Number");
                if (number != null)
                {
                    number.gameObject.SetActive(true); // Number 오브젝트 활성화
                    Text numberText = number.GetComponentInChildren<Text>();
                    if (numberText != null)
                    {
                        // 텍스트를 캐릭터 선택 순서로 변경
                        numberText.text = (partyCharacterNumbers[0] == 0) ? "1" :
                                          (partyCharacterNumbers[1] == 0) ? "2" : "3";
                        int characterNumber = int.Parse(numberText.text); // 캐릭터 번호 가져오기
                        // 파티 배열에 캐릭터 번호 저장
                        for (int j = 0; j < partyCharacterNumbers.Length; j++)
                        {
                            if (partyCharacterNumbers[j] == 0) // 비어있는 슬롯에 캐릭터 번호 저장
                            {
                                partyCharacterNumbers[j] = characterNumber; // 캐릭터 번호 저장
                                break; // 반복문 종료
                            }
                        }
                        // 슬롯에 캐릭터 추가
                        AddCharacterToParty(characterNumber - 1, CharacterIndex); // 슬롯에 캐릭터 추가
                    }
                }
            }
        }
    }

    public void DeselectCharacter(int CharacterIndex)
    {
        // 인덱스에 해당하는 캐릭터 오브젝트의 테두리 이미지 활성화
        for (int i = 0; i < Character.Count; i++)
        {
            if (i == CharacterIndex)
            {
                // 자식 오브젝트 중에서 이름이 "BorderLine"인 오브젝트를 찾아 활성화
                Transform borderLine = Character[i].transform.Find("BorderLine");
                if (borderLine != null)
                {
                    borderLine.gameObject.SetActive(false); // 테두리 이미지 비활성화
                }
                // 자식 오브젝트 중에서 이름이 "Number"인 오브젝트를 찾아
                // Number의 자식 오브젝트인 텍스트 오브젝트의 텍스트를 캐릭터를 선택한 순서대로 변경하고 Number 오브젝트를 활성화
                Transform number = Character[i].transform.Find("Number");
                if (number != null)
                {
                    number.gameObject.SetActive(false); // Number 오브젝트 비활성화
                    Text numberText = number.GetComponentInChildren<Text>();
                    if (numberText != null)
                    {
                        int characterNumber = int.Parse(numberText.text); // 캐릭터 번호 가져오기
                        for (int j = 0; j < partyCharacterNumbers.Length; j++)
                        {
                            if (partyCharacterNumbers[j] == characterNumber) // 캐릭터 번호가 일치할 때
                            {
                                partyCharacterNumbers[j] = 0; // 파티 배열에서 캐릭터 번호 제거
                                break;
                            }
                        }
                        numberText.text = "0"; // 텍스트를 캐릭터 선택 순서로 변경
                        // 슬롯에서 캐릭터 제거
                        RemoveCharacterFromParty(characterNumber - 1); // 슬롯에서 캐릭터 제거
                    }
                }
            }
        }
    }

    public void AddCharacterToParty(int CharacterNumber, int CharacterIndex)
    {
        RawImage slotImage = CharacterSlots[CharacterNumber].GetComponent<RawImage>();
        if (slotImage != null)
        {
            slotImage.texture = CharacterRenderTexture[CharacterIndex]; // 슬롯 이미지를 캐릭터 렌더 텍스처로 변경
        }
    }



    public void RemoveCharacterFromParty(int CharacterNumber)
    {
        RawImage slotImage = CharacterSlots[CharacterNumber].GetComponent<RawImage>();
        if (slotImage != null)
        {
            slotImage.texture = CharacterSlotTexture; // 슬롯 이미지를 슬롯 텍스처로 변경
        }
    }
}
