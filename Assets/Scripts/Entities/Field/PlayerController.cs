using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // �⺻ �̵� �ӵ�
    public float runSpeed = 10f; // �޸��� �ӵ�
    public Transform cameraTransform; // ī�޶� Transform

    public GameObject[] characterPrefabs; // ĳ���� ������ �迭
    private GameObject currentCharacter; // ���� Ȱ��ȭ�� ĳ����

    private CharacterController characterController; // ĳ���� ��Ʈ�ѷ�
    private Animator animator; // Animator ������Ʈ
    private bool isRunning = false; // �޸��� ���� ����

    void Start()
    {
        // ������Ʈ ��������
        characterController = GetComponent<CharacterController>(); // CharacterController
        animator = GetComponent<Animator>(); // Animator

        // ChangeCharacter(0); // ù ��° ĳ���ͷ� �ʱ�ȭ
    }

    void Update()
    {
        KeyboardInput(); // Ű���� �Է� ó��

        // �޸��� ���� ���
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) // ����Ʈ Ű �Ǵ� ���콺 ��Ŭ��
        {
            isRunning = !isRunning;
        }

        // ���� �ӵ� ����
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        // �Է� �� ��������
        float horizontal = Input.GetAxis("Horizontal"); // A, D Ű (����, ������)
        float vertical = Input.GetAxis("Vertical");     // W, S Ű (��, ��)

        // ī�޶� ���� �̵� ���� ���
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // y�� ���� ���� (���� �̵��� ���)
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * vertical + right * horizontal) * currentSpeed;

        // �̵� ����
        characterController.Move(moveDirection * Time.deltaTime);

        // �̵� ������ 0�� �ƴ� ���� ȸ��
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // �ε巴�� ȸ��
        }

        // �ִϸ��̼� ó��
        if (animator != null)
        {
            if (moveDirection != Vector3.zero)
            {
                animator.SetBool("IsMoving", true); // �̵� ��
            }
            else
            {
                animator.SetBool("IsMoving", false); // �̵� ����
            }

            // �޸��� ���� ����
            animator.SetBool("IsRunning", isRunning);
        }
    }

    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCharacter(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCharacter(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeCharacter(2);
    }

    private void ChangeCharacter(int index)
    {
        if (index < 0 || index >= characterPrefabs.Length) return;

        // ���� ĳ���� ����
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
        }

        // ���ο� ĳ���� ����
        currentCharacter = Instantiate(characterPrefabs[index], transform.position, transform.rotation);

        // ���ο� ĳ������ Animator ����
        animator = currentCharacter.GetComponent<Animator>();

        // ���� ������Ʈ�� Transform�� ���ο� ĳ���Ϳ� ����ȭ
        currentCharacter.transform.parent = transform;

        // �� ĳ������ CamPos�� ã�� ī�޶� Ÿ������ ����
        Transform camPos = currentCharacter.transform.GetChild(0); // ù ��° �ڽ� ��������
        Debug.Log("CamPos: " + camPos);
        MainCameraController.Instance.SetTarget(camPos);
    }
}
