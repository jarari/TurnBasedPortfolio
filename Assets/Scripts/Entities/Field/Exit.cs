using System;
using UnityEngine;

public class Exit : MonoBehaviour
{
    // �߻���ų �̺�Ʈ
    public static event Action OnExitReached;

    // �浹��
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // �÷��̾�� �浹��
        if (hit.gameObject.CompareTag("Player"))
        {
            // �̺�Ʈ�� ���ٸ�
            if (OnExitReached != null)
            {
                // �̺�Ʈ�� �߻���Ų��
                OnExitReached?.Invoke();
                Debug.Log("Exit���� �߽�");
            }
        }
    }

}
