using System;
using TurnBased.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }   // 싱글턴 인스턴스
    
    public StageData stagedata;  // 전투 진입시 넘겨줄 스테이지 데이터
    public string EnemyInstanceId;  // 에너미 고유 ID (전투 종료 후 해당 적 제거 용)
    public Vector3 PlayerSpawnPos;  // 필드에서 전투 전 플레이어 위치
    public bool LastBattleResult;   // 전투 승패

    private void Awake()
    {
        // 이미 인스턴스가 있다면
        if (Instance != null)
        { 
            Destroy(gameObject);    // 중복 메니저 제거
            return;
        }
        Instance = this;    // 인스턴스 지정
        DontDestroyOnLoad(gameObject);  // 씬이 바뀌어도 파괴되지 않게 설정
    }

    /// <summary>
    ///  전투 시작시 호출되는 함수
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="enemyId"></param>
    /// <param name="playerPos"></param>
    public void StartEncounter(StageData stage, string enemyId, Vector3 playerPos)
    {
        stagedata = stage;                                      // 전달 받은 SceneData 저장
        EnemyInstanceId = enemyId;                    // 적 정보 저장
        PlayerSpawnPos = playerPos;                   // 필드 복귀시 위치
        SceneManager.LoadScene("BattleScene");  // 전투씬을 불러온다
    }

    /// <summary>
    /// 전투가 끝나고 호출되는 함수
    /// </summary>
    /// <param name="isWin"></param>
    public void FinishEncounter(bool isWin)
    {
        LastBattleResult = isWin;   // 승패 여부를 가져온다
        SceneManager.LoadScene("FieldScene");   // 필드 씬을 불러온다
    }

    /// <summary>
    ///  데이터 초기화 함수
    /// </summary>
    public void Clear()
    {
        stagedata = null;
        EnemyInstanceId = "";
    }

}