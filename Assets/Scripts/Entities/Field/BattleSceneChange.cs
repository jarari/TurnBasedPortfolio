using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene��ȯ�� ����� Ŭ����
/// </summary>
public class BattleSceneChange : MonoBehaviour
{
    // scene��ȯ ���� ����
    public float scenewait = 2.0f;

    /// <summary>
    /// ���� ������ ��ȯ�� �Լ�
    /// </summary>
    public void ChangeScene()
    {
        Debug.Log("������ ��ȯ");
        // ��� ����� scene�� ��ȯ
        StartCoroutine(BattleScene());
    }

    IEnumerator BattleScene()
    {
        Debug.Log("Scene��ȯ �غ�");
        // scenewait ��ŭ ��ٸ���
        yield return new WaitForSeconds(scenewait);

        Debug.Log("Scene��ȯ");
        // scene��ȯ
        SceneManager.LoadScene("BattleScene");

    }
}

