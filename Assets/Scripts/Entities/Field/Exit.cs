using System;
using UnityEngine;

public class Exit : MonoBehaviour
{
    // 발생시킬 이벤트
    public static event Action OnExitReached;

    // 충돌시
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 플레이어와 충돌시
        if (hit.gameObject.CompareTag("Player"))
        {
            // 이벤트가 없다면
            if (OnExitReached != null)
            {
                // 이벤트를 발생시킨다
                OnExitReached?.Invoke();
                Debug.Log("Exit에서 발신");
            }
        }
    }

}
