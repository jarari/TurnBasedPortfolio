using UnityEngine;

public class TechniquePointManager : MonoBehaviour
{
    public static TechniquePointManager Instance; // �̱��� �ν��Ͻ�

    public int maxTechniquePoints = 5; // �ִ� ��� ����Ʈ
    public int currentTechniquePoints; // ���� ��� ����Ʈ

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // �ν��Ͻ� ����
            if (transform.parent != null)
            {
                transform.SetParent(null); // ��Ʈ GameObject�� ����
            }
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
        }
    }

    void Start()
    {
        currentTechniquePoints = maxTechniquePoints; // ��� ����Ʈ ���� �ʱ�ȭ
    }

    public void UseTechnique()
    {
        if (currentTechniquePoints > 0) // ��� ����Ʈ�� �������� ��
            currentTechniquePoints--; // ��� ����Ʈ ����
        else
            Debug.Log("��� ����Ʈ ����");
    }

    public void AddTechniquePoint(int amount)
    {
        currentTechniquePoints += amount; // ��� ����Ʈ �߰�
        if (currentTechniquePoints > maxTechniquePoints) // �ִ� ��� ����Ʈ�� �ʰ����� ��
            currentTechniquePoints = maxTechniquePoints; // �ִ� ��� ����Ʈ�� ����
    }
}
