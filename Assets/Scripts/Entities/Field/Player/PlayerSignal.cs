using TurnBased.Entities.Field;
using UnityEngine;

/// <summary>
/// �÷��̾��� �ִϸ��̼ǿ��� �ñ׳��� ���� Ŭ����
/// </summary>
public class PlayerSignal : MonoBehaviour
{
    // �θ� ������Ʈ���� ������ �÷��̾� ��ũ��Ʈ
    public PlayerController playercc;

    private void Awake()
    {
        // �÷��̾� ��ũ��Ʈ�� ���������
        if (playercc == null)
            // �θ� ������Ʈ���� �÷��̾� ��ũ��Ʈ�� �����´�
            playercc = GetComponentInParent<PlayerController>();

        // �÷��̾� ��ũ��Ʈ�� ã�� ��������
        if (playercc == null)
            Debug.Log("�θ� ������Ʈ���� PlayerController ��ũ��Ʈ�� ã�� ���߽��ϴ�");
    }

    public void Signal()
    {
        // ���ʹ̰� ������� �ʴٸ�
        if (playercc != null)
            // �θ� ������Ʈ�� �÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ���� ��Ʈ �ñ׳��� ȣ���Ѵ�
            playercc.hit_signal();

        // �÷��̾ ����ִٸ�
        else
            Debug.Log("�θ� player�� �����ϴ�");
    }

}
