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
        public Transform player; //플레이어 참조

        private Vector3 startposition; //처음 위치
        private Vector3 patrolDir = Vector3.forward; // 기본 이동 방향

        public float MoveSpeed = 3f; //enemy 이동 속도.
        public float patrolDistance = 5; //enemy 이동할 거리.
        public float detectRange = 7.5f; //enemy가 플레이어 감지 범위
                
        private bool isPatrol = true; // 처음부터 정찰하도록
        public float rotateSpeed = 180f; // 플레이어를 향해 보는 속도.

        public float tryAttack = 2f; // 공격을 시도하기 까지 시간.
        private bool isAttack;
        private float detectTimer = 0f; // 플레이어 감지 시간 (tryAttack 용)

     

        private void Start()
        {
            startposition = transform.position; // 위치 초기화
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= detectRange)
            {

                isPatrol = false; // 정찰 멈춤.
                detectTimer += Time.deltaTime;

                //플레이어 방향으로 회전
                Vector3 direction = (player.position - transform.position).normalized;
                direction.y = 0f; // 수직 회전을 방지. 



                if (direction != Vector3.zero) //방향 벡터가 (0,0,0)이 아닐때만
                {
                    //일단 임시로 -direction
                    Quaternion targetRotation = Quaternion.LookRotation(-direction);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotateSpeed * Time.deltaTime);
                }
                    

                    if (detectTimer >= tryAttack) // 경계 상태가 지속됐을 때
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
            // 이동
            transform.Translate(patrolDir * MoveSpeed * Time.deltaTime);

            //시작 위치에서 설정한 거리만큼 이동했으면 방향 전환. 
            float movedistance = Vector3.Distance(startposition, transform.position);
            if (movedistance >= patrolDistance)
            {

                //반대 방향으로 전환
                patrolDir = -patrolDir; 
                startposition = transform.position; //새로운 방향
            }
        }

        private void EnemyAttack()
        {
            if (!isAttack)
            {
                isAttack = true;
                Debug.Log("습격 ! & 씬 전환.");
                PlayerPrefs.SetString("FirstAttack", "Enemy"); // enemy가 먼저 때렸다는 정보 저장.
                SceneManager.LoadScene("BattleScene"); // 씬 전환
            
                //  StartCoroutine(Attacking());
            }

        }
        //임시로 애니메이터에서 피격 애니메이션 가져와야함.
          private IEnumerator Attacking() 
            {
                 if (animator != null)
                 {
                    //공격 모션이 나와야 함.
                    animator.SetTrigger("Hit");
                 }

                    PlayerPrefs.SetString("FirstAttack", "Enemy");

                    yield return new WaitForSeconds(1.2f);
                    //씬 전환
                    SceneManager.LoadScene("BattleScene");
            }
        
    }
}
