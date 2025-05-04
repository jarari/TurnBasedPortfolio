using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene전환을 담당할 클래스
/// </summary>
public class BattleSceneChange : MonoBehaviour
{
    // scene전환 대기용 변수
    public float scenewait = 2.0f;

    /// <summary>
    /// 전투 씬으로 전환할 함수
    /// </summary>
    public void ChangeScene()
    {
        Debug.Log("전투씬 전환");
        // 잠시 대기후 scene을 전환
        StartCoroutine(BattleScene());
    }

    IEnumerator BattleScene()
    {
        Debug.Log("Scene전환 준비");
        // scenewait 만큼 기다린다
        yield return new WaitForSeconds(scenewait);

        Debug.Log("Scene전환");
        // scene전환
        SceneManager.LoadScene("BattleScene");

    }
}

