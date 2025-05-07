using System.Collections;
using TurnBased.Battle.Managers;
using TurnBased.Entities.Field;
using UnityEngine;


/// <summary>
/// 필드에서 주요 이벤트를 관리하는 클래스
/// </summary>
public class FieldManager : MonoBehaviour
{
    [SerializeField] private GameObject player; // 플레이어를 움직이기 위한 변수
    [SerializeField] private Transform[] spawnPoints;   // 플레이어 이동 포인트
    [SerializeField] private Transform[] EnemyPoints;   // 에너미 스폰 포인트    
    [SerializeField] private GameObject[] enemyPrefabs; // 에너미 프리펩
    [SerializeField] private GameObject BossPrefabs; // 보스 에너미 프리펩

    [SerializeField] private AudioClip fieldBGM;

    private int progress = 0;   // 현재 진행도

    private void Start()
    {
        // encounter에서 진행도를 가져온다
        progress = EncounterManager.Instance.StageProgress;

        SoundManager.instance.PlayMusic(fieldBGM);

        // 전투에서 승리했는지 확인  
        if (EncounterManager.Instance.LastBattleResult)
        {
            HandleBattleResult();
        }
        else
        {
            // 게임을 처음 시작시 초기 에너미 스폰
            SpawnInitialEnemy();
        }
    }

    IEnumerator DelayedMove() {
        yield return null;
        // 위치의 y값을 0으로
        Vector3 playerPos = spawnPoints[progress].position;
        playerPos.y = 0;
        player.transform.position = playerPos;
        Debug.Log("플레이어 위치 이동: " + spawnPoints[progress].name);
    }

    /// <summary>
    /// 승리후
    /// </summary>
    private void HandleBattleResult()
    {
        Debug.Log("전투 승리 확인 - 진행 처리 시작");

        // 진행도를 증가
        progress = EncounterManager.Instance.StageProgress;

        // 다음 스폰포인트가 있다면
        if (progress < spawnPoints.Length)
        {
            StartCoroutine(DelayedMove());

            Debug.Log(progress);


            Vector3 enemyPos = EnemyPoints[progress].position;
            enemyPos.y = 0;
            // 에너미 소환
            int rand = Random.Range(0, enemyPrefabs.Length);
            if (progress < 2)
            {
                Instantiate(enemyPrefabs[rand], enemyPos, Quaternion.identity);
            }
            else if (progress >= 2)
            { 
                Instantiate(BossPrefabs, enemyPos, Quaternion.identity);
            }
            Debug.Log("적 소환 완료");

            // Encounter 상태 초기화
            EncounterManager.Instance.Clear();
        }
        else
        {
            Debug.Log("모든 지역 클리어");
        }
    }

    // <summary>
    /// 게임 시작 시 첫 적을 소환
    /// </summary>
    private void SpawnInitialEnemy()
    {
        Debug.Log("게임 처음 시작 - 첫 적 소환");

        StartCoroutine(DelayedMove());

        // 에너미 소환
        Vector3 enemyPos = EnemyPoints[progress].position;
        enemyPos.y = 0;
        int rand = Random.Range(0, enemyPrefabs.Length);
        Instantiate(enemyPrefabs[rand], enemyPos, Quaternion.identity);
        Debug.Log("첫 적 소환");
    }
}

