using UnityEngine;

public class CombatUIManager : MonoBehaviour
{
    // �ν��Ͻ�
    public static CombatUIManager Instance;
    
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

    void Start()
    {
        // ���� �� ��� â �ݱ�
        PauseWindow.SetActive(false); // �޴��� â ��Ȱ��ȭ
        CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ

        CurrentWindow = CombatUI; // ���� â�� ���� UI�� ����
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
}
