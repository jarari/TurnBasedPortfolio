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
                Debug.Log("데미지를 입히다.");

                // 기본적으로 일반공격으로 해놓는다
                var attackData = Data.AttackTable.normalAttack;

                if (payload == "Skill")
                {
                    attackData = Data.AttackTable.skillAttack;
                    Debug.Log("스킬 데미지");
                }
                else if (payload == "Ult")
                {
                    attackData = Data.AttackTable.ultAttack;
                    Debug.Log("필살기 데미지");
                }
                // 타겟정보를 가져온다
                var targets = TargetManager.instance.GetTargets();
                // 타겟 정보를 순회 하면서
                foreach (var t in targets)
                {
                    // 때린놈 맞은놈 계수를 보내 데미지를 계산한다
                    DamageResult result = CombatManager.CalculateDamage(c, t, attackData);
                    // 에너미에게 대미지를 입힌다
                    t.Damage(c, result);

                    // 이벤트를 실행시킨다
                    OnInflictedDamage?.Invoke(this, t, result);
                }

            }

            // 타임라인에서 디버프 시그널을 받게 된다면
            else if (animEvent == "Radioactivity")
            {
                // 타겟인 에너미와 주변의 에너미도 가져온다
                var targets = TargetManager.instance.GetTargets();

                // 타겟들을 순회하면서
                foreach (var t in targets)
                {
                    // 시전자를 자신으로 하고 에너미에게 디버프를 건다
                    t.GetComponent<CharacterBuffSystem>().ApplyBuff("Radioactivity", this);
                }
            }

            #region 사운드

            else if (animEvent == "PlaySound1")
            {
                SoundManager.instance.PlayVOSound(this,"VanguardNormalAttack1");
            }
            else if (animEvent == "PlaySound2")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardNormalAttack2");
            }

            else if (animEvent == "PlaySound3")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardSkillAttack1");
            }
            else if (animEvent == "PlaySound4")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardSkillAttack2");                
            }

            else if (animEvent == "PlaySound5")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardUltAttack1");
            }
            else if (animEvent == "PlaySound6")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardUltAttack2");
            }
            else if (animEvent == "PlaySound7")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardUltAttack3");
            }

            #endregion

        }

        private void CastUlt()
        {
            // 레이어를 필살리 타임라인으로 맞춘다
            SetMeshLayer(MeshLayer.UltTimeline);

            // 타임라인을 실행한다
            ultAttack.Play();

            // 마지막 공격이 필살기로 잡는다
            _lastAttack = CharacterState.CastUltAttack;

            // 중앙에 있는 에너미를 타겟으로 가져온다
            var enemyCenter = TargetManager.instance.Target;
            // 자신의 위치를 중앙으로 잡는다
            meshParent.transform.position = enemyCenter.transform.position + new Vector3(11.207f, 0f);

            //  모든 에너미를 순회하면서
            foreach (var c in CharacterManager.instance.GetEnemyCharacters())
            {                
                // 레이어를 변경한다
                c.SetMeshLayer(MeshLayer.UltTimeline);
            }
            // 아군 캐릭터는 자신을 제외하고 모두 비활성화 한다
            foreach (var c in CharacterManager.instance.GetAllAllyCharacters())
            {
                if (c != this)
                {
                    c.SetVisible(false);
                }
            }

            // 자신의 레이어를 필살기 타임라인 레이어로 잡는다
            SetMeshLayer(MeshLayer.UltTimeline);
            
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

        #region 공격하는 함수

        public override void CastSkill()
        {
            base.CastSkill();
            // 타겟을 가져온다
            var enemyCenter = TargetManager.instance.Target;
            // 플레이어를 타겟으로 하는 에너미의 중심을 기준으로 위치를 잡고
            meshParent.transform.position = enemyCenter.transform.position + new Vector3(11.207f, 0f);
            // 그 에너미와 주변의 타겟들도 가져온다
            var targets = TargetManager.instance.GetTargets();

            for (int i = 0; i < targets.Count; i++)
            {
                // 에너미의 레이어를 스킬 타임라인으로 맞춘다
                targets[i].SetMeshLayer(MeshLayer.SkillTimeine);
                // 첫번째 에너미가 센터가 아니라면
                if (i == 0 && targets[i] != enemyCenter)
                {
                    // 위치를 잡는다
                    targets[i].meshParent.transform.position = enemyCenter.meshParent.transform.position - new Vector3(0, 0, 4f);
                }
                // 예들도 센터가 아니라면
                else if (i == 1 && targets[i] != enemyCenter || i == 2)
                {
                    // 위치를 잡는다
                    targets[i].meshParent.transform.position = enemyCenter.meshParent.transform.position + new Vector3(0, 0, 4f);
                }
                // 레이어를 스킬 타임라인으로 맞춘다
                SetMeshLayer(MeshLayer.SkillTimeine);
                // 타임라인을 실행한다
                skillAttack.Play();
                // 마지막 공격을 castskill로 잡는다
                _lastAttack = CharacterState.CastSkill;
            }
            
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
            
            // 타겟을 가져와서
            var enemy = TargetManager.instance.Target;
            // 자신의 위치를 에너미 앞으로 잡고
            meshParent.transform.position = enemy.transform.position + new Vector3(11.207f, 0f);
            // 일반공격 타임라인을 실행한다
            normalAttack.Play();
            foreach (var c in CharacterManager.instance.GetEnemyCharacters())
            {
                // 에너미의 레이어를 스킬 타임라인으로 잡는다
                c.SetMeshLayer(MeshLayer.SkillTimeine);
            }
            // 자신의 레이어를 스킬 타임라인 레이어로 잡는다
            SetMeshLayer(MeshLayer.SkillTimeine);
            // 마지막 공격을 doattack으로 잡는다
            _lastAttack = CharacterState.DoAttack;
        }
                
        public override void DoExtraAttack(Character target)
        {
            base.DoExtraAttack(target);
        }

        #endregion

        #region 준비하는 함수

        public override void PrepareAttack()
        {
            base.PrepareAttack();

            Debug.Log("일반 공격 준비");

            // 애니메이터의 int 파라미터를 변경한다
            animator.SetInteger("State", 0);
            // 에너미 하나를 타겟으로 잡는다
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, TurnBased.Data.CharacterTeam.Enemy);
        }

        public override void PrepareSkill()
        {
            base.PrepareSkill();

            // 애니메이터의 int 파라미터를 변경한다
            animator.SetInteger("State", 1);

            Debug.Log("스킬 공격 준비");
            // 에너미 세명을 타겟으로 삼는다
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Triple, TurnBased.Data.CharacterTeam.Enemy);
       
        }

        public override void PrepareUltAttack()
        {
            base.PrepareUltAttack();

            // 애니메이터의 int 파라미터를 변경한다
            animator.SetInteger("State", 2);

            Debug.Log("필살기 준비");
            // 모든 에너미를 타겟으로 삼는다
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Triple, TurnBased.Data.CharacterTeam.Enemy);
        }

        public override void PrepareUltSkill()
        {
            base.PrepareUltSkill();

            // 애니메이터의 int 파라미터를 변경한다
            animator.SetInteger("State", 2);

            // 모든 에너미를 타겟으로 삼는다
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.All, TurnBased.Data.CharacterTeam.Enemy);
        }

        #endregion

        public override void Damage(Character attacker, DamageResult result)
        {
            base.Damage(attacker, result);

            // 애니메이션의 트리거를 켠다
            animator.SetTrigger("Damage");
            // 피격시 소리를 재생한다
            SoundManager.instance.Play2DSound("VanguardDamage");
        }

        public override void Dead()
        {
            base.Dead();
            // 애니메이션의 트리거를 켠다
            animator.SetTrigger("Dead");
        }

        // 카메라 체인지
        public override void ProcessCamChanged()
        {
            if (_lastAttack == CharacterState.DoAttack || _lastAttack == CharacterState.DoExtraAttack)
            {
                normalAttack.time = normalAttack.duration;
                normalAttack.Evaluate();
                normalAttack.Stop();
            }
            else if (_lastAttack == CharacterState.CastSkill)
            {
                skillAttack.time = skillAttack.duration;
                skillAttack.Evaluate();
                skillAttack.Stop();
            }
            else if (_lastAttack == CharacterState.CastUltAttack)
            {
                ultAttack.time = ultAttack.duration;
                ultAttack.Evaluate();
                ultAttack.Stop();
            }
            // 자신의 위치를 바로 잡는다
            meshParent.transform.localPosition = Vector3.zero;
            _lastAttack = CharacterState.Idle;
        }

    }
}
