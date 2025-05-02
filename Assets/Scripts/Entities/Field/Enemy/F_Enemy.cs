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

        #endregion

        #region ���ʹ� ���� ����
        
        // ���ʹ� ���ǵ�
        public float speed = 4.0f;

        // ���ʹ� Ž�� �Ÿ�
        public float findDistnace = 4.0f;

        // ������ �÷��̾ ���� ����
        public GameObject target;
                
        // �÷��̾� ���ݰ��� ����
        public float attDistance = 2.0f;

        #endregion

        #region �ν��Ͻ��� ������ ģ����

        // �÷��̾ Ž���� Ŭ����
        protected EnemyDetector detecter;

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

            // Ž������ �ν��Ͻ��� �����Ѵ�
            detecter = new EnemyDetector();
        }

        private void Update()
        {
            //findPlayer();

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


        public virtual void F_Idle()  { }
        public virtual void F_Move() { }
        public virtual void F_Attack() { }

        public virtual void findPlayer(GameObject player) 
        {
            target = player; // �÷��̾ Ÿ������ ����

            if (target != null) // Ÿ���� null�� �ƴҶ�
            {
                if (Vector3.Distance(transform.position, target.transform.position) < findDistnace) // Ÿ�ٰ��� �Ÿ��� Ž���Ÿ����� ������
                {
                    Debug.Log("�÷��̾� ����: " + target.name);
                    // f_state = F_EnemyState.Move;
                }
            }
        
        }

    }

}