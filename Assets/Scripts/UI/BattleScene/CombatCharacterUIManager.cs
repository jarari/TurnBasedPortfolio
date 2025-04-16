using UnityEngine;
using System.Collections;

public class CombatCharacterUIManager : MonoBehaviour
{
    public GameObject CombatUI;           // ���� UI ������Ʈ
    public GameObject CharacterWindow;    // ĳ���� â ������Ʈ
    public GameObject AllyCharacterList;  // �Ʊ� ĳ���� ��� ������Ʈ
    public GameObject EnemyCharacterList; // �� ĳ���� ��� ������Ʈ

    void Update()
    {
        if (CombatUIManager.Instance.CurrentWindow == CharacterWindow) // ���� â�� ĳ���� â�� ��
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
                StartCoroutine(CloseCharacterWindowCoroutine()); // ĳ���� â �ݱ�
            if (Input.GetKeyDown(KeyCode.Tab)) // Tab Ű�� ������ ��
                ToggleAllyEnemySwitch(); // �Ʊ�/�� ĳ���� ����Ʈ ��ȯ
        }
    }

    public IEnumerator CloseCharacterWindowCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        if (CharacterWindow.activeSelf) // ĳ���� â�� Ȱ��ȭ ������ ��
        {
            CharacterWindow.SetActive(false); // ĳ���� â ��Ȱ��ȭ
            CombatUIManager.Instance.CurrentWindow = CombatUI; // ���� â�� ���� UI�� ����
            CombatUI.SetActive(true); // ���� UI Ȱ��ȭ
        }
    }

    public void CloseCharacterWindow()
    {
        StartCoroutine(CloseCharacterWindowCoroutine());
    }

    public void ToggleAllyEnemySwitch()
    {
        if (CharacterWindow.activeSelf) // ĳ���� â�� Ȱ��ȭ ������ ��
        {
            if (AllyCharacterList.activeSelf) // �Ʊ� ĳ���� ����Ʈ�� Ȱ��ȭ ������ ��
            {
                AllyCharacterList.SetActive(false); // �Ʊ� ĳ���� ����Ʈ ��Ȱ��ȭ
                EnemyCharacterList.SetActive(true); // �� ĳ���� ����Ʈ Ȱ��ȭ
            }
            else // �Ʊ� ĳ���� ����Ʈ�� ��Ȱ��ȭ ������ ��
            {
                AllyCharacterList.SetActive(true); // �Ʊ� ĳ���� ����Ʈ Ȱ��ȭ
                EnemyCharacterList.SetActive(false); // �� ĳ���� ����Ʈ ��Ȱ��ȭ
            }
        }
    }
}
