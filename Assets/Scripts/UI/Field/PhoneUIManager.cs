using UnityEngine;

public class PhoneUIManager : MonoBehaviour
{
    public GameObject PhoneWindow; // �޴��� â ������Ʈ

    void Update()
    {
        // â �ݱ� ����Ű (ESC)
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
            ClosePhoneWindow(); // �޴��� â �ݱ�
    }

    public void ClosePhoneWindow()
    {
        if (PhoneWindow.activeSelf) // �޴��� â�� Ȱ��ȭ ������ ��
            PhoneWindow.SetActive(false); // �޴��� â ��Ȱ��ȭ
    }
}