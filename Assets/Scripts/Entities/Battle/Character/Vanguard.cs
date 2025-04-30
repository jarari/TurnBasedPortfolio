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
        public PlayableDirector normalAttack;   // 기본 공격
        public PlayableDirector skillAttack;    // 스킬 공격
        public PlayableDirector ultAttack;      // 필살기

        [Header("AttackObjects")]
        public GameObject attackObject_1;     // 공격에 사용될 오브젝트 1
        

        [Header("Components")]
        public Animator animator;   // 캐릭터의 애니메이터

        // 공격할 에너미를 담을 함수
        public Character e_target;

        // 가할 데미지의 계수
        float attackMult = 0f;

        // 마지막 공격 상태를 담을 변수
        private CharacterState _lastAttack;
        
        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload)
        {
            // 공격이 종료 되었다는 신호를 받았다면
            if (animEvent == "AttackEnd")
            {
                // 턴을 종료한다
                EndTurn();
            }
            // 타임라인에서 데미지 시그널을 받게 된다면
            else if (animEvent == "Damage")
            {
                // 일반공격의 데미지 계수를 1.0으로 한다
                attackMult = 1.0f;
                
                // 공격이 필살기라면 3.0으로 계수를 맞춘다
                if (payload == "Ult")
                {
                    attackMult = 3.0f;
                }

                // 데미지를 계산하는 함수를 호출하고
                DamageResult result = CombatManager.CalculateDamage(c, e_target, attackMult);

                // 플레이어의 데미지 함수에 때린놈을 자신으로 하고 호출
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

        #region 공격하는 함수

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

        /* 생각해볼 엑스트라어택
        public override void DoExtraAttack(Character target)
        {
            base.DoExtraAttack(target);

            _lastAttack = CharacterState.DoExtraAttack;
        }
        */

        #region 준비하는 함수

        public override void PrepareAttack()
        {
            base.PrepareAttack();
            // 에너미 하나를 타겟으로 잡는다
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
