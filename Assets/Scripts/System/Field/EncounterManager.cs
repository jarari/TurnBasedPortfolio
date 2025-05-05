using System;
using TurnBased.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }   // �̱��� �ν��Ͻ�
    
    public StageData stagedata;  // ���� ���Խ� �Ѱ��� �������� ������
    public string EnemyInstanceId;  // ���ʹ� ���� ID (���� ���� �� �ش� �� ���� ��)
    public Vector3 PlayerSpawnPos;  // �ʵ忡�� ���� �� �÷��̾� ��ġ
    public bool LastBattleResult;   // ���� ����

    private void Awake()
    {
        // �̹� �ν��Ͻ��� �ִٸ�
        if (Instance != null)
        { 
            Destroy(gameObject);    // �ߺ� �޴��� ����
            return;
        }
        Instance = this;    // �ν��Ͻ� ����
        DontDestroyOnLoad(gameObject);  // ���� �ٲ� �ı����� �ʰ� ����
    }

    /// <summary>
    ///  ���� ���۽� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="enemyId"></param>
    /// <param name="playerPos"></param>
    public void StartEncounter(StageData stage, string enemyId, Vector3 playerPos)
    {
        stagedata = stage;                                      // ���� ���� SceneData ����
        EnemyInstanceId = enemyId;                    // �� ���� ����
        PlayerSpawnPos = playerPos;                   // �ʵ� ���ͽ� ��ġ
        SceneManager.LoadScene("BattleScene");  // �������� �ҷ��´�
    }

    /// <summary>
    /// ������ ������ ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="isWin"></param>
    public void FinishEncounter(bool isWin)
    {
        LastBattleResult = isWin;   // ���� ���θ� �����´�
        SceneManager.LoadScene("FieldScene");   // �ʵ� ���� �ҷ��´�
    }

    /// <summary>
    ///  ������ �ʱ�ȭ �Լ�
    /// </summary>
    public void Clear()
    {
        stagedata = null;
        EnemyInstanceId = "";
    }

}