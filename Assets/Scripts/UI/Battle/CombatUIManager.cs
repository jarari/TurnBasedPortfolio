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
    public static CombatUIManager Instance; // CombatUIManager �ν��Ͻ�

    public GameObject CombatUI;           // ���� UI ������Ʈ
    public GameObject PauseWindow;        // �Ͻ� ���� â ������Ʈ
    public GameObject CharacterWindow;    // ĳ���� â ������Ʈ
    public GameObject AllyCharacterList;  // �Ʊ� ĳ���� ��� ������Ʈ
    public GameObject EnemyCharacterList; // �� ĳ���� ��� ������Ʈ
    public GameObject StateRoot;
    public GameObject BasicAttackUI;      // �Ϲ� ���� UI ������Ʈ
    public GameObject BasicAttackUIBorder;// �Ϲ� ���� UI �׵θ� ������Ʈ
    public GameObject SkillUI;            // ���� ��ų UI ������Ʈ
    public GameObject SkillUIBorder;      // ���� ��ų UI �׵θ� ������Ʈ
    public GameObject UltimateUI;         // �ʻ�� UI ������Ʈ

    public List<AllyState> AllyStates; // �Ʊ� ĳ���� ���� ǥ�� UI ������Ʈ

    public List<GameObject> SkillPoints; // ��ų ����Ʈ ����Ʈ
    public Text SkillPointText; // ��ų ����Ʈ ���� �ؽ�Ʈ

    public RawImage BasicAttackIcon;
    public RawImage SkillIcon;

    public GameObject TargetSelectorRoot;
    public List<Transform> TargetSelectors;

    public GameObject CurrentWindow;     // ���� ���� �ִ� â

    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // �ν��Ͻ� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
        }
        _mainCamera = Camera.main;
    }

    void Start()
    {
        // ���� �� ��� â �ݱ�
        PauseWindow.SetActive(false); // �޴��� â ��Ȱ��ȭ
        CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ

        CurrentWindow = CombatUI; // ���� â�� ���� UI�� ����

        CombatManager.instance.OnSkillPointChanged += UpdateSkillPointUI;
        CharacterManager.instance.OnCharacterSpawn += HandleCharacterSpawn;

        TurnManager.instance.OnBeforeTurnStart += HandleBeforeTurnStart;
        TurnManager.instance.OnTurnEnd += HandleTurnEnd;

        TargetManager.instance.OnTargetChanged += HandleTargetChanged;
        TargetManager.instance.OnTargetSettingChanged += HandleTargetSettingChanged;
    }

    void Update()
    {
        if (CurrentWindow == CombatUI) // ���� â�� ���� UI�� ��
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
                OpenPauseWindow(); // �Ͻ� ���� â ����
            if (Input.GetKeyDown(KeyCode.C)) // C Ű�� ������ ��
                OpenAllyCharacterWindow(); // �Ʊ� ĳ���� â ����
            if (Input.GetKeyDown(KeyCode.Z)) // Z Ű�� ������ ��
                OpenEnemyCharacterWindow(); // �� ĳ���� â ����
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
        // �ؽ�Ʈ ������Ʈ
        SkillPointText.text = currentSkillPoints.ToString();

        // ��ų ����Ʈ ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
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
        if (!window.activeSelf) // â�� ��Ȱ��ȭ ������ ��
        {
            if (window == CharacterWindow) // ĳ���� â�� ��
            {
                if (isAlly) // �Ʊ� ĳ���� â�� �� ��
                {
                    AllyCharacterList.SetActive(true); // �Ʊ� ĳ���� ����Ʈ Ȱ��ȭ
                    EnemyCharacterList.SetActive(false); // �� ĳ���� ����Ʈ ��Ȱ��ȭ
                }
                else // �� ĳ���� â�� �� ��
                {
                    AllyCharacterList.SetActive(false); // �Ʊ� ĳ���� ����Ʈ ��Ȱ��ȭ
                    EnemyCharacterList.SetActive(true); // �� ĳ���� ����Ʈ Ȱ��ȭ
                }
            }
            CurrentWindow.SetActive(false); // ���� â ��Ȱ��ȭ
            window.SetActive(true); // â Ȱ��ȭ
            CurrentWindow = window; // ���� â�� �� â���� ����
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
        if (AttackObject == BasicAttackUI) // �Ϲ� ���� UI�� ��
        {
            UltimateUI.SetActive(false); // �ʻ�� UI ��Ȱ��ȭ
            BasicAttackUIBorder.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.78f, 0f); // �Ϲ� ���� UI �׵θ� ���� ����
            SkillUIBorder.GetComponent<UnityEngine.UI.Image>().color = Color.white; // ���� ��ų UI �׵θ� ���� ����
        }
        else if (AttackObject == SkillUI) // ���� ��ų UI�� ��
        {
            UltimateUI.SetActive(false); // �ʻ�� UI ��Ȱ��ȭ
            BasicAttackUIBorder.GetComponent<UnityEngine.UI.Image>().color = Color.white; // �Ϲ� ���� UI �׵θ� ���� ����
            SkillUIBorder.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.78f, 0f); // ���� ��ų UI �׵θ� ���� ����
        }
        else if (AttackObject == UltimateUI) // �ʻ�� UI�� ��
        {
            UltimateUI.SetActive(true); // �ʻ�� UI Ȱ��ȭ
            BasicAttackUI.SetActive(false); // �Ϲ� ���� UI ��Ȱ��ȭ
            SkillUI.SetActive(false); // ���� ��ų UI ��Ȱ��ȭ
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
