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

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        // ������ ��ġ�� ȸ������ ���� ����
        private Vector3 EnPosition;
        private Vector3 EnRotate;

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
            // Ÿ�Ӷ��ο��� ������ �ñ׳��� �ް� �ȴٸ� ����
            if (animEvent == "Damage")
            {
                var player = TargetManager.instance.Target;

                // �ڱ��ڽ��� ĳ���͸� �����´�
                Character ch = GetComponent<Character>();
                // �������� ����ϴ� �Լ��� ȣ���ϰ�
                DamageResult result = CombatManager.DoDamage(ch, player);

                Debug.Log(player.name + " ���� " + result.FinalDamage + " ������");

                // �÷��̾��� ������ �Լ��� �������� �ڽ����� �ϰ� ȣ��
                player.Damage(this);                
            }
            // Ÿ�Ӷ��ο��� ������ ���� ��ȣ�� �ްԵȴٸ� ����
            if (animEvent == "NormalAttackEnd" || animEvent == "SkillAttackEnd")
            {                
                // ������ �ִϸ��̼� ó�� �ڷ�ƾ�� ȣ����
                StartCoroutine(DelayReturnFromAttack());

                Debug.Log("�� ����");

                // Ÿ�Ӷ����� ���°� Pause�϶� (����� ���� �Ǿ�����)
                if (normalAttack.state == PlayState.Paused)
                {
                    // ���ʹ��� �θ�ü�� �������� ������� ��ġ�� 0���� �����
                    meshParent.transform.localPosition = Vector3.zero;
                    // �����ϱ� ���� Ʋ���� ȸ������ ������� �����´�
                    meshParent.transform.eulerAngles = EnRotate;

                    Debug.Log("������� ��ġ"+ meshParent.transform.localPosition);
                    Debug.Log("�������� ��ġ"+ meshParent.transform.position);

                    // ���� �����Ѵ�
                    EndTurn();
                }
            }
        }
        protected override void Awake()
        {
            base.Awake();

            // �̺�Ʈ�� �߻����� ��� ����� �Լ��� �߰��Ѵ�
            OnAnimationEvent += OnAnimationEvent_Impl;
        }
        
        // ���� �޾�����
        public override void TakeTurn()
        {
            // �θ� Ŭ�������� TakeTurn ���� �� ����
            base.TakeTurn();

            // �̰� ������ ���� �غ� ���� �����ϰ� ������ �����ϸ� ���Ⱑ ������ �ȴ�
            Debug.Log("���� �޾Ҵ�!");

            
            // ���ε��� 0���� ���
            if (this.Data.stats.CurrentToughness < 0)
            {
                // ���� ���ε��� �ִ�� �Ѵ�
                this.Data.stats.CurrentToughness = this.Data.stats.MaxToughness;                
            }
        }

        #region �ൿ�ϴ� �Լ� (��ų, ����, �ñر�, ����Ʈ�� ����)

        // ��ų�� ����Ҷ�
        public override void CastSkill()
        {
            // �θ� Ŭ�������� CastSkill ������ ����
            base.CastSkill();
            Debug.Log(gameObject.name + " �� ��ų �ߵ�!");
                        
            // �÷��̾� Ÿ���� �����´�
            var player = TargetManager.instance.Target;

            // ���ʹ̰� �÷��̾� �տ� ������ �Ѵ�
            meshParent.transform.position = player.gameObject.transform.position - new Vector3(8.47f, 0f);

            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            meshParent.transform.forward = player.gameObject.transform.position;

            
            // ��ų ���� �ִϸ��̼� ���
            skillAttack.Play();

            // ������ ������ ��ų�������� �˸���
            _lastAttack = CharacterState.CastSkill;
                        

            // �ڱ��ڽ��� ĳ���͸� �����´�
            Character ch = GetComponent<Character>();
            // �������� ����ϴ� �Լ��� ȣ���ϰ�
            DamageResult result = CombatManager.DoDamage(ch, player);

            Debug.Log(player.name + " ���� " + result.FinalDamage + " ������");

            // �÷��̾��� ������ �Լ��� �������� �ڽ����� �ϰ� ȣ��
            player.Damage(this);

        }
      
        // ������ �����Ҷ�
        public override void DoAttack()
        {
            base.DoAttack();
            Debug.Log("Enemy Attack");

            // �÷��̾� Ÿ���� �����´� (1��)
            var player = TargetManager.instance.Target;

            // ���ʹ̰� �÷��̾� �տ� ������ �Ѵ�
            meshParent.transform.position = player.gameObject.transform.position - new Vector3(8.47f,0f);

            // ���ʹ̰� �÷��̾ �ٶ󺸰��Ѵ�
            meshParent.transform.forward = player.gameObject.transform.position;

            // ���� ������ �����ϴ� �ڽ��� �̸� (�׽�Ʈ��)
            Debug.Log(gameObject.name + " �� ����! ");            
            
            // �Ϲݰ��� �ִϸ��̼��� ����
            normalAttack.Play();
            
            // ������ ������ �Ϲݰ������� ������
            _lastAttack = CharacterState.DoAttack;
            
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

            // ���� �޾����� ���ʹ��� ���� ��ġ�� ȸ������ �����Ѵ�            
            EnRotate = meshParent.transform.eulerAngles;

            // Ÿ���� �����Ѵ�
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Player);            
            // �����ϴ� �Լ�
            DoAttack();
        }
        // ��ų�� �غ��ϴ� �Լ�
        public override void PrepareSkill()
        {
            base.PrepareSkill();
            // Ÿ���� �����Ѵ� (�ϴ� ����� ���Ϸ�)
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Player);
            // ��ų�� ����ϴ� �Լ�
            CastSkill();
        }
        // �ñر⸦ �غ��ϴ� �Լ�
        public override void PrepareUltAttack()
        {
            base.PrepareUltAttack();
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

            // ������ �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Damage");

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

            // ��� ���ϸ��̼��� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Dead");
            // ĳ���� ���� ��Ȱ��ȭ
            SetVisible(false);
            
        }


        // �׷α� �Լ�
        public override void Groggy()
        {
            base.Groggy();
            // �׷α� �ִϸ��̼� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Groggy");

            // ���� ���ǵ�� ������ �������� �Ѵ�
            Data.stats.Speed = (Data.stats.Speed) / 2;
            Data.stats.Defense = (Data.stats.Defense) / 2;
        }

    }



}