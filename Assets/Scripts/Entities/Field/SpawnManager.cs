using JetBrains.Annotations;
using NUnit.Framework;
using TurnBased.Entities.Field;
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawners")]
    // �����ʵ��� ���� �迭
    public GameObject[] spawners_1;
    public GameObject[] spawners_2;
    public GameObject[] spawners_3;

    [Header("Doors")]
    // ���� �������� �� ����
    public GameObject Exit_1;
    public GameObject Exit_2;
    public GameObject Exit_3;

    [Header("Starts")]
    public GameObject Start_1;
    public GameObject Start_2;
    public GameObject Start_3;

    [Header("Enemy")]
    // ��ȯ�� ���ʹ̵�
    public GameObject[] enemy;

    // ���� ���൵
    public int now = 0;

    // ���� ������ �ž����� Ȯ���ϴ� �Ұ�
    private bool spawnb = false;

    // ������ ���ʹ̵�
    private List<GameObject> aliveEnemies = new List<GameObject>();
    
    // ���� ������
    GameObject[] n_spawner;

    // �÷��̾� ������Ʈ (�̵��� ����)
    public GameObject player;

    bool check = false;

    private void OnEnable()
    {
        // exit Ŭ������ �̺�Ʈ ����
        Exit.OnExitReached += HandleExitReached;
    }
    private void OnDisable()
    {
        // �̺�Ʈ ���� ����
        Exit.OnExitReached -= HandleExitReached;
    }

    private void Start()
    {
        // ���� ��ȯ �Ǿ� �ı����� �ʵ��� �Ѵ�
        DontDestroyOnLoad(this.gameObject);

        // �������� �����Ѱ��̶��
        if (EncounterManager.Instance.stagedata != null)
        {
            Debug.Log("���� ���͸� �����Ͽ����ϴ�  -> ���� ����");
            spawnb = true;      // Ȥ�ó� �;� �̹� �����Ȱ����� ó��
            return;
        }

        // ���۽� ù��° �����ʸ� ����Ű���Ѵ�
        n_spawner = spawners_1;
        // ���۽� ���ʹ� ����
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
            // ��� ���ʹ̸� �����ߴٸ�
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
    /// ���� �������� �迭�� ���ʹ� ��ȯ
    /// </summary>
    public void enemySpawn()
    {
        // ���� ������ �Ǿ��ٸ� ��ȯ�Ѵ�
        if (spawnb == true)
            return;


        // ���� �Ϸ�
        spawnb = true;

        foreach (GameObject point in n_spawner)
        {
            // y���� 0���� ���缭 ��ȯ
            Vector3 spawnPosition = new Vector3(point.transform.position.x, 0, point.transform.position.z);

            // �������� ���ʹ� ����
            int rand = Random.Range(0, enemy.Length);
            
            // ���ʹ̸� �ν��Ͻ� ��Ŵ
            GameObject spawned = Instantiate(enemy[rand], spawnPosition, Quaternion.identity);
                        
            // ����ִ� ���ʹ� ����Ʈ�� �߰�
            aliveEnemies.Add(spawned);
            Debug.Log( spawned.name + "���ʹ̰� �����Ǿ����ϴ�");
        }        
    }

    /// <summary>
    /// ���ʹ��� ����� ó���� �Լ�
    /// </summary>
    /// <param name="d_enemy"></param>
    public void EnemyDead(GameObject d_enemy)
    {
        
        // ����Ʈ�� d_enemy�� �ִٸ�
        if (aliveEnemies.Contains(d_enemy))
        { 
            // ����Ʈ���� �����ϰ�
            aliveEnemies.Remove(d_enemy);
            // ������Ʈ�� �����Ѵ�
            Destroy(d_enemy);
            Debug.Log("���� �Լ����� ������Ʈ ���ŵ�"); 

        }

    }


    /// <summary>
    /// exit �̺�Ʈ�� �����Ͽ� ó���� �Լ�
    /// </summary>
    private void HandleExitReached()
    {        
        if (aliveEnemies.Count > 0)
            return;

        // ���൵ ����
        now++;
        
        // ������ �� ����Ʈ �ʱ�ȭ
        aliveEnemies.Clear();

        // ���� ���� ���� �ʱ�ȭ
        EncounterManager.Instance.Clear();

        // �÷��̾� ��ġ ����
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

        // y���� 0����
        newPosition.y = 0;
        // �÷��̾��� ��ġ ������Ʈ
        player.transform.position = newPosition;

        // �������� �ʱ�ȭ
        //spawnb = false;

        // ���ο� ������ ����
        enemySpawn();
    }
}