using System.Collections;
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

        Debug.Log("LastBattleResult ��: " + EncounterManager.Instance.LastBattleResult);

        if (EncounterManager.Instance.LastBattleResult == true)
        {
            Debug.Log("�������� �¸��Ͽ����ϴ�.");

            StartCoroutine(waitManager());
                        
        }
        else
        {
            Debug.Log("�������� �й��Ͽ����ϴ�.");
        }

    }

    IEnumerator waitManager()
    { 
        yield return null;

        // ���� ���� �޴����� �����س��� ���ʹ� ������Ʈ�� �����´�
        GameObject defeatdEnemy = EncounterManager.Instance.enemy;
        defeatdEnemy.SetActive(true);

        if (EncounterManager.Instance.LastBattleResult == true)
        {
            Debug.Log("������ �����ߴ� ���ʹ� ������Ʈ�� ���� �����Ͽ� ���� �õ�");

            // �ʵ忡 �ִ� ���� �޴����� ã�� ���� ó��
            GameObject f_spawnM = GameObject.Find("SpawnManager");

            if (f_spawnM != null)
            {
                SpawnManager spawnManger = f_spawnM.GetComponent<SpawnManager>();

                if (spawnManger != null)
                {
                    spawnManger.EnemyDead(defeatdEnemy);
                    Debug.Log(defeatdEnemy.name + "������ ���� �Ϸ�");
                }
            }
            else
            {
                Debug.Log("������ ������ ���ʹ̰� null�Դϴ�");
            }

        }
    }

}
