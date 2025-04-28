using UnityEngine;

public class TechniquePointManager : MonoBehaviour
{
    public static TechniquePointManager Instance; // 싱글톤 인스턴스

    public int maxTechniquePoints = 5; // 최대 비술 포인트
    public int currentTechniquePoints; // 현재 비술 포인트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 인스턴스 설정
            if (transform.parent != null)
            {
                transform.SetParent(null); // 루트 GameObject로 설정
            }
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스 삭제
        }
    }

    void Start()
    {
        currentTechniquePoints = maxTechniquePoints; // 비술 포인트 개수 초기화
    }

    public void UseTechnique()
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
