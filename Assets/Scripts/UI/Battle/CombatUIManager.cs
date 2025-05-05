using System.Collections.Generic;
using System.Linq;
using TurnBased.Battle.Managers;
using TurnBased.Battle;
using TurnBased.Data;
using UnityEngine;
using UnityEngine.UI;
using TurnBased.Battle.UI.Element;
using System;


public class CombatUIManager : MonoBehaviour
{
    public static CombatUIManager Instance; // CombatUIManager 인스턴스

    public GameObject CombatUI;           // 전투 UI 오브젝트
    public GameObject PauseWindow;        // 일시 정지 창 오브젝트
    public GameObject CharacterWindow;    // 캐릭터 창 오브젝트
    public GameObject AllyCharacterList;  // 아군 캐릭터 목록 오브젝트
    public GameObject EnemyCharacterList; // 적 캐릭터 목록 오브젝트
    public GameObject StateRoot;
    public GameObject BasicAttackUI;      // 일반 공격 UI 오브젝트
    public GameObject BasicAttackUIBorder;// 일반 공격 UI 테두리 오브젝트
    public GameObject SkillUI;            // 전투 스킬 UI 오브젝트
    public GameObject SkillUIBorder;      // 전투 스킬 UI 테두리 오브젝트
    public GameObject UltimateUI;         // 필살기 UI 오브젝트

    public List<AllyState> AllyStates; // 아군 캐릭터 상태 표시 UI 엘레멘트

    public List<GameObject> SkillPoints; // 스킬 포인트 리스트
    public Text SkillPointText; // 스킬 포인트 개수 텍스트

    public RawImage BasicAttackIcon;
    public RawImage SkillIcon;

    public GameObject TargetSelectorRoot;
    public List<Transform> TargetSelectors;

    public GameObject CurrentWindow;     // 현재 열려 있는 창

