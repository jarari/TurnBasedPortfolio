using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public static MainCameraController Instance { get; private set; } // �̱��� �ν��Ͻ�

    public Transform target; // ���� ���
    public Vector3 offset = new Vector3(0, 5, -10); // ī�޶��� �⺻ ��ġ ������
    public float rotationSpeed = 4f; // ���콺 ȸ�� �ӵ�

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // �̱��� �ν��Ͻ� ����
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }
    }

    private void Start()
    {
        // Ŀ���� �߾ӿ� �����Ѵ�
        Cursor.lockState = CursorLockMode.Locked;
        // Ŀ���� ȭ������� ������ �ʰ� �Ѵ�
        Cursor.lockState = CursorLockMode.Confined;
    }

    void LateUpdate()
    {
        if (!Input.GetKey(KeyCode.LeftAlt))
        {
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

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
