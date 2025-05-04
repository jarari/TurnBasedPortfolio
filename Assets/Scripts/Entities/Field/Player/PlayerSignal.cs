using TurnBased.Entities.Field;
using UnityEngine;

/// <summary>
/// 플레이어의 애니메이션에서 시그널을 받을 클래스
/// </summary>
public class PlayerSignal : MonoBehaviour
{
    // 부모 오브젝트에서 가져올 플레이어 스크립트
    public PlayerController playercc;

    private void Awake()
    {
        // 플레이어 스크립트가 비어있을때
        if (playercc == null)
            // 부모 오브젝트에서 플레이어 스크립트를 가져온다
            playercc = GetComponentInParent<PlayerController>();

        // 플레이어 스크립트를 찾지 못했을때
        if (playercc == null)
            Debug.Log("부모 오브젝트에서 PlayerController 스크립트를 찾지 못했습니다");
    }

    public void Signal()
    {
        // 에너미가 비어있지 않다면
        if (playercc != null)
            // 부모 오브젝트의 플레이어 컨트롤러 스크립트에서 히트 시그널을 호출한다
            playercc.hit_signal();

        // 플레이어가 비어있다면
        else
            Debug.Log("부모 player가 없습니다");
    }

}
