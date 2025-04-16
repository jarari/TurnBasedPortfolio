using UnityEngine;

public class TechniquePointManager : MonoBehaviour
{
    public int maxTechniquePoints = 5; // �ִ� ��� ����Ʈ
    public int currentTechniquePoints; // ���� ��� ����Ʈ

    void Start()
    {
        currentTechniquePoints = maxTechniquePoints; // ��� ����Ʈ ���� �ʱ�ȭ
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E Ű�� ������ ��
            UseTechnique(); // ��� ���
    }

    private void UseTechnique()
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
