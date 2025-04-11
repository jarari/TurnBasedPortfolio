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

        private bool Attack; //���� �����Ѱ�?

        private bool isAttack; // ���� �ߴ°�?

        public float moveSpeed = 7f;   // player �̵��ӵ�
       

        void Start()
        {
            rb = GetComponent<Rigidbody>(); // ������ٵ� �ʱ�ȭ
            animator = GetComponent<Animator>(); //�ִϸ����� �ʱ�ȭ
            if (image != null ) { image.SetActive(false); } //ó������ ��Ȱ��ȭ
        }

        private void Update()
        {
            if ( Attack && Input.GetMouseButtonDown(0) && !isAttack) //
            {
                isAttack = true;
              
                
                // StartCoroutine(PlayerAttack()); �ӽ÷� �ּ�ó��.
                Debug.Log(" �÷��̾� ���� ! & ��, ��ȯ");

                PlayerPrefs.SetString("FirstAttack", "Player"); //���� ���� ����
                SceneManager.LoadScene("BattleScene"); // BattleScene �ҷ�����

            }
        }
        private IEnumerator PlayerAttack()
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
            PlayerPrefs.SetString("FirstAttack", "Player"); //���� ���� ����

            yield return new WaitForSeconds(1.2f);

            SceneManager.LoadScene("BattleScene"); // BattleScene �ҷ�����
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
            if (other.CompareTag("Enemy")) //���ʹ� ��ü�� DetectionZone �� ������
            {
                Debug.Log("Enemy in"); // ǥ�� ui Ȱ��ȭ
                if (image != null)
                    image.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.CompareTag("Enemy")) //DetectionZone ������
            {
                if (image != null)
                    image.SetActive(false); //ui ��Ȱ��ȭ
            }
        }


        
    }
}
