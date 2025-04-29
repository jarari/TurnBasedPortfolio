using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;
using NUnit.Framework;
using System.Collections.Generic;

namespace TurnBased.Entities.Battle
{
    /// <summary> 
    /// ����
    /// </summary>
    public class EberBird_Boss : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // �Ϲ� ���� �ִϸ��̼�
        public PlayableDirector skillAttack;        // ��ų ���� �ִϸ��̼�       
        public PlayableDirector Groggy_anim;    // �׷α� �ִϸ��̼�
        public PlayableDirector Rampage_anim;   // ����ȭ �ִϸ��̼�

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        // ĳ������ ������ ���� ���¸� ���� ����
        private CharacterState _lastAttack;

        // ������ �÷��̾ ���� ����
        public Character target;
        // �������ִ� �÷��̾���� ���� ����Ʈ
        public List<Character> M_targets;
        // ������ �÷��̾���� �߾��� ���� ����
        Vector3 Center;

        // ������ ���ظ� ���ҽ� �Ϲݰ��ݰ� ��ų ������ ����� ���� ����
        public float Damage_factor = 0f;

        public float skill_cool = 0f;

        // ���� ���� �� �ൿ�ϱ� ���� ����
        private bool myTurn = false;

        #region ������ ���� (����ȭ, ä�� ����)

        // ���ʹ��� ���� (�븻, ����ȭ)
        public enum BossState { Normal, Rampage}
        // ��� ���� ����
        public BossState b_State;        

        // ������ ����ȭ ���¸� ��Ÿ���� �Ұ�
        bool ram = false;

        // ����ȭ�� ��ƼŬ ������Ʈ�� ���� ����
        public GameObject ramObj;

        #endregion

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
            if (animEvent == "Normal_Damage")
            {
                // �������� ����ϴ� �Լ��� ȣ���ϰ�
                DamageResult result = CombatManager.CalculateDamage(this, target, Damage_factor);

                // �÷��̾��� ������ �Լ��� �������� �ڽ����� �ϰ� ȣ��
                target.Damage(this, result);
            }
            else if (animEvent == "Skill_Damage")
            {
                // for������ �޾ƿ� �÷��̾� ����Ʈ�� ��ȸ�ϸ�
                for(int i =0; i< M_targets.Count; i++)
                {
                    // �������� ����ϴ� �Լ��� ȣ���ϰ�
                    DamageResult result = CombatManager.CalculateDamage(this, M_targets[i], Damage_factor);

                    // �÷��̾��� ������ �Լ��� �������� �ڽ����� �ϰ� ȣ��
                    M_targets[i].Damage(this, result);

                    Debug.Log(M_targets[i].name + " ���� ������");
                }

            }


            #region ����

            // Ÿ�Ӷ��ο��� ���� �÷��� �ñ׳��� �ް� �ȴٸ� ����
            if (animEvent == "PlaySound_1")
            {
                // ���� �޴����� ��� �س��� ���带 �����Ѵ�
                SoundManager.instance.PlayVOSound(this, "Enemy_EberBird_Normal_Attack");
            }
            if (animEvent == "PlaySound_2")
            {
                SoundManager.instance.PlayVOSound(this, "Enemy_EberBird_Skill_Attack1");
            }
            if (animEvent == "PlaySound_3")
            {
                SoundManager.instance.PlayVOSound(this, "Enemy_EberBird_Rampage");
            }

            #endregion

