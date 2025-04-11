using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


namespace TurnBased.Entities.Roam
{
    public class CharacterMove : MonoBehaviour
    {
        public GameObject image;
        public Animator animator;
        private Rigidbody rb;

        private bool Attack; //공격 가능한가?

        private bool isAttack; // 공격 했는가?

        public float moveSpeed = 7f;   // player 이동속도
       

        void Start()
        {
            rb = GetComponent<Rigidbody>(); // 리지드바디 초기화
            animator = GetComponent<Animator>(); //애니메이터 초기화
            if (image != null ) { image.SetActive(false); } //처음에는 비활성화
        }

        private void Update()
        {
            if ( Attack && Input.GetMouseButtonDown(0) && !isAttack) //
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy")) //에너미 개체가 DetectionZone 에 들어오면
            {
                Debug.Log("Enemy in"); // 표적 ui 활성화
                if (image != null)
                    image.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.CompareTag("Enemy")) //DetectionZone 나가면
            {
                if (image != null)
                    image.SetActive(false); //ui 비활성화
            }
        }


        
    }
}
