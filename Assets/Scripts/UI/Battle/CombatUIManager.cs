using System.Collections.Generic;
using System.Linq;
using TurnBased.Battle.Managers;
using TurnBased.Battle;
using TurnBased.Data;
using UnityEngine;
using UnityEngine.UI;


public class CombatUIManager : MonoBehaviour
{
    public static CombatUIManager Instance; // CombatUIManager �ν��Ͻ�

    public GameObject CombatUI;           // ���� UI ������Ʈ
    public GameObject PauseWindow;        // �Ͻ� ���� â ������Ʈ
    public GameObject CharacterWindow;    // ĳ���� â ������Ʈ
    public GameObject AllyCharacterList;  // �Ʊ� ĳ���� ��� ������Ʈ
    public GameObject EnemyCharacterList; // �� ĳ���� ��� ������Ʈ
    public GameObject BasicAttackUI;      // �Ϲ� ���� UI ������Ʈ
    public GameObject BasicAttackUIBorder;// �Ϲ� ���� UI �׵θ� ������Ʈ
    public GameObject SkillUI;            // ���� ��ų UI ������Ʈ
    public GameObject SkillUIBorder;      // ���� ��ų UI �׵θ� ������Ʈ
    public GameObject UltimateUI;         // �ʻ�� UI ������Ʈ

    public List<Image> AllyCharacterImagesUI; // �Ʊ� ĳ���� �̹����� ǥ���� UI ����Ʈ

    public List<GameObject> SkillPoints; // ��ų ����Ʈ ����Ʈ
    public Text SkillPointText; // ��ų ����Ʈ ���� �ؽ�Ʈ

    public List<Slider> HPSlider;          // HP �����̴�
    public List<Text> HPText; // HP �ؽ�Ʈ

    public List<GameObject> Ultimate;         // �ñر� ����Ʈ

    public GameObject CurrentWindow;     // ���� ���� �ִ� â

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
    }
    private void OnEnable()
    {
        if (CombatManager.instance != null)
        {
            // CombatManager�� SkillPoint ���� �̺�Ʈ ����
            CombatManager.instance.OnSkillPointChanged += UpdateSkillPointUI;
        }
    }

    private void OnDisable()
    {
        // �̺�Ʈ ���� ����
        CombatManager.instance.OnSkillPointChanged -= UpdateSkillPointUI;
    }

    void Start()
    {
        // ���� �� ��� â �ݱ�
        PauseWindow.SetActive(false); // �޴��� â ��Ȱ��ȭ
        CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ

        CurrentWindow = CombatUI; // ���� â�� ���� UI�� ����

        UpdateCharacterImages();
        UpdateCharacterHPUI();
        UpdateUltimateUI();
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
            if (Input.GetKeyDown(KeyCode.Q)) // Q Ű�� ������ ��
                SelectObject(BasicAttackUI); // �Ϲ� ���� UI ����
            if (Input.GetKeyDown(KeyCode.E)) // E Ű�� ������ ��
                SelectObject(SkillUI); // ���� ��ų UI ����
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

    public void UpdateSkillPointUI(int currentSkillPoints)
    {
        // �ؽ�Ʈ ������Ʈ
        SkillPointText.text = currentSkillPoints.ToString();

        // ��ų ����Ʈ ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
        for (int i = 0; i < SkillPoints.Count; i++)
        {
            SkillPoints[i].SetActive(i < currentSkillPoints);
        }
    }

    public void UpdateCharacterHPUI()
    {
        // �Ʊ� ĳ���� ����Ʈ ��������
        List<Character> allyCharacters = CharacterManager.instance.GetAllyCharacters();

        // �Ʊ� ü�� ������Ʈ
        for (int i = 0; i < HPSlider.Count; i++)
        {
            if (i < allyCharacters.Count)
            {
                HPSlider[i].gameObject.SetActive(true);
                HPSlider[i].value = allyCharacters[i].Data.HP.Current / allyCharacters[i].Data.HP.CurrentMax;

                // ü�� �ؽ�Ʈ ������Ʈ
                HPText[i].gameObject.SetActive(true);
                HPText[i].text = allyCharacters[i].Data.HP.Current.ToString();
            }
            else
            {
                HPSlider[i].gameObject.SetActive(false);

                // ü�� �ؽ�Ʈ ��Ȱ��ȭ
                HPText[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateCharacterImages()
    {
        // �Ʊ� ĳ���� ����Ʈ ��������
        List<Character> allyCharacters = CharacterManager.instance.GetAllyCharacters();

        for (int i = 0; i < AllyCharacterImagesUI.Count; i++)
        {
            if (i < allyCharacters.Count)
            {
                // ĳ���� �����Ϳ��� �̹��� ��� ��������
                string imagePath = allyCharacters[i].Data.BaseData.CharacterImagePath;

                // �̹��� �ε�
                Sprite characterSprite = Resources.Load<Sprite>(imagePath);

                if (characterSprite != null)
                {
                    // UI�� �̹��� ����
                    AllyCharacterImagesUI[i].sprite = characterSprite;
                    AllyCharacterImagesUI[i].gameObject.SetActive(true);
                }
                else
                {
                    AllyCharacterImagesUI[i].gameObject.SetActive(false);
                }
            }
            else
            {
                // ĳ���Ͱ� ������ �̹��� ��Ȱ��ȭ
                AllyCharacterImagesUI[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateUltimateUI()
    {
        // �Ʊ� ĳ���� ����Ʈ ��������
        List<Character> allyCharacters = CharacterManager.instance.GetAllyCharacters();

        for (int i = 0; i < Ultimate.Count; i++)
        {
            if (i < allyCharacters.Count)
            {
                // �ñر� ��� ���� ���� Ȯ��
                bool canUseUlt = CombatManager.CanCharacterUseUlt(allyCharacters[i]);

                // �ñر� ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
                Ultimate[i].SetActive(canUseUlt);
            }
            else
            {
                // ĳ���Ͱ� ������ �ñر� ������Ʈ ��Ȱ��ȭ
                Ultimate[i].SetActive(false);
            }
        }
    }

}
