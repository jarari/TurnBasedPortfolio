using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public Transform target; // ���� ��� (�÷��̾�)
    public Vector3 offset = new Vector3(0, 5, -10); // ī�޶��� �⺻ ��ġ ������
    public float rotationSpeed = 5f; // ���콺 ȸ�� �ӵ�

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned to MainCameraController.");
            return;
        }

        // ���콺 �Է����� ī�޶� ȸ��
        currentRotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentRotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentRotationY = Mathf.Clamp(currentRotationY, -30f, 60f); // ���� ȸ�� ����

        // ȸ�� ����
        Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // ī�޶� ��ġ�� ȸ�� ����
        transform.position = desiredPosition;
        transform.LookAt(target);
    }
}
