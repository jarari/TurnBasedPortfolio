using UnityEngine;

public class PartySetupUIManager : MonoBehaviour
{
    public GameObject PartySetupWindow; // ��Ƽ �� â ������Ʈ

    void Update()
    {
        // â �ݱ� ����Ű (ESC)
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
            ClosePartySetupWindow(); // ��Ƽ �� â �ݱ�
    }

    public void ClosePartySetupWindow()
    {
        if (PartySetupWindow.activeSelf) // ��Ƽ �� â�� Ȱ��ȭ ������ ��
            PartySetupWindow.SetActive(false); // ��Ƽ �� â ��Ȱ��ȭ
    }
}