    private Camera _mainCamera;

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
        _mainCamera = Camera.main;
    }

    void Start()
    {
        // 시작 시 모든 창 닫기
        PauseWindow.SetActive(false); // 휴대폰 창 비활성화
        CharacterWindow.SetActive(false); // 캐릭터 창 비활성화

        CurrentWindow = CombatUI; // 현재 창을 메인 UI로 설정

        CombatManager.instance.OnSkillPointChanged += UpdateSkillPointUI;
        CharacterManager.instance.OnCharacterSpawn += HandleCharacterSpawn;

        TurnManager.instance.OnBeforeTurnStart += HandleBeforeTurnStart;
        TurnManager.instance.OnTurnEnd += HandleTurnEnd;

        TargetManager.instance.OnTargetChanged += HandleTargetChanged;
        TargetManager.instance.OnTargetSettingChanged += HandleTargetSettingChanged;
    }

    void Update()
    {
        if (CurrentWindow == CombatUI) // 현재 창이 전투 UI일 때
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌렀을 때
                OpenPauseWindow(); // 일시 정지 창 열기
            if (Input.GetKeyDown(KeyCode.C)) // C 키를 눌렀을 때
                OpenAllyCharacterWindow(); // 아군 캐릭터 창 열기
            if (Input.GetKeyDown(KeyCode.Z)) // Z 키를 눌렀을 때
                OpenEnemyCharacterWindow(); // 적 캐릭터 창 열기
        }

        foreach (var selector in TargetSelectors) {
            if (selector.gameObject.activeInHierarchy) {
                selector.LookAt(_mainCamera.transform);
            }
        }
    }

    private void HandleCharacterSpawn(Character c, int idx) {
        if (c.Data.Team == CharacterTeam.Player) {
            InitializeAllyUI(c, idx);
        }
    }

    private void HandleCharacterStateChanged(Character c, Character.CharacterState state) {
        switch (state) {
            case Character.CharacterState.PrepareAttack:
                SelectObject(BasicAttackUI);
                break;
            case Character.CharacterState.PrepareSkill:
                SelectObject(SkillUI);
                break;
            case Character.CharacterState.PrepareUltAttack:
                SelectObject(BasicAttackUI);
                break;
            case Character.CharacterState.PrepareUltSkill:
                SelectObject(SkillUI);
                break;
        }
    }

    private void HandleBeforeTurnStart(TurnContext context) {
        var c = context.Character;
        if (c.Data.Team == CharacterTeam.Player) {
            c.OnCharacterStateChanged += HandleCharacterStateChanged;
            UpdateSkillIcons(c, context.Type);
        }
        else {
            TargetSelectorRoot.SetActive(false);
        }
    }

    private void HandleTurnEnd(TurnContext context) {
        var c = context.Character;
        if (c.Data.Team == CharacterTeam.Player) {
            c.OnCharacterStateChanged -= HandleCharacterStateChanged;
        }
    }

    private void UpdateTargetSelectors() {
        var targets = TargetManager.instance.GetTargets();
        if (targets.Count == 0) {
            TargetSelectorRoot.SetActive(false);
        }
        else {
            TargetSelectorRoot.SetActive(true);
            for (int i = 0; i < TargetSelectors.Count; ++i) {
                if (i >= targets.Count) {
                    TargetSelectors[i].gameObject.SetActive(false);
                }
                else {
                    TargetSelectors[i].gameObject.SetActive(true);
                    if (targets[i] != null && targets[i].Chest != null) {
                        TargetSelectors[i].transform.position = targets[i].Chest.transform.position;
                    }
                }
            }
        }
    }

    private void HandleTargetSettingChanged() {
        UpdateTargetSelectors();
    }

    private void HandleTargetChanged(Character character) {
        UpdateTargetSelectors();
    }

    private void UpdateSkillPointUI(int currentSkillPoints) {
        // 텍스트 업데이트
        SkillPointText.text = currentSkillPoints.ToString();

        // 스킬 포인트 오브젝트 활성화/비활성화
        for (int i = 0; i < SkillPoints.Count; i++) {
            SkillPoints[i].SetActive(i < currentSkillPoints);
        }
    }

    private void UpdateSkillIcons(Character c, TurnType type) {
        StateRoot.SetActive(true);
        if (type == TurnType.Normal) {
            Texture basicTex = Resources.Load<Texture>(c.Data.BaseData.BasicAttackImagePath);
            BasicAttackIcon.texture = basicTex;
            Texture skillTex = Resources.Load<Texture>(c.Data.BaseData.SkillImagePath);
            SkillIcon.texture = skillTex;
        }
        else if (type == TurnType.Ult) {
            Texture ultTex = Resources.Load<Texture>(c.Data.BaseData.UltimateImagePath);
            BasicAttackIcon.texture = ultTex;
            SkillIcon.texture = ultTex;
        }
        else {
            StateRoot.SetActive(false);
        }
    }

    public void OpenWindow(GameObject window, bool isAlly = true)
    {
        if (!window.activeSelf) // 창이 비활성화 상태일 때
        {
            if (window == CharacterWindow) // 캐릭터 창일 때
            {
                if (isAlly) // 아군 캐릭터 창을 열 때
                {
                    AllyCharacterList.SetActive(true); // 아군 캐릭터 리스트 활성화
                    EnemyCharacterList.SetActive(false); // 적 캐릭터 리스트 비활성화
                }
                else // 적 캐릭터 창을 열 때
                {
                    AllyCharacterList.SetActive(false); // 아군 캐릭터 리스트 비활성화
                    EnemyCharacterList.SetActive(true); // 적 캐릭터 리스트 활성화
                }
            }
            CurrentWindow.SetActive(false); // 현재 창 비활성화
            window.SetActive(true); // 창 활성화
            CurrentWindow = window; // 현재 창을 연 창으로 설정
        }
    }

    public void OpenPauseWindow()
    {
        OpenWindow(PauseWindow);
    }

    public void OpenAllyCharacterWindow()
    {
        OpenWindow(CharacterWindow, true);
    }

    public void OpenEnemyCharacterWindow()
    {
        OpenWindow(CharacterWindow, false);
    }

    public void SelectObject(GameObject AttackObject)
    {
        if (AttackObject == BasicAttackUI) // 일반 공격 UI일 때
        {
            UltimateUI.SetActive(false); // 필살기 UI 비활성화
            BasicAttackUIBorder.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.78f, 0f); // 일반 공격 UI 테두리 색상 변경
            SkillUIBorder.GetComponent<UnityEngine.UI.Image>().color = Color.white; // 전투 스킬 UI 테두리 색상 변경
        }
        else if (AttackObject == SkillUI) // 전투 스킬 UI일 때
        {
            UltimateUI.SetActive(false); // 필살기 UI 비활성화
            BasicAttackUIBorder.GetComponent<UnityEngine.UI.Image>().color = Color.white; // 일반 공격 UI 테두리 색상 변경
            SkillUIBorder.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.78f, 0f); // 전투 스킬 UI 테두리 색상 변경
        }
        else if (AttackObject == UltimateUI) // 필살기 UI일 때
        {
            UltimateUI.SetActive(true); // 필살기 UI 활성화
            BasicAttackUI.SetActive(false); // 일반 공격 UI 비활성화
            SkillUI.SetActive(false); // 전투 스킬 UI 비활성화
        }
    }

    public void InitializeAllyUI(Character c, int idx) {
        if (idx < AllyStates.Count) {
            var allyState = AllyStates[idx];
            allyState.gameObject.SetActive(true);
            allyState.InitializeAllyUI(c);
        }
    }

}
