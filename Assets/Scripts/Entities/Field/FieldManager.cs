using System.Collections;
using TurnBased.Entities.Field;
using UnityEngine;


/// <summary>
/// 필드에서 주요 이벤트를 관리하는 클래스
/// </summary>
public class FieldManager : MonoBehaviour
{
    // 필드 씬에서 움직이는 플레이어 오브젝트
    [SerializeField] private GameObject player;

    private void Start()
    {
        HandleBattleReturn();
        
    }
    
    private void HandleBattleReturn()
    {
        // 전투에서 복귀한것이 아니라면 처리하지 않음
        if (EncounterManager.Instance.stagedata == null)
            return;               

        // 플레이어를 전투 직전 위치로 이동
        player.transform.position = EncounterManager.Instance.PlayerSpawnPos;

        Debug.Log("LastBattleResult 값: " + EncounterManager.Instance.LastBattleResult);

        if (EncounterManager.Instance.LastBattleResult == true)
        {
            Debug.Log("전투에서 승리하였습니다.");

            StartCoroutine(waitManager());
                        
        }
        else
        {
            Debug.Log("전투에서 패배하였습니다.");
        }

    }

    IEnumerator waitManager()
    { 
        yield return null;

        // 전투 진입 메니저에 저장해놓은 에너미 오브젝트를 가져온다
        GameObject defeatdEnemy = EncounterManager.Instance.enemy;
        defeatdEnemy.SetActive(true);

        if (EncounterManager.Instance.LastBattleResult == true)
        {
            Debug.Log("전투에 참여했던 에너미 오브젝트를 직접 참조하여 제거 시도");

            // 필드에 있는 스폰 메니저를 찾아 제거 처리
            GameObject f_spawnM = GameObject.Find("SpawnManager");

            if (f_spawnM != null)
            {
                SpawnManager spawnManger = f_spawnM.GetComponent<SpawnManager>();

                if (spawnManger != null)
                {
                    spawnManger.EnemyDead(defeatdEnemy);
                    Debug.Log(defeatdEnemy.name + "전투후 제거 완료");
                }
            }
            else
            {
                Debug.Log("전투에 참여한 에너미가 null입니다");
            }

        }
    }

}
