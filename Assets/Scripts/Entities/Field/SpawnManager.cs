using JetBrains.Annotations;
using NUnit.Framework;
using TurnBased.Entities.Field;
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawners")]
    // 스포너들을 담을 배열
    public GameObject[] spawners_1;
    public GameObject[] spawners_2;
    public GameObject[] spawners_3;

    [Header("Doors")]
    // 다음 영역으로 갈 문들
    public GameObject Exit_1;
    public GameObject Exit_2;
    public GameObject Exit_3;

    [Header("Starts")]
    public GameObject Start_1;
    public GameObject Start_2;
    public GameObject Start_3;

    [Header("Enemy")]
    // 소환할 에너미들
    public GameObject[] enemy;

    // 현재 진행도
    public int now = 0;

    // 현재 스폰이 돼었는지 확인하는 불값
    private bool spawnb = false;

    // 생존한 에너미들
    private List<GameObject> aliveEnemies = new List<GameObject>();
    
    // 현재 스포너
    GameObject[] n_spawner;

    // 플레이어 오브젝트 (이동을 위해)
    public GameObject player;

    bool check = false;

    private void OnEnable()
    {
        // exit 클래스의 이벤트 수신
        Exit.OnExitReached += HandleExitReached;
    }
    private void OnDisable()
    {
        // 이벤트 구독 해재
        Exit.OnExitReached -= HandleExitReached;
    }

    private void Start()
    {
        // 씬이 전환 되어 파괴되지 않도록 한다
        DontDestroyOnLoad(this.gameObject);

        // 전투에서 복귀한것이라면
        if (EncounterManager.Instance.stagedata != null)
        {
            Debug.Log("전투 복귀를 감지하였습니다  -> 스폰 생략");
            spawnb = true;      // 혹시나 싶어 이미 스폰된것으로 처리
            return;
        }

        // 시작시 첫번째 스포너를 가리키게한다
        n_spawner = spawners_1;
        // 시작시 에너미 스폰
        enemySpawn();
    }

    private void Update()
    {
        switch (now)
        {
            case 0:
                n_spawner = spawners_1;                
                break;
            case 1:
                n_spawner = spawners_2;                
                break;
            case 2:
                n_spawner= spawners_3;                
                break;
        }

        if (check == false)
        { 
            // 모든 에너미를 제거했다면
            if (aliveEnemies.Count <= 0)
            {            
                check = true;

                switch (now)
                {
                    case 0:
                         Exit_1.SetActive(true);                        
                        break;
                    case 1:
                        Exit_2.SetActive(true);
                        break;
                    case 2:
                        Exit_3.SetActive(true);
                        break;
                }
            }
        }

    }

    /// <summary>
    /// 현재 스포너의 배열에 에너미 소환
    /// </summary>
    public void enemySpawn()
    {
        // 만약 스폰이 되었다면 반환한다
        if (spawnb == true)
            return;


        // 스폰 완료
        spawnb = true;

        foreach (GameObject point in n_spawner)
        {
            // y값을 0으로 맞춰서 소환
            Vector3 spawnPosition = new Vector3(point.transform.position.x, 0, point.transform.position.z);

            // 랜덤으로 에너미 선택
            int rand = Random.Range(0, enemy.Length);
            
            // 에너미를 인스턴스 시킴
            GameObject spawned = Instantiate(enemy[rand], spawnPosition, Quaternion.identity);
                        
            // 살아있는 에너미 리스트에 추가
            aliveEnemies.Add(spawned);
            Debug.Log( spawned.name + "에너미가 스폰되었습니다");
        }        
    }

    /// <summary>
    /// 에너미의 사망시 처리할 함수
    /// </summary>
    /// <param name="d_enemy"></param>
    public void EnemyDead(GameObject d_enemy)
    {
        
        // 리스트에 d_enemy가 있다면
        if (aliveEnemies.Contains(d_enemy))
        { 
            // 리스트에서 삭제하고
            aliveEnemies.Remove(d_enemy);
            // 오브젝트를 제거한다
            Destroy(d_enemy);
            Debug.Log("데드 함수에서 오브젝트 제거됨"); 

        }

    }


    /// <summary>
    /// exit 이벤트를 수신하여 처리할 함수
    /// </summary>
    private void HandleExitReached()
    {        
        if (aliveEnemies.Count > 0)
            return;

        // 진행도 증가
        now++;
        
        // 생존한 적 리스트 초기화
        aliveEnemies.Clear();

        // 전투 관련 정보 초기화
        EncounterManager.Instance.Clear();

        // 플레이어 위치 변경
        Vector3 newPosition = player.transform.position;

        switch (now)
        {
            case 1:
                newPosition = Start_2.transform.position;
                break;
            case 2:
                newPosition = Start_3.transform.position;
                break;
            default:
                newPosition = Start_1.transform.position;
                break;
        }

        // y값을 0으로
        newPosition.y = 0;
        // 플레이어의 위치 업데이트
        player.transform.position = newPosition;

        // 스폰상태 초기화
        //spawnb = false;

        // 새로운 스폰을 시작
        enemySpawn();
    }
}