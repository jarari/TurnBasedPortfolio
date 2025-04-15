using UnityEngine;
using System.Collections;

public class PauseUIManager : MonoBehaviour
{
    public GameObject CombatUI;     // ���� UI ������Ʈ
    public GameObject PauseWindow; // �Ͻ� ���� â ������Ʈ

    void Update()
    {
        if (CombatUIManager.Instance.CurrentWindow == PauseWindow && Input.GetKeyDown(KeyCode.Escape)) // ���� â�� �Ͻ� ���� â�̰� ESC Ű�� ������ ��
            StartCoroutine(ClosePauseWindowCoroutine()); // �Ͻ� ���� â �ݱ�
    }

    public IEnumerator ClosePauseWindowCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        if (PauseWindow.activeSelf) // �Ͻ� ���� â�� Ȱ��ȭ ������ ��
        {
            PauseWindow.SetActive(false); // �Ͻ� ���� â ��Ȱ��ȭ
            CombatUIManager.Instance.CurrentWindow = CombatUI; // ���� â�� ���� UI�� ����
            CombatUI.SetActive(true); // ���� UI Ȱ��ȭ
        }
    }

    public void ClosePauseWindow()
    {
        StartCoroutine(ClosePauseWindowCoroutine());
    }
}
