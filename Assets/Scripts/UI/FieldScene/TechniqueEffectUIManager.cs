using UnityEngine;

public class TechniqueEffectUIManager : MonoBehaviour
{
    public GameObject TechniqueEffectWindow; // ��Ƽ �� â ������Ʈ

    void Update()
    {
        // â �ݱ� ����Ű (ESC)
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
            CloseTechniqueEffectWindow(); // ��Ƽ �� â �ݱ�
    }

    public void CloseTechniqueEffectWindow()
    {
        if (TechniqueEffectWindow.activeSelf) // ��Ƽ �� â�� Ȱ��ȭ ������ ��
            TechniqueEffectWindow.SetActive(false); // ��Ƽ �� â ��Ȱ��ȭ
    }
}
