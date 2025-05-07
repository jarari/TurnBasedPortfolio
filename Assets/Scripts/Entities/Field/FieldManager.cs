using System.Collections;
using TurnBased.Battle.Managers;
using TurnBased.Entities.Field;
using UnityEngine;


/// <summary>
/// �ʵ忡�� �ֿ� �̺�Ʈ�� �����ϴ� Ŭ����
/// </summary>
public class FieldManager : MonoBehaviour
{
    [SerializeField] private GameObject player; // �÷��̾ �����̱� ���� ����
    [SerializeField] private Transform[] spawnPoints;   // �÷��̾� �̵� ����Ʈ
    [SerializeField] private Transform[] EnemyPoints;   // ���ʹ� ���� ����Ʈ    
    [SerializeField] private GameObject[] enemyPrefabs; // ���ʹ� ������
    [SerializeField] private GameObject BossPrefabs; // ���� ���ʹ� ������

    [SerializeField] private AudioClip fieldBGM;

    private int progress = 0;   // ���� ���൵

    private void Start()
    {
        // encounter���� ���൵�� �����´�
        progress = EncounterManager.Instance.StageProgress;

        SoundManager.instance.PlayMusic(fieldBGM);

        // �������� �¸��ߴ��� Ȯ��  
        if (EncounterManager.Instance.LastBattleResult)
        {
            HandleBattleResult();
        }
        else
        {
            // ������ ó�� ���۽� �ʱ� ���ʹ� ����
            SpawnInitialEnemy();
        }
    }

    IEnumerator DelayedMove() {
        yield return null;
        // ��ġ�� y���� 0����
        Vector3 playerPos = spawnPoints[progress].position;
        playerPos.y = 0;
        player.transform.position = playerPos;
        Debug.Log("�÷��̾� ��ġ �̵�: " + spawnPoints[progress].name);
    }

    /// <summary>
    /// �¸���
    /// </summary>
    private void HandleBattleResult()
    {
        Debug.Log("���� �¸� Ȯ�� - ���� ó�� ����");

        // ���൵�� ����
        progress = EncounterManager.Instance.StageProgress;

        // ���� ��������Ʈ�� �ִٸ�
        if (progress < spawnPoints.Length)
        {
            StartCoroutine(DelayedMove());

            Debug.Log(progress);


            Vector3 enemyPos = EnemyPoints[progress].position;
            enemyPos.y = 0;
            // ���ʹ� ��ȯ
            int rand = Random.Range(0, enemyPrefabs.Length);
            if (progress < 2)
            {
                Instantiate(enemyPrefabs[rand], enemyPos, Quaternion.identity);
            }
            else if (progress >= 2)
            { 
                Instantiate(BossPrefabs, enemyPos, Quaternion.identity);
            }
            Debug.Log("�� ��ȯ �Ϸ�");

            // Encounter ���� �ʱ�ȭ
            EncounterManager.Instance.Clear();
        }
        else
        {
            Debug.Log("��� ���� Ŭ����");
        }
    }

    // <summary>
    /// ���� ���� �� ù ���� ��ȯ
    /// </summary>
    private void SpawnInitialEnemy()
    {
        Debug.Log("���� ó�� ���� - ù �� ��ȯ");

        StartCoroutine(DelayedMove());

        // ���ʹ� ��ȯ
        Vector3 enemyPos = EnemyPoints[progress].position;
        enemyPos.y = 0;
        int rand = Random.Range(0, enemyPrefabs.Length);
        Instantiate(enemyPrefabs[rand], enemyPos, Quaternion.identity);
        Debug.Log("ù �� ��ȯ");
    }
}

