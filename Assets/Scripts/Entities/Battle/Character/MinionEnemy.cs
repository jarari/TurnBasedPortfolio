using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;


namespace TurnBased.Entities.Battle { 
    
    /// <summary> 
    /// �Ϲ� ����
    /// </summary>
    public class MinionEnemy : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // �Ϲ� ���� �ִϸ��̼�
        public PlayableDirector skillAttack;        // ��ų ���� �ִϸ��̼�       

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        // ������ ȸ������ ���� ����        
        private Vector3 EnRotate;

        // ĳ������ ������ ���� ���¸� ���� ����
        private CharacterState _lastAttack;
                
        // ������ ���ظ� ���ҽ� �Ϲݰ��ݰ� ��ų ������ ����� ���� ����
        public float Damage_factor = 0f;

        /// <summary> 
        /// �����Ŀ� �ִϸ��̼��� ���� ���� ��ȯ�� ó���ϴ� �ڷ�ƾ
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// �ִϸ��̼� �̺�Ʈ�� �߻�������� ó���ϴ� �Լ�
        /// </summary>
        /// <param name="c"></param>
        /// <param name="animEvent"></param>
        /// <param name="payload"></param>
        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload)
        {
            // Ÿ�Ӷ��ο��� ������ �ñ׳��� �ް� �ȴٸ� ����
            if (animEvent == "Damage")
            {
                var player = TargetManager.instance.Target;

                // �������� ����ϴ� �Լ��� ȣ���ϰ�
                DamageResult result = CombatManager.CalculateDamage(this, player, 1.5f);

                Debug.Log(player.name + " ���� " + result.FinalDamage + " ������");

                // �÷��̾��� ������ �Լ��� �������� �ڽ����� �ϰ� ȣ��
                player.Damage(this, result);                
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
        
        /// <summary>
        /// ���� �޾�����
        /// </summary>
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
                                    
            // ������ �غ��ϴ� �Լ��� �����Ѵ�
            PrepareAttack();
                        
        }

        #region �ൿ�ϴ� �Լ� (��ų, ����, �ñر�, ����Ʈ�� ����)

        /// <summary>
        /// ��ų�� ����Ҷ�
        /// </summary>
        public override void CastSkill()
        {
            // �θ� Ŭ�������� CastSkill ������ ����
            base.CastSkill();
            Debug.Log("Enemy SkillAttack");

            // �÷��̾� Ÿ���� �����´�
            var player = TargetManager.instance.Target;

            // ���ʹ̰� �÷��̾� �տ� ������ �Ѵ�
            meshParent.transform.position = player.gameObject.transform.position - new Vector3(8.47f, 0f);

            // ��ų ���� ����� ���
            Damage_factor = 1.5f;
            
            // ��ų ���� �ִϸ��̼� ���
            skillAttack.Play();

            // ������ ������ ��ų�������� �˸���
            _lastAttack = CharacterState.CastSkill;

        }
      
        /// <summary>
        /// ������ �����Ҷ�
        /// </summary>
        public override void DoAttack()
        {
            base.DoAttack();
            Debug.Log("Enemy Attack");

            // �÷��̾� Ÿ���� �����´� (1��)
            var player = TargetManager.instance.Target;

            // ���ʹ̰� �÷��̾� �տ� ������ �Ѵ�
            meshParent.transform.position = player.gameObject.transform.position - new Vector3(8.47f,0f);

            // �Ϲ� ���� ����� ���
            // (���߿� ��������� ������� ����� �̰��� ���ϰų� ���� ��ĵ� �����غ���)
            Damage_factor = 1.0f;

            // �Ϲݰ��� �ִϸ��̼��� ����
            normalAttack.Play();
            
            // ������ ������ �Ϲݰ������� ������
            _lastAttack = CharacterState.DoAttack;
            
        }

        /// <summary>
        /// ����Ʈ�� ������ �Ҷ�
        /// </summary>
        public override void DoExtraAttack()
        {
            base.DoExtraAttack();
        }

        #endregion


        #region �غ��ϴ� �Լ� (����, ��ų, �ñر�)

        /// <summary>
        /// ������ �غ��ϴ� �Լ�
        /// </summary>
        public override void PrepareAttack()
        {
            base.PrepareAttack();
                        
            // Ÿ���� �����Ѵ�
            TargetManager.instance.SetPlayerTarget();

            // ������ ȸ������ �����Ѵ�
            EnRotate = meshParent.transform.eulerAngles;

            // �����ϴ� �Լ�
            DoAttack();
        }
        /// <summary>
        /// ��ų�� �غ��ϴ� �Լ�
        /// </summary>
        public override void PrepareSkill()
        {
            base.PrepareSkill();

            // Ÿ���� �����Ѵ� (�ϴ� ����� ���Ϸ�)
            TargetManager.instance.SetPlayerTarget();

            // ������ ȸ������ �����Ѵ�
            EnRotate = meshParent.transform.eulerAngles;

            // ��ų�� ����ϴ� �Լ�
            CastSkill();
        }
        /// <summary>
        /// �ñر⸦ �غ��ϴ� �Լ�
        /// </summary>
        public override void PrepareUltAttack()
        {
            base.PrepareUltAttack();
        }

        #endregion

        /// <summary>
        /// Ȥ�� ���� ���� ������ �Լ� (���� ���� ������ �����´�)
        /// </summary>
        /// <param name="attacker">������</param>
        /// <param name="result"></param>
        public override void Damage(Character attacker, DamageResult result) 
        {
            // �θ� Ŭ������ Dagage�� ������ ����
            base.Damage(attacker, result);

            // ������ �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Damage");

            // ���� ä���� 0���ϰ� �Ǿ��ٸ�
            if (Data.stats.CurrentHP <= 0)
            {
                // ��� ���ϸ��̼��� Ʈ���Ÿ� �Ҵ�
                animator.SetTrigger("Dead");
            }
        }

        /// <summary>
        /// �׷α� �Լ�
        /// </summary>
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