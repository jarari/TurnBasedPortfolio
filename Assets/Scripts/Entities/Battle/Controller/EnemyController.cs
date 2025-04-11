using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnBased.Entities.Roam
{
    public class EnemyController : MonoBehaviour
    {
        public Animator animator;
        public Transform player; //�÷��̾� ����

        private Vector3 startposition; //ó�� ��ġ
        private Vector3 patrolDir = Vector3.forward; // �⺻ �̵� ����

        public float MoveSpeed = 3f; //enemy �̵� �ӵ�.
        public float patrolDistance = 5; //enemy �̵��� �Ÿ�.
        public float detectRange = 7.5f; //enemy�� �÷��̾� ���� ����
                
        private bool isPatrol = true; // ó������ �����ϵ���
        public float rotateSpeed = 180f; // �÷��̾ ���� ���� �ӵ�.

        public float tryAttack = 2f; // ������ �õ��ϱ� ���� �ð�.
        private bool isAttack;
        private float detectTimer = 0f; // �÷��̾� ���� �ð� (tryAttack ��)

     

        private void Start()
        {
            startposition = transform.position; // ��ġ �ʱ�ȭ
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= detectRange)
            {

                isPatrol = false; // ���� ����.
                detectTimer += Time.deltaTime;

                //�÷��̾� �������� ȸ��
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0f; // ���� ȸ���� ����. 



                if (direction != Vector3.zero) //���� ���Ͱ� (0,0,0)�� �ƴҶ���
                {
                    //�ϴ� �ӽ÷� -direction
                    Quaternion targetRotation = Quaternion.LookRotation(-direction);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotateSpeed * Time.deltaTime);
                }
                    

                    if (detectTimer >= tryAttack) // ��� ���°� ���ӵ��� ��
                    {
                        EnemyAttack();
                        detectTimer = 0f;
                    }

                    return;
                }

                detectTimer = 0f;
                isPatrol = true;

                if (isPatrol)
                {
                    Patrol();
                }
        }
        
        

        private void Patrol()
        {
            // �̵�
            transform.Translate(patrolDir * MoveSpeed * Time.deltaTime);

            //���� ��ġ���� ������ �Ÿ���ŭ �̵������� ���� ��ȯ. 
            float movedistance = Vector3.Distance(startposition, transform.position);
            if (movedistance >= patrolDistance)
            {

                //�ݴ� �������� ��ȯ
                patrolDir = -patrolDir; 
                startposition = transform.position; //���ο� ����
            }
        }

        private void EnemyAttack()
        {
            if (!isAttack)
            {
                isAttack = true;
                Debug.Log("���� ! & �� ��ȯ.");
                PlayerPrefs.SetString("FirstAttack", "Enemy"); // enemy�� ���� ���ȴٴ� ���� ����.
                SceneManager.LoadScene("BattleScene"); // �� ��ȯ
            
                //  StartCoroutine(Attacking());
            }

        }
        //�ӽ÷� �ִϸ����Ϳ��� �ǰ� �ִϸ��̼� �����;���.
          private IEnumerator Attacking() 
            {
                 if (animator != null)
                 {
                    //���� ����� ���;� ��.
                    animator.SetTrigger("Hit");
                 }

                    PlayerPrefs.SetString("FirstAttack", "Enemy");

                    yield return new WaitForSeconds(1.2f);
                    //�� ��ȯ
                    SceneManager.LoadScene("BattleScene");
            }
        
    }
}
