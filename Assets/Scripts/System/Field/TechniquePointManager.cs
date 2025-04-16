using UnityEngine;

public class TechniquePointManager : MonoBehaviour
{
    public int maxTechniquePoints = 5; // 최대 비술 포인트
    public int currentTechniquePoints; // 현재 비술 포인트

    void Start()
    {
        currentTechniquePoints = maxTechniquePoints; // 비술 포인트 개수 초기화
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E 키를 눌렀을 때
            UseTechnique(); // 비술 사용
    }

    private void UseTechnique()
    {
        if (currentTechniquePoints > 0) // 비술 포인트가 남아있을 때
            currentTechniquePoints--; // 비술 포인트 감소
        else
            Debug.Log("비술 포인트 없음");
    }

    public void AddTechniquePoint(int amount)
    {
        currentTechniquePoints += amount; // 비술 포인트 추가
        if (currentTechniquePoints > maxTechniquePoints) // 최대 비술 포인트를 초과했을 때
            currentTechniquePoints = maxTechniquePoints; // 최대 비술 포인트로 설정
    }
}
