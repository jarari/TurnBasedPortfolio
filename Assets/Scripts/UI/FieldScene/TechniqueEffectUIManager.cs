using UnityEngine;

public class TechniqueEffectUIManager : MonoBehaviour
{
    public GameObject MainUI;                // ���� UI ������Ʈ
    public GameObject TechniqueEffectWindow; // ��� ȿ�� â ������Ʈ

    public AudioClip Cancel;                    // ��� ȿ����

    private AudioSource audioSource;            // ����� �ҽ�

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource ������Ʈ �߰�
    }

    void Update()
    {
        // â �ݱ� ����Ű (ESC)
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ��
            CloseTechniqueEffectWindow(); // ��� ȿ�� â �ݱ�
    }

    public void CloseTechniqueEffectWindow()
    {
        if (TechniqueEffectWindow.activeSelf) // ��� ȿ�� â�� Ȱ��ȭ ������ ��
        {
            TechniqueEffectWindow.SetActive(false); // ��� ȿ�� â ��Ȱ��ȭ
            MainUIManager.Instance.CurrentWindow = MainUI; // ���� â�� ���� UI�� ����
            audioSource.PlayOneShot(Cancel); // ��� ȿ���� ���
        }
    }
}
