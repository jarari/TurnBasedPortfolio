using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;


namespace TurnBased.Entities.Battle { 
    
    // �Ϲ� ����
    public class MinionEnemy : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // �Ϲ� ���� �ִϸ��̼�
        public PlayableDirector skillAttack;        // ��ų ���� �ִϸ��̼�        
        public PlayableDirector Groggy_anim;             // �׷α� �ִϸ��̼�
        public PlayableDirector Dead_anim;             // ����ִϸ��̼�

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        // Ÿ������ �÷��̾ ���� ���� (�̰� Ÿ���� �Ǵ��� Ȯ���� �������� public�� ���)
        public Character ch_player;
        public Character[] ch_players;

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
        }

        // �ִϸ��̼� �̺�Ʈ�� �߻�������� ó���ϴ� �Լ�
        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload)
        {
            if (animEvent == "NomalAttackEnd" || animEvent == "SkillAttackEnd")
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
          
            // ���۽� ���ʹ��� ���� ��ġ�� ȸ������ �����Ѵ�
            EnPosition = transform.position;
            EnRotate = transform.eulerAngles;
        }

        // ���� �޾�����
        public override void TakeTurn()
        {
            // �θ� Ŭ�������� TakeTurn ���� �� ����
            base.TakeTurn();

            // ���ε��� �ִٸ�
            if (this.Data.stats.CurrentToughness > 0)
            {
                // �����ϴ� �Լ�
                DoAttack();                

                // ���� ������
                EndTurn();
            }
            // ���ε��� 0���� ���
            else
            {
                // ���� ���ε��� �ִ�� �Ѵ�
                this.Data.stats.CurrentToughness = this.Data.stats.MaxToughness;

                // �����ϴ� �Լ�
                DoAttack();                
                
                // ���� ������
                EndTurn();
            }

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
            // �÷��̾� Ÿ���� �����´�
            var player = TargetManager.instance.Target;
           
            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            meshParent.transform.forward = player.gameObject.transform.position;

            // ��ų ���� �ִϸ��̼� ���
            //skillAttack.Play();

            // ������ ������ ��ų�������� �˸���
            _lastAttack = CharacterState.CastSkill;
        }
      
        // ������ �����Ҷ�
        public override void DoAttack()
        {
            base.DoAttack();
            Debug.Log("Enemy Attack");

            // Ÿ������ �����Ѵ� (�÷��̾��)
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Player);
            // �÷��̾� Ÿ���� �����´� (1��)
            var player = TargetManager.instance.Target;

            Debug.Log(player);

            // ���ʹ̰� �÷��̾� �տ� ������ �Ѵ� (������ Mesh�� x : 2, y : 0, z:8 �� ���� �ȴ�)
            meshParent.transform.position = player.gameObject.transform.position - new Vector3(8.47f,0f);

            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            meshParent.transform.forward = player.gameObject.transform.position;

            // ���� ������ �����ϴ� �ڽ��� �̸� (�׽�Ʈ��)
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
            // ������...
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Player);
        }
        // ��ų�� �غ��ϴ� �Լ�
        public override void PrepareSkill()
        {
            base.PrepareSkill();
        }
        // �ñر⸦ �غ��ϴ� �Լ�
        public override void PrepareUlt()
        {
            base.PrepareUlt();
        }

        #endregion

        // Ȥ�� ���� ���� ������ �Լ� (���� ���� ������ �����´�)
        // IDamageApply�� ��ӽ��� �����Դ�
        public override void Damage(Character pl)
        {
            // �θ� Ŭ������ DagageApply�� ������ ����
            base.Damage(pl);

            // �ڱ��ڽ��� ĳ���͸� �����´�
            Character ch = GetComponent<Character>();
            // �������� ����ϴ� �Լ��� ȣ�� (������, ������)
            DamageResult result = CombatManager.DoDamage(pl, ch);

            // ���� ���ε��� �ִٸ�
            if (ch.Data.stats.CurrentToughness > 0)
            {               

                // ä���� ���������� �޴� �������� ������ �ް�
                this.Data.stats.CurrentHP -= (result.FinalDamage / 2);

                // ä���� ���� 0���ϰ� �Ǿ��ٸ�
                if (ch.Data.stats.CurrentHP <= 0)
                {
                    // ������ �ٷ� �Լ��� �����Ѵ�
                    Dead();
                }

                // �÷��̾ ���� �Ӽ����� �����ٸ�
                if (Element_Check(pl, ch) == true)
                {
                    // ���ʹ��� ���ε��� �÷��̾��� ���ݷ¸�ŭ ���δ�
                    this.Data.stats.CurrentToughness -= result.NormalAttack;

                    // ���ε��� ���� 0���ϰ� �Ǿ��ٸ�
                    if (ch.Data.stats.CurrentToughness <= 0)
                    {
                        // ���� ���ε��� 0���� �����
                        ch.Data.stats.CurrentToughness = 0;
                        // �׷α⸦ �ٷ� �Լ��� �����Ѵ�
                        Groggy();
                    }

                }

            }
            // ���� ���ε��� ���ٸ�
            else
            {
                // ���ʹ̴� ���������� �޴� �������� ��� �޴´�.
                this.Data.stats.CurrentHP -= result.FinalDamage;

                // ä���� ���� 0���ϰ� �Ǿ��ٸ�
                if (ch.Data.stats.CurrentHP <= 0)
                {
                    // ������ �ٷ� �Լ��� �����Ѵ�
                    Dead();
                }

            }
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