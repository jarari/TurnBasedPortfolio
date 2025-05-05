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

        if (EncounterManager.Instance.LastBattleResult)
        {

            // 모든 F_Enemy를 찾아 배열에 반환한다
            F_Enemy[] enemies = GameObject.FindObjectsByType<F_Enemy>(FindObjectsSortMode.None);

            foreach (F_Enemy enemy in enemies)
            {
                // F_Enemy들 중에 특정 ID를 가진 녀석이 있다면
                if (enemy.enemyID == EncounterManager.Instance.EnemyInstanceId)
                {
                    // 적을 제거한다
                    Destroy(enemy.gameObject);
                    Debug.Log("전투에서 승리하였고 에너미" + EncounterManager.Instance.EnemyInstanceId + "를 제거하였습니다.");
                    // 찾았다면 종료
                    break;
                }
            }

            // 전투 관련 정보 초기화
            EncounterManager.Instance.Clear();
            Debug.Log("EncounterManager 내의 정보 초기화 완료");
        }

    }


}