            // Ÿ�Ӷ��ο��� ������ ���� ��ȣ�� �ްԵȴٸ� ����
            if (animEvent == "NormalAttackEnd" || animEvent == "SkillAttackEnd")
            {
                // ������ �ִϸ��̼� ó�� �ڷ�ƾ�� ȣ����
                StartCoroutine(DelayReturnFromAttack());


                // ��ų ��Ÿ���� ������Ų��
                skill_cool++;

                // ��ų ��Ÿ���� 3�� �ʰ��Ͽ�����
                if (skill_cool > 3)
                {
                    // ��ų ��Ÿ���� 0���� �����
                    skill_cool = 0;
                }

                // Ÿ�Ӷ����� ���°� Pause�϶� (����� ���� �Ǿ�����)
                if (normalAttack.state == PlayState.Paused || skillAttack.state == PlayState.Paused)
                {
                    // �Ϲݰ��� �Ǵ� ��ų���� �ִϸ��̼��� �����ٸ�
                    if (normalAttack.time >= normalAttack.duration || skillAttack.time >= skillAttack.duration)
                    {
                        // ���ʹ��� �θ�ü�� �������� ������� ��ġ�� 0���� �����
                        animator.gameObject.transform.localPosition = Vector3.zero;

                        // �ڽ��� ���� �������� �˸���
                        myTurn = false;

                        Debug.Log("�� ����");
                        
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

            // ===== ����ȭ ========
            // ������ ���¸� �븻�� �Ѵ�
            b_State = BossState.Normal;

            // ����ȭ ��ƼŬ�� ��Ȱ��ȭ �Ѵ�
            ramObj.SetActive(false);
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
                // ��ų ��Ÿ���� 3�̻� �̸�
                if (skill_cool >= 3)
                {
                    // ��ų�� �غ��ϴ� �Լ��� �����Ѵ�
                    PrepareSkill();
                }
                // ��ų ��Ÿ���� 3�̸� �̶��
                else if (skill_cool < 3)
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

            

            // ���ʹ̰� �÷��̾���� �߾ӿ� ������ �Ѵ�
            animator.gameObject.transform.position = Center - new Vector3(8.47f, 0f);

            // ��ų ���� ����� ���
            Damage_factor = 0.25f;

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
            Damage_factor = 0.45f;

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

                // �߾� ���Ͱ��� �ʱ�ȭ �Ѵ�
                Center = Vector3.zero;

                // ������ �ִ� �÷��̾���� ����Ʈ�� �����´�
                M_targets = TargetManager.instance.SetMPlayerTarget();
                // ����Ʈ�� ��ȸ�ϸ鼭
                foreach (var players in M_targets)
                {
                    // ������ �÷��̾���� ���Ͱ��� ��� ���Ѵ�
                    Center += players.transform.position;
                }
                // ����Ʈ�� �÷��̾ �����Ѵٸ�
                if (M_targets.Count > 0)
                { 
                    // ��� �� �Ͽ��� ���Ͱ��� ������ �÷��̾� ���ڸ�ŭ ������ �߾ӿ� �ش��ϴ� ���Ͱ��� ���Ѵ�
                    Center /= M_targets.Count;
                }
                
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
                                
                // ���� ����ȭ ���°� �ƴϸ鼭 ä���� �ִ� ä���� ���� ���϶��
                if (ram == false && this.Data.HP.Current <= (this.Data.HP.CurrentMax) / 2)
                {
                    // ���¸� ����ȭ�� �ٲ۴�
                    b_State = BossState.Rampage;

                    this.GetComponent<CharacterBuffSystem>().ApplyBuff("Rampage_Buff", this);
                    
                    // ���� ���°� �׷α� ���¶��
                    if (this.CurrentState == CharacterState.Groggy)
                    {
                        // ����ȭ �ִϸ��̼��� ����� �ڷ�ƾ�� ����
                        StartCoroutine(StartRam());

                        // ���ʹ��� ���� ���¸� �⺻���� �Ѵ�
                        this.CurrentState = CharacterState.Idle;

                        animator.SetBool("GroggyBool", false);

                        Debug.Log("����ȭ�� �׷α� ���¿��� ���ε��� ȸ��");
                    }

                    // ����ȭ �������� �˸���
                    ram = true;
                    Debug.Log("����ȭ ���� ����");
                }
            }
            Debug.Log("�������� �Ծ���");
        }

        // ����ȭ �ڷ�ƾ
        IEnumerator StartRam()
        {
            // ������ ���� ����ȭ �ִϸ��̼��� �����Ѵ�
            Rampage_anim.Stop();
            Rampage_anim.Play();

            // ����ȭ Ÿ�Ӷ����� �������϶�
            while (Rampage_anim.state == PlayState.Playing)
            { 
                // �� ������ ���
                yield return null;
            }

            Debug.Log("����ȭ �ڷ�ƾ ����");

            // ����ȭ Ÿ�Ӷ����� ������ �����Ų��
            Rampage_anim.time = Rampage_anim.duration;
            // Ÿ�Ӷ����� ���� �ð��� �°� ���¸� ������Ʈ
            Rampage_anim.Evaluate();

            // ����ȭ�� ��ƼŬ�� Ȱ��ȭ �Ѵ�
            ramObj.SetActive(true); 
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

            // ����ȭ�� ��ƼŬ�� ��Ȱ��ȭ �Ѵ�
            ramObj.SetActive(false);

            // ���� �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Dead");
        }
    }
}