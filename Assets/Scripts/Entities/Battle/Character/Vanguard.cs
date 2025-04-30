using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using UnityEngine.Playables;
using Unity.Cinemachine;
using UnityEngine.VFX;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;

namespace TurnBased.Entities.Battle
{
    public class Vanguard : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;   // �⺻ ����
        public PlayableDirector skillAttack;    // ��ų ����
        public PlayableDirector ultAttack;      // �ʻ��

        [Header("AttackObjects")]
        public GameObject attackObject_1;     // ���ݿ� ���� ������Ʈ 1
        

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        // ������ ���ʹ̸� ���� �Լ�
        public Character e_target;

        // ���� �������� ���
        float attackMult = 0f;

        // ������ ���� ���¸� ���� ����
        private CharacterState _lastAttack;
        
        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload)
        {
            // ������ ���� �Ǿ��ٴ� ��ȣ�� �޾Ҵٸ�
            if (animEvent == "AttackEnd")
            {
                // ���� �����Ѵ�
                EndTurn();
            }
            // Ÿ�Ӷ��ο��� ������ �ñ׳��� �ް� �ȴٸ�
            else if (animEvent == "Damage")
            {
                // �Ϲݰ����� ������ ����� 1.0���� �Ѵ�
                attackMult = 1.0f;
                
                // ������ �ʻ���� 3.0���� ����� �����
                if (payload == "Ult")
                {
                    attackMult = 3.0f;
                }

                // �������� ����ϴ� �Լ��� ȣ���ϰ�
                DamageResult result = CombatManager.CalculateDamage(c, e_target, attackMult);

                // �÷��̾��� ������ �Լ��� �������� �ڽ����� �ϰ� ȣ��
                e_target.Damage(this, result);
            }
            
        }

        private void CastUlt()
        {
            ultAttack.Play();
           
        }

        protected override void Awake()
        {
            base.Awake();
            OnAnimationEvent += OnAnimationEvent_Impl;
        }

        protected override void Start()
        {
            base.Start();
            
        }

        public override void TakeUltTurn()
        {
            base.TakeUltTurn();
           
        }

        #region �����ϴ� �Լ�

        public override void CastSkill()
        {
            base.CastSkill();
            
        }

        public override void CastUltAttack()
        {
            base.CastUltAttack();
            CastUlt();
        }

        public override void CastUltSkill()
        {
            base.CastUltSkill();
            CastUlt();
        }

        public override void DoAttack()
        {
            base.DoAttack();
         
            var enemy = TargetManager.instance.Target;
            Vector3 diff = enemy.transform.position - transform.position;
            diff.y = 0;
            diff.Normalize();
            meshParent.transform.forward = diff;
            normalAttack.Play();
            _lastAttack = CharacterState.DoAttack;
        }

        #endregion

        /* �����غ� ����Ʈ�����
        public override void DoExtraAttack(Character target)
        {
            base.DoExtraAttack(target);

            _lastAttack = CharacterState.DoExtraAttack;
        }
        */

        #region �غ��ϴ� �Լ�

        public override void PrepareAttack()
        {
            base.PrepareAttack();
            // ���ʹ� �ϳ��� Ÿ������ ��´�
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, TurnBased.Data.CharacterTeam.Enemy);
        }

        public override void PrepareSkill()
        {
            base.PrepareSkill();
       
        }

        public override void PrepareUltAttack()
        {
            base.PrepareUltAttack();
           
        }

        public override void PrepareUltSkill()
        {
            base.PrepareUltSkill();
        }

        #endregion

        public override void Damage(Character attacker, DamageResult result)
        {
            base.Damage(attacker, result);
          
        }

        public override void Dead()
        {
            base.Dead();
          
        }

        public override void ProcessCamChanged()
        {
            if (_lastAttack == CharacterState.DoAttack || _lastAttack == CharacterState.DoExtraAttack)
            {
                normalAttack.time = normalAttack.duration;
                normalAttack.Evaluate();                
            }
            else if (_lastAttack == CharacterState.CastSkill)
            {
                skillAttack.time = skillAttack.duration;
                skillAttack.Evaluate();
            }
            else if (_lastAttack == CharacterState.CastUltAttack)
            {
                ultAttack.time = ultAttack.duration;
                ultAttack.Evaluate();
            }
        }

        public override void ProcessCamGain()
        {
            
        }
    }
}
