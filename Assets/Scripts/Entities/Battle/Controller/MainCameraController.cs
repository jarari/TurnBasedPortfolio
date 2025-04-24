using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 5, -10); // 카메라의 기본 위치 오프셋
    public float rotationSpeed = 5f; // 마우스 회전 속도

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned to MainCameraController.");
            return;
        }

        // 마우스 입력으로 카메라 회전
        currentRotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentRotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentRotationY = Mathf.Clamp(currentRotationY, -30f, 60f); // 상하 회전 제한

        // 회전 적용
        Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // 카메라 위치와 회전 설정
        transform.position = desiredPosition;
        transform.LookAt(target);
    }
}
