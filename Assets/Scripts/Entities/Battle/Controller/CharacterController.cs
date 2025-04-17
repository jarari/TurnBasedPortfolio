using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


namespace TurnBased.Entities.Roam
{
    public class CharacterMove : MonoBehaviour
    {
        
        public Animator animator;
        public GameObject image;
        private Rigidbody rb;

        public Transform[] enemise; // 감지할 적들의 trnasform 배열.
        private Transform currentTarget; // 현재 감지된 가장 가까운 적의 transform을 저장.

        public float Range = 7.5f; // 감지 & 공격 거리.

        

        private bool isAttack; // 공격 했는가?

        public float moveSpeed = 7f;   // player 이동속도
       

        void Start()
        {
            rb = GetComponent<Rigidbody>(); // 리지드바디 초기화
            animator = GetComponent<Animator>(); //애니메이터 초기화

            if (image != null)
                image.SetActive(false); // 시작 시 UI 비활성화

        }

        private void Update()
        {
            currentTarget = null; // 현재 타겟을 초기화
            float minDistance = Mathf.Infinity; //가까운 적을 찾기위해 비교할 최소거리를 무한대로.

            //enemy 순회
            foreach ( var enemy in enemise )
            {
                //플레이어와 해당 적 사이의 거리를 계산합니다.
                float dist = Vector3.Distance (transform.position, enemy.position);
              
                if (dist <= Range && dist < minDistance)
                {
                    minDistance = dist; //최소 거리 와 현재 타겟을 업데이트.
                    currentTarget = enemy;
                    Debug.Log(" 적을 감지 했습니다");
                }

            }

            // UI 활성화: 가장 가까운 적이 범위 내에 있으면 UI를 활성화
            if (image != null)
                image.SetActive(currentTarget != null);


            // 공격 가능한 상태에서 공격을 하지 않았고 마우스 왼클릭 했다면.
            if ( currentTarget != null && Input.GetMouseButtonDown(0) && !isAttack) 
            {
                isAttack = true;
              
                
                // StartCoroutine(PlayerAttack()); 임시로 주석처리.
                Debug.Log(" 플레이어 공격 ! & 씬, 전환");

                PlayerPrefs.SetString("FirstAttack", "Player"); //선공 정보 저장
                SceneManager.LoadScene("BattleScene"); // BattleScene 불러오기

            }
        }
        private IEnumerator PlayerAttack()
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
            PlayerPrefs.SetString("FirstAttack", "Player"); //선공 정보 저장

            yield return new WaitForSeconds(1.2f);

            SceneManager.LoadScene("BattleScene"); // BattleScene 불러오기
        }

        void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal"); // A/D
            float v = Input.GetAxis("Vertical");   // W/S

            Vector3 move = new Vector3(h, 0f, v) * moveSpeed;
            rb.linearVelocity = move;
        }

     
    }
}
