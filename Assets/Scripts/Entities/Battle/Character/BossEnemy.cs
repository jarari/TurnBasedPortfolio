using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;
using static TurnBased.Entities.Battle.TestEnemyCharacter;


namespace TurnBased.Entities.Battle { 
    
    // ���� ����
    public class BossEnemy : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // �Ϲ� ���� �ִϸ��̼�
        public PlayableDirector skillAttack;        // ��ų ���� �ִϸ��̼�
        public PlayableDirector UltAttack;          // �ʻ�� ���� �ִϸ��̼�
        public PlayableDirector Dead_anim;             // ��� �ִϸ��̼�
        public PlayableDirector Groggy_anim;             // �׷α� �ִϸ��̼�

        // ==( ����ȭ�� ���ݴ� ����� ���� ����... )==

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        #region ���ʹ��� ����, ����

        // ���ʹ��� ���� (�븻, ����)
        public enum EnemyState { Nomal, Rampage }
        // ��� ���� ����
        public EnemyState e_State;

        // ����ȭ ���¸� ������ �Ұ�
        bool ram = false;

        // ä�� ������ �־��...? (������������?)

        #endregion

        // ������ ��ġ�� ȸ������ ���� ����
        public Vector3 EnPosition;
        public Vector3 EnRotate;

        // ĳ������ ������ ���� ���¸� ���� ����
        private CharacterState _lastAttack;
                
        // �����Ŀ� �ִϸ��̼��� ���� ���� ��ȯ�� ó���ϴ� �ڷ�ƾ
        private IEnumerator DelayReturnFromAttack()
        {
            // �Ͻ� ���� ���� ���� �����ӿ��� ������
            yield return null;
            // ������ ���� ���°� DoAttack �� ���
            if (_lastAttack == CharacterState.DoAttack)
            {
                // �Ϲ� ���� �ִϸ��̼��� ������ �����Ų��
                normalAttack.time = normalAttack.duration;
                // Ÿ�Ӷ����� ���� �ð��� �°� ���¸� ������Ʈ
                normalAttack.Evaluate();
            }
            // ������ ���� ���°� CastSkill �� ���
            else if (_lastAttack == CharacterState.CastSkill)
            {
                // ��ų ���� �ִϸ��̼��� ������ �����Ų��
                skillAttack.time = skillAttack.duration;
                // Ÿ�Ӷ����� ���� �ð��� �°� ���¸� ������Ʈ
                skillAttack.Evaluate();
            }
            else if (_lastAttack == CharacterState.CastUltAttack)
            {
                // �ʻ�� ���� �ִϸ��̼��� ������ �����Ų��
                UltAttack.time = UltAttack.duration;
                // Ÿ�Ӷ����� ���� �ð��� �°� ���¸� ������Ʈ
                UltAttack.Evaluate();
            }

        }

