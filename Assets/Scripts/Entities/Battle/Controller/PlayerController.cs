using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // �⺻ �̵� �ӵ�
    public float runSpeed = 10f; // �޸��� �ӵ�
    public Transform cameraTransform; // ī�޶� Transform
    private CharacterController characterController;
    private Animator animator; // Animator ������Ʈ
    private bool isRunning = false; // �޸��� ���� ����

    void Start()
    {
        // CharacterController ������Ʈ ��������
        characterController = GetComponent<CharacterController>();

        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();
    }

    void Update()
    {
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
}
