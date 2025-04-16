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

        public Transform[] enemise; // ������ ������ trnasform �迭.
        private Transform currentTarget; // ���� ������ ���� ����� ���� transform�� ����.

        public float Range = 7.5f; // ���� & ���� �Ÿ�.

        

        private bool isAttack; // ���� �ߴ°�?

        public float moveSpeed = 7f;   // player �̵��ӵ�
       

        void Start()
        {
            rb = GetComponent<Rigidbody>(); // ������ٵ� �ʱ�ȭ
            animator = GetComponent<Animator>(); //�ִϸ����� �ʱ�ȭ

            if (image != null)
                image.SetActive(false); // ���� �� UI ��Ȱ��ȭ

        }

        private void Update()
        {
            currentTarget = null; // ���� Ÿ���� �ʱ�ȭ
            float minDistance = Mathf.Infinity; //����� ���� ã������ ���� �ּҰŸ��� ���Ѵ��.

            //enemy ��ȸ
            foreach ( var enemy in enemise )
            {
                //�÷��̾�� �ش� �� ������ �Ÿ��� ����մϴ�.
                float dist = Vector3.Distance (transform.position, enemy.position);
              
                if (dist <= Range && dist < minDistance)
                {
                    minDistance = dist; //�ּ� �Ÿ� �� ���� Ÿ���� ������Ʈ.
                    currentTarget = enemy;
                    Debug.Log(" ���� ���� �߽��ϴ�");
                }

            }

            // UI Ȱ��ȭ: ���� ����� ���� ���� ���� ������ UI�� Ȱ��ȭ
            if (image != null)
                image.SetActive(currentTarget != null);


            // ���� ������ ���¿��� ������ ���� �ʾҰ� ���콺 ��Ŭ�� �ߴٸ�.
            if ( currentTarget != null && Input.GetMouseButtonDown(0) && !isAttack) 
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

     
    }
}
