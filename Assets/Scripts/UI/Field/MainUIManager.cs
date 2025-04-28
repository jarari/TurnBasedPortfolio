using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Instance;       // 인스턴스

    public GameObject MainUI;                   // 메인 UI 오브젝트
    public GameObject PhoneWindow;              // 휴대폰 창 오브젝트
    public GameObject CharacterWindow;          // 캐릭터 창 오브젝트
    public GameObject PartySetupWindow;         // 파티 편성 창 오브젝트
    public GameObject TechniqueEffectWindow;    // 비술 효과 창 오브젝트
    public GameObject CurrentWindow;            // 현재 열려 있는 창

    public List<GameObject> partyMember;        // 파티원 오브젝트 리스트

    public GameObject InterActionButton;        // 상호작용 버튼 오브젝트

    public Button Attack;                       // 공격 버튼 오브젝트
    public Button Technique;                    // 비술 버튼 오브젝트
    public Button Run;                          // 달리기 버튼 오브젝트
    
    public AudioClip Select;                    // 선택 효과음
    public AudioClip Confirm;                   // 확인 효과음
    public AudioClip Cancel;                    // 취소 효과음

    private AudioSource audioSource;            // 오디오 소스
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 인스턴스 설정
            transform.SetParent(null); // 루트 GameObject로 설정
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else // 인스턴스가 이미 존재할 때
        {
            Destroy(gameObject); // 현재 오브젝트 파괴
        }

        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 컴포넌트 추가
    }

    void Start()
    {
        // 모든 창 닫기
        PhoneWindow.SetActive(false); // 휴대폰 창 비활성화
        CharacterWindow.SetActive(false); // 캐릭터 창 비활성화
        PartySetupWindow.SetActive(false); // 파티 편성 창 비활성화
        TechniqueEffectWindow.SetActive(false); // 비술 효과 창 비활성화
        InterActionButton.SetActive(false); // 상호작용 버튼 비활성화

        CurrentWindow = MainUI; // 현재 창을 메인 UI로 설정

        Cursor.visible = false; // 마우스 커서 숨기기

        Attack.GetComponent<Image>().enabled = false; // 공격 버튼 오브젝트 이미지 비활성화
        Technique.GetComponent<Image>().enabled = false; // 비술 버튼 오브젝트 이미지 비활성화
        Run.GetComponent<Image>().enabled = false; // 달리기 버튼 오브젝트 이미지 비활성화

    }

    void Update()
    {
        HandleCursor(); // 마우스 커서 처리
        OnKeyInput(); // 조작키 입력 처리
        UpdatePartyUI(); // 파티 UI 업데이트
    }

    private void HandleCursor()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || CurrentWindow != MainUI) // LeftAlt 키를 홀드하고 있거나 현재 열려있는 창이 메인 UI가 아닐 때
        {
            Cursor.visible = true; // 마우스 커서 보이기
        }
        else // 그 외의 경우
        {
            Cursor.visible = false; // 마우스 커서 숨기기
        }
    }

    public void OnKeyInput()
    {
        if (CurrentWindow == MainUI && !Cursor.visible) // 현재 창이 메인 UI이고 마우스 커서가 보이지 않을 때
        {
            // if (Input.GetKeyDown(KeyCode.Escape)) OpenWindow(PhoneWindow); // 휴대폰 창 열기
            if (Input.GetKeyDown(KeyCode.C)) OpenWindow(CharacterWindow); // 캐릭터 창 열기
            if (Input.GetKeyDown(KeyCode.L)) OpenWindow(PartySetupWindow); // 파티 편성 창 열기
            // if (Input.GetKeyDown(KeyCode.B)) OpenWindow(BackpackWindow); // 가방 창 열기
            if (Input.GetKeyDown(KeyCode.U)) OpenWindow(TechniqueEffectWindow); // 비술 효과 창 열기
            if (Input.GetKeyDown(KeyCode.E)) TechniqueButtonFlash(); // 비술 버튼 클릭 시 비술 버튼 이미지 깜빡임 효과
            if (Input.GetMouseButtonDown(0)) AttackButtonFlash(); // 공격 버튼 클릭 시 공격 버튼 이미지 깜빡임 효과
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift)) ToggleRunButton(); // 달리기 버튼 토글
        }
    }

    public void TechniqueButtonFlash()
    {
        TechniquePointManager.Instance.UseTechnique(); // 비술 포인트 사용
        StartCoroutine(TechniqueButtonCoroutine()); // 비술 버튼 이미지를 활성화하고 0.1초 뒤 비활성화
    }

    private IEnumerator TechniqueButtonCoroutine()
    {
        Technique.GetComponent<Image>().enabled = true; // 비술 버튼 이미지 활성화
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        Technique.GetComponent<Image>().enabled = false; // 비술 버튼 이미지 비활성화
    }

    public void AttackButtonFlash()
    {
        StartCoroutine(AttackButtonCoroutine()); // 공격 버튼 이미지를 활성화하고 0.1초 뒤 비활성화
    }

    private IEnumerator AttackButtonCoroutine()
    {
        Attack.GetComponent<Image>().enabled = true; // 공격 버튼 이미지 활성화
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        Attack.GetComponent<Image>().enabled = false; // 공격 버튼 이미지 비활성화
    }

    public void ToggleRunButton()
    {
        if (!Run.GetComponent<Image>().enabled) // 달리기 버튼 오브젝트의 이미지가 비활성화 상태일 때
            Run.GetComponent<Image>().enabled = true; // 달리기 버튼 오브젝트의 이미지를 활성화
        else // 달리기 버튼 오브젝트의 이미지가 활성화 상태일 때
            Run.GetComponent<Image>().enabled = false; // 달리기 버튼 오브젝트의 이미지를 비활성화
    }
    public void OpenWindow(GameObject window)
    {
        if (!window.activeSelf) // 창이 비활성화 상태일 때
        {
            window.SetActive(true); // 창 활성화
            CurrentWindow = window; // 현재 창을 연 창으로 설정
            audioSource.PlayOneShot(Select); // 선택 효과음 재생
        }
    }

    public void UpdatePartyUI()
    {
        string[] party = PartyManager.Instance.GetParty();
        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] != null)
            {
                // 파티원 오브젝트를 활성화
                partyMember[i].SetActive(true);
                
                Text characterNameText = partyMember[i].transform.Find("CharacterName").GetComponent<Text>(); // 파티원 오브젝트의 자식 오브젝트 중에서 이름이 "CharacterName"인 오브젝트를 찾기
                characterNameText.text = CharacterDataManager.GetCharacterName(party[i]); // 캐릭터 이름을 가져와서 텍스트 설정

                Image characterImage = partyMember[i].transform.Find("CharacterImage").GetComponent<Image>(); // 파티원 오브젝트의 자식 오브젝트 중에서 이름이 "CharacterImage"인 오브젝트를 찾기
                Sprite sprite = Resources.Load<Sprite>(CharacterDataManager.GetCharacterImagePath(party[i])); // 캐릭터 이미지 경로를 가져와서 스프라이트 로드
                characterImage.sprite = sprite; // 캐릭터 이미지 설정

                Image ultimateImage = partyMember[i].transform.Find("Ultimate").GetComponent<Image>(); // 파티원 오브젝트의 자식 오브젝트 중에서 이름이 "Ultimate"인 오브젝트를 찾기
                Sprite ultimateSprite = Resources.Load<Sprite>(CharacterDataManager.GetCharacterImagePath(party[i])); // 캐릭터 이미지 경로를 가져와서 스프라이트 로드
                // ultimateImage.sprite = ultimateSprite; // 캐릭터 이미지 설정
            }
            else
            {
                // 파티원 오브젝트를 비활성화
                partyMember[i].SetActive(false);
            }
        }
    }
}
