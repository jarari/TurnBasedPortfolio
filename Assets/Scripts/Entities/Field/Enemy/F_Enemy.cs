using UnityEngine;

namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// �ʵ��� ���ʹ� ��ũ��Ʈ
    /// </summary>
    public class F_Enemy : MonoBehaviour
    {

        #region ���ʹ��� ����

        // ���ʹ� ����
        public enum F_EnemyState { Idle,Move,Attack,Damage }
        // ��� ���� ����
        public F_EnemyState f_state;

        Animator anim;

        #endregion

        #region ���ʹ� ���� ����
        
        // ���ʹ� Ž�� �Ÿ�
        public float findDistnace = 4.0f;

        // ������ �÷��̾ ���� ����
        public GameObject target;
                
        // �÷��̾� ���ݰ��� ����
        public float attDistance = 2.0f;

        // ���ʹ��� ĳ���� ��Ʈ�ѷ��� ���� ����
        CharacterController cc;

        #endregion

        #region �ν��Ͻ��� ������ ģ���� (Ŭ����)

        // �÷��̾ Ž���� Ŭ����
        protected EnemyDetector detecter;
        protected EnemyMove Move;
        protected EnemyAttack attack;
        protected EnemySignal signal;

        #endregion

        private void OnEnable()
        {
            // �÷��̾ ���ʹ̿� ����������� �̺�Ʈ�� ����
            PlayerController.OnPlayerNearEnemy += findPlayer;
        }

        private void OnDisable()
        {
            // �̺�Ʈ ���� ����
            PlayerController.OnPlayerNearEnemy -= findPlayer;            
        }

        private void Start()
        {
            // ���۽� ���ʹ��� ���¸� �⺻���� �Ѵ�
            f_state = F_EnemyState.Idle;

            // ĳ���� ��Ʈ�ѷ��� �����´�
            cc = this.GetComponent<CharacterController>();

            // ĳ������ ���ϸ����͸� �����´�
            anim = this.GetComponent<Animator>();

            // �� Ŭ�������� �ν��Ͻ��� �����Ѵ�
            detecter = new EnemyDetector();
            Move = new EnemyMove();
            attack = new EnemyAttack();
            signal = new EnemySignal();
        }

        private void Update()
        {
            switch (f_state)
            {
                case F_EnemyState.Idle:
                    F_Idle();
                    break;
                case F_EnemyState.Move:
                    F_Move();
                    break;
                case F_EnemyState.Attack:
                    F_Attack();
                    break;                                
            }
        }


        public void F_Idle()  
        {
            // ���� ���� ���°� �⺻ ������ ���
            if (f_state == F_EnemyState.Idle)
            { 
                // �׳� ��ȯ�Ѵ�
                return; 
            }
        }

        public void F_Move() 
        {
            // ���ʹ̸� �÷��̾ ���� �����δ�
            Move.FE_Move(target.transform.position, cc, this.gameObject);

            // ���ʹ̰� Ÿ���� �ٶ󺸰� �Ѵ�.
            this.transform.forward = target.transform.position;


            // �÷��̾���� �Ÿ��� ����ϴ� �Ұ��� �Ѱ�
            bool A_switch = Move.FE_SwitchMove(target.transform.position, this.gameObject, attDistance);
            
            // ���ʹ̰� �÷��̾� ��ó�� �´ٸ�
            if (A_switch == true)
            {
                // ���¸� �������� �ٲ۴�
                f_state = F_EnemyState.Attack;
                Debug.Log("���� ���� : Move -> Attack");
                // �ִϸ��̼� Ʈ���Ÿ� �Ҵ�
                anim.SetTrigger("MoveToAttack");
            }

        }
        public void F_Attack()
        {
            // �÷��̾���� �Ÿ��� ����ϴ� �Ұ��� �Ѱ�
            bool A_switch = Move.FE_SwitchMove(target.transform.position, this.gameObject, attDistance);

            // �÷��̾ ���� �������� �־����ٸ�
            if (A_switch == false)
            {
                // ���ʹ��� ���¸� �̵����� �ٲ۴�
                f_state = F_EnemyState.Move;
                Debug.Log("���� ���� : Attack -> Move");
                // �ִϸ��̼� Ʈ���Ÿ� �Ҵ�
                anim.SetTrigger("AttackToMove");
            }

            bool battle = hit_signal(A_switch);

            // ���� ���ʹ��� ������ �÷��̾�� ��Ʈ ���� ���
            if (battle == true)
            {
                // ���� ������ ��ȯ�� �Լ��� �����Ѵ�
                attack.ChangeScene();
            }
            // ��Ʈ ���� ������ ���
            else
                return;
        }

        public virtual void findPlayer(GameObject player) 
        {
            target = player; // �÷��̾ Ÿ������ ����

            if (target != null) // Ÿ���� null�� �ƴҶ�
            {
                if (Vector3.Distance(transform.position, target.transform.position) <= findDistnace) // Ÿ�ٰ��� �Ÿ��� Ž���Ÿ����� �۰ų� ������
                {
                    // ���� ���¸� ����� �ٲ۴�
                    f_state = F_EnemyState.Move;
                    // �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
                    anim.SetTrigger("IdleToMove");
                }
                else if (Vector3.Distance(transform.position, target.transform.position) > findDistnace) // Ÿ�ٰ��� �Ÿ��� Ž�� �Ÿ����� �ֶ�
                {
                    // ���ʹ̻��¸� ��ȯ �Ѵ�
                    f_state = F_EnemyState.Idle;
                    // �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
                    anim.SetTrigger("MoveToIdle");
                }
            }
            else if (target == null)
            {
                // ���ʹ̻��¸� ��ȯ �Ѵ�
                f_state = F_EnemyState.Idle;
                // �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
                anim.SetTrigger("MoveToIdle");
            }
        
        }

        // ���� �ִϸ��̼� ����� �ñ׳��� ���� �Լ�
        public virtual bool hit_signal(bool a) { return false;}

    }

}