        // �ִϸ��̼� �̺�Ʈ�� �߻�������� ó���ϴ� �Լ�
        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload)
        {
            if (animEvent == "NomalAttackEnd" || animEvent == "SkillAttackEnd" || animEvent == "UltAttackEnd")
            {
                // ������ �ִϸ��̼� ó�� �ڷ�ƾ�� ȣ����
                StartCoroutine(DelayReturnFromAttack());
                // �����ϱ� ���� Ʋ���� ȸ������ ������� �����´�
                transform.eulerAngles = EnRotate;
                // ���� �����Ѵ�
                EndTurn();
            }
        }
        protected override void Awake()
        {
            base.Awake();
            // �̺�Ʈ�� �߻����� ��� ����� �Լ��� �߰��Ѵ�
            OnAnimationEvent += OnAnimationEvent_Impl;
        }

        protected override void Start()
        {
            base.Start();
            // ���۽� ���ʹ��� ���¸� Nomal�� �Ѵ�
            e_State = EnemyState.Nomal;
            // ���۽� ���ʹ��� ���� ��ġ�� ȸ������ �����Ѵ�
            EnPosition = transform.position;
            EnRotate = transform.eulerAngles;
        }

        // ���� �޾�����
        public override void TakeTurn()
        {
            // �θ� Ŭ�������� TakeTurn ���� �� ����
            base.TakeTurn();
       
            // �Ұ��� �̿��� �ѹ��� ȣ�� �ǵ����Ѵ�
            // ���ʹ��� ���� ä���� ��ä ä���� ���� ���ϰ� �ǰ� ����ȭ �Ұ��� false�϶�
            if (Data.stats.CurrentHP <= (Data.stats.MaxHP / 2) && ram == false)
            {
                // ĳ������ ���¸� ����ȭ �� �����Ѵ�
                e_State = EnemyState.Rampage;
                // ����ȭ�� ���ݷ��� 1.5���Ѵ�
                Data.stats.Attack += (Data.stats.Attack / 2);

                // �Ұ��� �����Ѵ�
                ram = true;
            }            

            // �����ϴ� �Լ�
            DoAttack();

            // ���� ������
            EndTurn();

        }

        #region �ൿ�ϴ� �Լ� (��ų, ����, �ñر�, ����Ʈ�� ����)

        // ��ų�� ����Ҷ�
        public override void CastSkill()
        {
            // �θ� Ŭ�������� CastSkill ������ ����
            base.CastSkill();
            Debug.Log(gameObject.name + " �� ��ų �ߵ�!");

            // Ÿ������ �����Ѵ� (�÷��̾��)
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Player);
            // �÷��̾� Ÿ���� �����´� (1��)
            var player = TargetManager.instance.Target;
            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            meshParent.transform.forward = player.gameObject.transform.position;

            // ��ų ���� �ִϸ��̼� ���
            //skillAttack.Play();

            // ������ ������ ��ų�������� �˸���
            _lastAttack = CharacterState.CastSkill;
        }
        // �ñر�
        public override void CastUltAttack()
        {
            base.CastUltAttack();
            Debug.Log(gameObject.name + " �� �ʻ�� �ߵ�!");

            // Ÿ������ �����Ѵ� (�÷��̾��)
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Player);
            // �÷��̾� Ÿ���� �����´� (1��)
            var player = TargetManager.instance.Target;
            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            meshParent.transform.forward = player.gameObject.transform.position;

            // �ʻ�� ���� �ִϸ��̼� ���
            //UltAttack.Play();

            // ������ ������ �ʻ������ ������
            _lastAttack = CharacterState.CastUltAttack;
        }
        // ������ �����Ҷ�
        public override void DoAttack()
        {
            base.DoAttack();
            Debug.Log("Enemy Attack");

            // Ÿ������ �����Ѵ� (�÷��̾��)
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Player);
            // �÷��̾� Ÿ���� �����´�
            var player = TargetManager.instance.Target;
            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            meshParent.transform.forward = player.gameObject.transform.position;

            // ���� ������ �����ϴ� ������Ʈ�� �̸� (�׽�Ʈ��)
            Debug.Log(gameObject.name + " �� ����! ");

            // �Ϲݰ��� �ִϸ��̼��� ����
            //normalAttack.Play();
            
            // ������ ������ �Ϲݰ������� ������
            _lastAttack = CharacterState.DoAttack;

            // �ڱ��ڽ��� ĳ���͸� �����´�
            Character ch = GetComponent<Character>();
            // �������� ����ϴ� �Լ��� ȣ���ϰ�
            DamageResult result = CombatManager.DoDamage(ch, player);

            Debug.Log(player.name + " ���� " + result.FinalDamage + " ������");
            // ( �÷��̾��� �������Լ��� ȣ�� �Ǵ� ���⼭ ���)

            // �÷��̾��� ������ �Լ��� �������� �ڽ����� �ϰ� ȣ��
            player.Damage(this);

            // ���ʹ̰� ���� ������ ���� �ٶ󺸰��Ѵ�
            meshParent.transform.forward = EnPosition;

        }

        // ����Ʈ�� ������ �Ҷ�
        public override void DoExtraAttack()
        {
            base.DoExtraAttack();
        }

        #endregion

        #region �غ��ϴ� �Լ� (����, ��ų, �ñر�)

        // ������ �غ��ϴ� �Լ�
        public override void PrepareAttack()
        {
            base.PrepareAttack();
        }
        // ��ų�� �غ��ϴ� �Լ�
        public override void PrepareSkill()
        {
            base.PrepareSkill();
        }
        // �ñر⸦ �غ��ϴ� �Լ�
        public override void PrepareUltAttack()
        {
            base.PrepareUltAttack();
        }

        #endregion


        // Ȥ�� ���� ���� ������ �Լ�
        public override void Damage(Character pl)
        {
            base.Damage(pl);
        }

        // ����� �Լ�
        public override void Dead()
        {
            base.Dead();
            // ĳ���� ���� ��Ȱ��ȭ
            SetVisible(false);

        }


        // �׷α� �Լ�
        public override void Groggy()
        {
            base.Groggy();
            // ���� ���ǵ�� ������ �������� �Ѵ�
            Data.stats.Speed = (Data.stats.Speed) / 2;
            Data.stats.Defense = (Data.stats.Defense) / 2;
        }

        
    }

}