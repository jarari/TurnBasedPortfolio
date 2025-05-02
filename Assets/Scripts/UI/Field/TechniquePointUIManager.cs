using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechniquePointUIManager : MonoBehaviour
{
    public TechniquePointManager techniquePointManager; // 비술 포인트 매니저
    public List<GameObject> techniquePointObjects; // 비술 포인트 오브젝트 리스트
    public Text techniquePointCount; // 비술 포인트 개수 오브젝트

    void Update()
    {
        UpdateTechniquePointsUI(); // 비술 포인트 UI 업데이트
    }

    public void UpdateTechniquePointsUI()
    {
        int currentPoints = techniquePointManager.currentTechniquePoints; // 현재 비술 포인트 가져오기
        for (int i = 0; i < techniquePointObjects.Count; i++)
        {
            if (i < currentPoints) // 현재 비술 포인트보다 작을 때
                techniquePointObjects[i].SetActive(true); // 활성화
            else // 그 외의 경우
                techniquePointObjects[i].SetActive(false); // 비활성화
        }
        techniquePointCount.text = currentPoints.ToString(); // 현재 비술 포인트 개수를 텍스트로 출력
    }
}
