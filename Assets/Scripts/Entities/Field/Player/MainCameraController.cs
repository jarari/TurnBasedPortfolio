using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public static MainCameraController Instance { get; private set; } // 싱글톤 인스턴스

    public Transform target; // 따라갈 대상
    public Vector3 offset = new Vector3(0, 5, -10); // 카메라의 기본 위치 오프셋
    public float rotationSpeed = 4f; // 마우스 회전 속도

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 싱글톤 인스턴스 설정
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    private void Start()
    {
        // 커서를 중앙에 고정한다
        Cursor.lockState = CursorLockMode.Locked;
        // 커서가 화면밖으로 나가지 않게 한다
        Cursor.lockState = CursorLockMode.Confined;
    }

    void LateUpdate()
    {
        if (!Input.GetKey(KeyCode.LeftAlt))
        {
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

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
