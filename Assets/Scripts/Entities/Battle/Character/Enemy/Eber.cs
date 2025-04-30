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
    /// ��� ���ʹ�
    /// </summary>
    public class Eber : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // �Ϲ� ���� �ִϸ��̼�
        public PlayableDirector skillAttack;        // ��ų ���� �ִϸ��̼�       
        public PlayableDirector Groggy_anim;    // �׷α� �ִϸ��̼�

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        // ĳ������ ������ ���� ���¸� ���� ����
        private CharacterState _lastAttack;

        // ������ �÷��̾ ���� ����
        public Character target;

        // ������ ���ظ� ���ҽ� �Ϲݰ��ݰ� ��ų ������ ����� ���� ����
        public AttackData Damage_factor;

        public float skill_cool = 0f;

        // ���� ���� �� �ൿ�ϱ� ���� ����
        private bool myTurn = false;


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

            #region ����

            // Ÿ�Ӷ��ο��� ���� �÷��� �ñ׳��� �ް� �ȴٸ� ����
            if (animEvent == "PlaySound_1")
            {
                // ���� �޴����� ��� �س��� ���带 �����Ѵ�
                SoundManager.instance.PlayVOSound(this, "Enemy_Eber_Normal_Attack");
            }
            if (animEvent == "PlaySound_2")
            {
                SoundManager.instance.PlayVOSound(this, "Enemy_Eber_Skill_Attack1");
            }
            if (animEvent == "PlaySound_3")
            {
                SoundManager.instance.PlayVOSound(this, "Enemy_Eber_Skill_Attack2");
            }

            #endregion

            // Ÿ�Ӷ��ο��� ������ ���� ��ȣ�� �ްԵȴٸ� ����
            if (animEvent == "NormalAttackEnd" || animEvent == "SkillAttackEnd")
            {
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
                        animator.gameObject.transform.localPosition = Vector3.zero;

                        // �ڽ��� ���� �������� �˸���
                        myTurn = false;

                        // ���� �����Ѵ�
                        EndTurn();
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

            // �ڽ��� ������ �˸���
            myTurn = true;

            // ���� ���°� �׷α���
            if (this.CurrentState == CharacterState.Groggy)
            {
                // ���� �����ӿ��� ������ �����ϱ����� �ڷ�ƾ�� �����Ѵ�
                StartCoroutine(Groggy_Idle());

                // �ִϸ������� �Ұ��� �����Ѵ�               
                animator.SetBool("GroggyBool", false);

                // ���� ���ε��� �ִ�� �Ѵ�
                this.Data.Toughness.Reset();


            }
            // ���� ���°� �׷αⰡ �ƴ϶��
            else
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

        }

        IEnumerator Groggy_Idle()
        {

            // �׷α� Ÿ�Ӷ����� ������Ų�� ����� ��Ų��
            Groggy_anim.Stop();
            Groggy_anim.Play();

            // �׷α� Ÿ�Ӷ����� �������϶�
            while (Groggy_anim.state == PlayState.Playing)
            {
                // �� ������ ���
                yield return null;
            }

            Debug.Log("�ڷ�ƾ ����");

            // �׷α� Ÿ�Ӷ����� ������ �����Ų��
            Groggy_anim.time = Groggy_anim.duration;
            // Ÿ�Ӷ����� ���� �ð��� �°� ���¸� ������Ʈ
            Groggy_anim.Evaluate();

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
            Debug.Log(this.name + "��ų ����");

            // ���ʹ̰� �÷��̾� �տ� ������ �Ѵ�
            animator.gameObject.transform.position = target.transform.position - new Vector3(8.47f, 0f);

            // ��ų ���� ����� ���
            Damage_factor = Data.AttackTable.skillAttack;

            // ��ų ���� �ִϸ��̼��� �����
            skillAttack.Stop();
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

            Debug.Log(this.name + "�Ϲ� ����!");

            // ���ϸ������� ���Ͱ� ���ʹ̰� �÷��̾� �տ� ������ �Ѵ�            
            animator.gameObject.transform.position = target.transform.position - new Vector3(8.47f, 0f);

            // �Ϲ� ���� ����� ���
            // (���߿� ��������� ������� ����� �̰��� ���ϰų� ���� ��ĵ� �����غ���)
            Damage_factor = Data.AttackTable.normalAttack;

            // �Ϲݰ��� �ִϸ��̼��� �����
            normalAttack.Stop();
            // �Ϲݰ��� �ִϸ��̼��� ����
            normalAttack.Play();

            // ������ ������ �Ϲݰ������� ������
            _lastAttack = CharacterState.DoAttack;

        }

        #endregion


        #region �غ��ϴ� �Լ� (����, ��ų, �ñر�)

        /// <summary>
        /// ������ �غ��ϴ� �Լ�
        /// </summary>
        public override void PrepareAttack()
        {
            // ���� �޾Ҵٸ�
            if (myTurn == true)
            {
                base.PrepareAttack();

                // ������ �ִ� �÷��̾ �����´�
                target = TargetManager.instance.SetPlayerTarget();

                // �����ϴ� �Լ�
                DoAttack();
            }
            // ���� ���� �ʾҴٸ�
            else
                return;
        }
        /// <summary>
        /// ��ų�� �غ��ϴ� �Լ�
        /// </summary>
        public override void PrepareSkill()
        {
            // ���� �޾Ҵٸ�
            if (myTurn == true)
            {
                base.PrepareSkill();

                // ������ �ִ� �÷��̾ �����´�
                target = TargetManager.instance.SetPlayerTarget();

                // ��ų�� ����ϴ� �Լ�
                CastSkill();
            }
            // ���� ���� �ʾҴٸ�
            else
                return;

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
        /// �׷α� �Լ�
        /// </summary>
        public override void Groggy()
        {
            base.Groggy();

            Debug.Log("�׷α� ���� ����");

            // �׷α� �ִϸ��̼� Ʈ���Ÿ� �Ҵ�
            animator.SetBool("GroggyBool", true);

            Debug.Log("ĳ������ ���� ���� : " + this.CurrentState);
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