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
        public float findDistnace = 6.0f;

        // ������ �÷��̾ ���� ����
        public GameObject target;
                
        // �÷��̾� ���ݰ��� ����
        public float attDistance = 1.0f;

        // ���ʹ��� ĳ���� ��Ʈ�ѷ��� ���� ����
        CharacterController cc;

        // �̺�Ʈ �ߺ� ���Ź��� ����
        bool prevention = false;

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

            // �ڽĿ�����Ʈ�� ���ϸ����͸� �����´�
            anim = GetComponentInChildren<Animator>();

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
        }

        public void F_Move() 
        {
            if (target == null)
                return;

            // ���ʹ̸� �÷��̾ ���� �����δ�            
            cc.Move(Move.FE_MoveVector(target, this.gameObject, true));

            // ���ʹ̰� �÷��̾ �ٶ󺸰� �Ѵ�
            this.transform.forward = Move.FE_MoveVector(target, this.gameObject, false);

            // �÷��̾���� ���� ������ �Ÿ��� ����ϴ� �Ұ��� �Ѱ�
            bool A_switch = Move.FE_SwitchMove(target.transform.position, this.gameObject, attDistance);
            
            // �÷��̾ ���� ������ ���� ��ó�� �´ٸ�
            if (A_switch == true)
            {
                // ���¸� �������� �ٲ۴�
                f_state = F_EnemyState.Attack;
                Debug.Log("���� ���� : Move -> Attack");
                // �ִϸ��̼� Ʈ���Ÿ� �Ҵ�
                anim.SetTrigger("ToAttack");
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
                anim.SetTrigger("ToMove");
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
                    // Ÿ���� �ִ� ���¿��� �ߺ� ���� �Ұ��� false���
                    if (prevention == false)
                    { 
                        // ���� ���¸� ����� �ٲ۴�
                        f_state = F_EnemyState.Move;
                        // �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
                        anim.SetTrigger("ToMove");

                        // �ߺ� ���� �� �Ҵ�
                        prevention = true;
                    }
                }
                else if (Vector3.Distance(transform.position, target.transform.position) > findDistnace) // Ÿ�ٰ��� �Ÿ��� Ž�� �Ÿ����� �ֶ�
                {
                    if (prevention == true)
                    { 
                        // ���ʹ̻��¸� ��ȯ �Ѵ�
                        f_state = F_EnemyState.Idle;
                        // �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
                        anim.SetTrigger("ToIdle");
                    
                        // �ߺ� ���� �� �Ҵ�
                        prevention = false;
                    }
                }

            }
            
        
        }

        // ���� �ִϸ��̼� ����� �ñ׳��� ���� �Լ�
        public virtual bool hit_signal(bool a) { return false;}

    }

}