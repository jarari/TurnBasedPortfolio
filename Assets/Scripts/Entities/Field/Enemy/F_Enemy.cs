using TurnBased.Data;
using UnityEngine;
using System.Collections.Generic;

namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// �ʵ��� ���ʹ� ��ũ��Ʈ
    /// </summary>
    public class F_Enemy : MonoBehaviour, hit_Damage
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
        protected EnemySignal signal;

        // scene��ȯ�� ����� ����
        protected BattleSceneChange bs_change;

        #endregion

        #region ��������

        // �������� ������
        public StageData myStageData;

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
            // ���������� ��ȯ�� ��ũ��Ʈ�� �����´�
            bs_change = GetComponent<BattleSceneChange>();

            signal = transform.GetComponentInChildren<EnemySignal>();
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


        public void F_Idle() { }
        
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
        public void hit_signal() 
        {
            Debug.Log("���ʹ̰� ���� �ִϸ��̼ǿ��� �ñ׳��� ���� ����");

            hit_Damage phit = target.GetComponent<hit_Damage>();

            // �÷��̾��� ������ �Լ��� �����Ѵ�
            phit.Damage();

            // �������� �����Ϳ� ���ʹ� �̸� �߰�
            PrepareWaveData();

            // EncounterManger �� ���� ������ ���� + �� ȣ��
            EncounterManager.Instance.StartEncounter(myStageData, cc.gameObject.name, transform.position);
        }

        /// <summary>
        /// �������� �����Ϳ� ���ʹ��̸��� �����ϰ� �߰��� �Լ�
        /// </summary>
        public void PrepareWaveData()
        {
            // ���̺� �����Ͱ� ���ٸ� ����
            if (myStageData.waves == null || myStageData.waves.Count == 0)
            {
                myStageData.waves = new List<Wave>();
                myStageData.waves.Add(new Wave { enemies = new List<string>() });
            }

            var currentWave = myStageData.waves[0];
            currentWave.enemies.Clear();    // ���� ��� �ʱ�ȭ

            int random1 = Random.Range(2, 4);

            // �ڽ��� �̸��� ���� �߰�
            currentWave.enemies.Add(this.name);

            // ������ ���̸��� 3~5ȸ �߰�
            for (int i = 0; i < random1; i++)
            {
                // 0���� 4���� ������ ���� �����
                int random2 = Random.Range(0, 4);

                // ���� ���� ���ʹ� �̸��� �߰��Ѵ�
                switch (random2)
                {
                    case 0:
                        currentWave.enemies.Add("Alien_Soldier");
                        break;
                    case 1:
                        currentWave.enemies.Add("Eber");
                        break;
                    case 2:
                        currentWave.enemies.Add("Machine");
                        break;
                    case 3:
                        currentWave.enemies.Add("Mutant");
                        break;
                    case 4:
                        currentWave.enemies.Add("Y_Bot");
                        break;
                }

            }

        }

        // �ڽ��� ������ �޾�����
        public void Damage()
        {
            // �ִϸ������� Ʈ���Ÿ� �Ҵ�
            anim.SetTrigger("Damage");

            // �������� �����Ϳ� ���ʹ� �̸� �߰�
            PrepareWaveData();

            // EncounterManger �� ���� ������ ���� + �� ȣ��
            EncounterManager.Instance.StartEncounter(myStageData, cc.gameObject.name, transform.position);

        }

    }

}