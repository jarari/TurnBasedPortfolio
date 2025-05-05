using TurnBased.Entities.Field;
using UnityEngine;


/// <summary>
/// �ʵ忡�� �ֿ� �̺�Ʈ�� �����ϴ� Ŭ����
/// </summary>
public class FieldManager : MonoBehaviour
{
    // �ʵ� ������ �����̴� �÷��̾� ������Ʈ
    [SerializeField] private GameObject player;

    
    private void Start()
    {
        HandleBattleReturn();
    }

    private void HandleBattleReturn()
    {
        // �������� �����Ѱ��� �ƴ϶�� ó������ ����
        if (EncounterManager.Instance.stagedata == null)
            return;

        // �÷��̾ ���� ���� ��ġ�� �̵�
        player.transform.position = EncounterManager.Instance.PlayerSpawnPos;

        if (EncounterManager.Instance.LastBattleResult)
        {

            // ��� F_Enemy�� ã�� �迭�� ��ȯ�Ѵ�
            F_Enemy[] enemies = GameObject.FindObjectsByType<F_Enemy>(FindObjectsSortMode.None);

            foreach (F_Enemy enemy in enemies)
            {
                // F_Enemy�� �߿� Ư�� ID�� ���� �༮�� �ִٸ�
                if (enemy.enemyID == EncounterManager.Instance.EnemyInstanceId)
                {
                    // ���� �����Ѵ�
                    Destroy(enemy.gameObject);
                    Debug.Log("�������� �¸��Ͽ��� ���ʹ�" + EncounterManager.Instance.EnemyInstanceId + "�� �����Ͽ����ϴ�.");
                    // ã�Ҵٸ� ����
                    break;
                }
            }

            // ���� ���� ���� �ʱ�ȭ
            EncounterManager.Instance.Clear();
            Debug.Log("EncounterManager ���� ���� �ʱ�ȭ �Ϸ�");
        }

    }


}
