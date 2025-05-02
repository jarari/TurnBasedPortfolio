using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechniquePointUIManager : MonoBehaviour
{
    public TechniquePointManager techniquePointManager; // ��� ����Ʈ �Ŵ���
    public List<GameObject> techniquePointObjects; // ��� ����Ʈ ������Ʈ ����Ʈ
    public Text techniquePointCount; // ��� ����Ʈ ���� ������Ʈ

    void Update()
    {
        UpdateTechniquePointsUI(); // ��� ����Ʈ UI ������Ʈ
    }

    public void UpdateTechniquePointsUI()
    {
        int currentPoints = techniquePointManager.currentTechniquePoints; // ���� ��� ����Ʈ ��������
        for (int i = 0; i < techniquePointObjects.Count; i++)
        {
            if (i < currentPoints) // ���� ��� ����Ʈ���� ���� ��
                techniquePointObjects[i].SetActive(true); // Ȱ��ȭ
            else // �� ���� ���
                techniquePointObjects[i].SetActive(false); // ��Ȱ��ȭ
        }
        techniquePointCount.text = currentPoints.ToString(); // ���� ��� ����Ʈ ������ �ؽ�Ʈ�� ���
    }
}
