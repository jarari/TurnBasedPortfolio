using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;

namespace TurnBased.Entities.Battle
{
    /// <summary> 
    /// �ܰ��� ����
    /// </summary>
    public class Alien_Soldier : Character
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

        // ������ �÷��̾ ���� ����
        public Character target;

        // ������ ���ظ� ���ҽ� �Ϲݰ��ݰ� ��ų ������ ����� ���� ����
        public float Damage_factor = 0f;

        public float skill_cool = 0f;

        // �� ���� �ߺ� ���� ����
        private bool isTurnEnd = false;

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
                // �������� ����ϴ� �Լ��� ȣ���ϰ�
                DamageResult result = CombatManager.CalculateDamage(this, target, Damage_factor);

                // �÷��̾��� ������ �Լ��� �������� �ڽ����� �ϰ� ȣ��
                target.Damage(this, result);
            }
            // Ÿ�Ӷ��ο��� ������ ���� ��ȣ�� �ްԵȴٸ� ����
            if (animEvent == "NormalAttackEnd" || animEvent == "SkillAttackEnd")
            {
                // �ߺ� ó�� ����
                isTurnEnd = true;

                // ������ �ִϸ��̼� ó�� �ڷ�ƾ�� ȣ����
                StartCoroutine(DelayReturnFromAttack());

                Debug.Log("�� ����");

                // ��ų ��Ÿ���� ������Ų��
                skill_cool++;

                // ��ų ��Ÿ���� 2�� �ʰ��Ͽ�����
                if (skill_cool > 2)
                {
                    // ��ų ��Ÿ���� 0���� �����
                    skill_cool = 0;
                }

                // Ÿ�Ӷ����� ���°� Pause�϶� (����� ���� �Ǿ�����)
                if (normalAttack.state == PlayState.Paused)
                {
                    // �Ϲݰ��� �ִϸ��̼��� �����ٸ�
                    if (normalAttack.time >= normalAttack.duration)
                    {
                        // ���ʹ��� �θ�ü�� �������� ������� ��ġ�� 0���� �����
                        meshParent.transform.localPosition = Vector3.zero;
                        // �����ϱ� ���� Ʋ���� ȸ������ ������� �����´�
                        meshParent.transform.eulerAngles = EnRotate;

                        // ���� �����Ѵ�
                        EndTurn();
                        // �ߺ����� �ʱ�ȭ
                        isTurnEnd = false;
                    }


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

            // Ÿ���� �ʱ�ȭ �Ѵ�
            target = null;

            // ���� ���°� �׷α���
            if (this.CurrentState == CharacterState.Groggy)
            {
                // �ִϸ������� Ʈ���Ÿ� �Ҵ�
                animator.SetTrigger("GroggyToIdle");

                // ���� ���ε��� �ִ�� �Ѵ�
                this.Data.stats.CurrentToughness = this.Data.stats.MaxToughness;

                // ���� ���¸� �⺻���� �����Ѵ�
                this.CurrentState = CharacterState.Idle;

                // ������ �Լ��� �����Ѵ�
                AttackStart();
            }
            // ���� ���°� �׷αⰡ �ƴ϶��
            else
            {
                // ������ �Լ��� �����Ѵ�
                AttackStart();
            }

        }

        /// <summary>
        /// ��ų ��Ÿ�ӿ� ���� ������ �� �Լ�
        /// </summary>
        private void AttackStart()
        {
            // ��ų ��Ÿ���� 2�̻� �̸�
            if (skill_cool >= 2)
            {
                // ��ų�� �غ��ϴ� �Լ��� �����Ѵ�
                PrepareSkill();
            }
            // ��ų ��Ÿ���� 2�̸� �̶��
            else if (skill_cool < 2)
            {
                // ������ �غ��ϴ� �Լ��� �����Ѵ�
                PrepareAttack();
            }
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

            // ���ʹ̰� �÷��̾� �տ� ������ �Ѵ�
            meshParent.transform.position = target.gameObject.transform.position - new Vector3(8.47f, 0f);

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
            Debug.Log("���� ä�� : " + this.Data.stats.CurrentHP);

            // ���ʹ̰� �÷��̾� �տ� ������ �Ѵ�
            meshParent.transform.position = target.gameObject.transform.position - new Vector3(8.47f, 0f);

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

            // ������ �ִ� �÷��̾ �����´�
            target = TargetManager.instance.SetPlayerTarget();

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

            // ������ �ִ� �÷��̾ �����´�
            target = TargetManager.instance.SetPlayerTarget();

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

            // ĳ������ ���°� dead���°� �ƴҶ�
            if (this.CurrentState != CharacterState.Dead)
            {
                // ������ �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
                animator.SetTrigger("Damage");
            }

            Debug.Log("�������� �Ծ���");

        }

        /// <summary>
        /// �׷α� �غ��Լ�
        /// </summary>
        public override void PrepareGroggy()
        {
            base.PrepareGroggy();

        }


        /// <summary>
        /// �׷α� �Լ�
        /// </summary>
        public override void Groggy()
        {
            base.Groggy();

            Debug.Log("�׷α� ���� ����");

            // �׷α� �ִϸ��̼� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Groggy");

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log("���� �ִϸ��̼� ���� : " + state.fullPathHash + " , �̸� : " + state.IsName("Groggy"));
            Debug.Log("ĳ������ ���� ���� : " + this.CurrentState);

            // ���� ���ǵ�� ������ �������� �Ѵ�
            Data.stats.Speed = (Data.stats.Speed) / 2;
            Data.stats.Defense = (Data.stats.Defense) / 2;
        }

        /// <summary>
        /// ����� ȣ��Ǵ� �Լ�
        /// </summary>
        public override void Dead()
        {
            base.Dead();

            Debug.Log("���� ���� �ִϸ��̼� ����");

            // ���� �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Dead");
        }

    }
}