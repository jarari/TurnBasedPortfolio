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
    /// 기계 에너미
    /// </summary>
    public class Eber : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // 일반 공격 애니메이션
        public PlayableDirector skillAttack;        // 스킬 공격 애니메이션       
        public PlayableDirector Groggy_anim;    // 그로기 애니메이션

        [Header("Components")]
        public Animator animator;   // 캐릭터의 애니메이터

        // 캐릭터의 마지막 공격 상태를 담을 변수
        private CharacterState _lastAttack;

        // 공격할 플레이어를 담을 변수
        public Character target;

        // 데미지 피해를 가할시 일반공격과 스킬 데미지 계수를 담을 변수
        public AttackData Damage_factor;

        public float skill_cool = 0f;

        // 턴을 받은 뒤 행동하기 위한 변수
        private bool myTurn = false;


        /// <summary> 
        /// 공격후에 애니메이션이 끝날 때의 반환을 처리하는 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelayReturnFromAttack()
        {
            // 일시 정지 없이 다음 프레임에서 실행함
            yield return null;
            // 마지막 공격 상태가 DoAttack 일 경우
            if (_lastAttack == CharacterState.DoAttack)
            {
                // 일반 공격 애니메이션을 끝까지 진행시킨다
                normalAttack.time = normalAttack.duration;
                // 타임라인을 현재 시간에 맞게 상태를 업데이트
                normalAttack.Evaluate();
            }
            // 마지막 공격 상태가 CastSkill 일 경우
            else if (_lastAttack == CharacterState.CastSkill)
            {
                // 스킬 공격 애니메이션을 끝까지 진행시킨다
                skillAttack.time = skillAttack.duration;
                // 타임라인을 현재 시간에 맞게 상태를 업데이트
                skillAttack.Evaluate();
            }
        }

        /// <summary>
        /// 애니메이션 이벤트가 발생했을경우 처리하는 함수
        /// </summary>
        /// <param name="c"></param>
        /// <param name="animEvent"></param>
        /// <param name="payload"></param>
        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload)
        {
            // 타임라인에서 데미지 시그널을 받게 된다면 실행
            if (animEvent == "Damage")
            {
                // 데미지를 계산하는 함수를 호출하고
                DamageResult result = CombatManager.CalculateDamage(this, target, Damage_factor);

                // 플레이어의 데미지 함수에 때린놈을 자신으로 하고 호출
                target.Damage(this, result);
            }

            #region 사운드

            // 타임라인에서 사운드 플레이 시그널을 받게 된다면 실행
            if (animEvent == "PlaySound_1")
            {
                // 사운드 메니저에 등록 해놓은 사운드를 실행한다
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

            // 타임라인에서 공격이 끝난 신호를 받게된다면 실행
            if (animEvent == "NormalAttackEnd" || animEvent == "SkillAttackEnd")
            {
                // 공격후 애니메이션 처리 코루틴을 호출후
                StartCoroutine(DelayReturnFromAttack());

                Debug.Log("턴 종료");

                // 스킬 쿨타임을 증가시킨다
                skill_cool++;

                // 스킬 쿨타임이 2을 초과하였을때
                if (skill_cool > 2)
                {
                    // 스킬 쿨타임을 0으로 만든다
                    skill_cool = 0;
                }

                // 타임라인의 상태가 Pause일때 (재생이 종료 되었을때)
                if (normalAttack.state == PlayState.Paused)
                {
                    // 일반공격 애니메이션이 끝났다면
                    if (normalAttack.time >= normalAttack.duration)
                    {
                        // 에너미의 부모객체를 기준으로 상대적인 위치를 0으로 맞춘다
                        animator.gameObject.transform.localPosition = Vector3.zero;

                        // 자신의 턴이 끝났음을 알린다
                        myTurn = false;

                        // 턴을 종료한다
                        EndTurn();
                    }


                }
            }
        }
        protected override void Awake()
        {
            base.Awake();

            // 이벤트가 발생했을 경우 실행될 함수를 추가한다
            OnAnimationEvent += OnAnimationEvent_Impl;
        }

        /// <summary>
        /// 턴을 받았을때
        /// </summary>
        public override void TakeTurn()
        {
            // 부모 클래스에서 TakeTurn 실행 후 실행
            base.TakeTurn();

            // 이거 순서가 공격 준비를 먼저 시작하고 공격을 시작하면 여기가 실행이 된다
            Debug.Log("턴을 받았다!");

            // 타겟을 초기화 한다
            target = null;

            // 자신의 턴임을 알린다
            myTurn = true;

            // 현재 상태가 그로기라면
            if (this.CurrentState == CharacterState.Groggy)
            {
                // 다음 프레임에서 공격을 시작하기위한 코루틴을 실행한다
                StartCoroutine(Groggy_Idle());

                // 애니메이터의 불값을 변경한다               
                animator.SetBool("GroggyBool", false);

                // 현재 강인도를 최대로 한다
                this.Data.Toughness.Reset();


            }
            // 현재 상태가 그로기가 아니라면
            else
            {
                // 스킬 쿨타임이 2이상 이면
                if (skill_cool >= 2)
                {
                    // 스킬을 준비하는 함수를 실행한다
                    PrepareSkill();
                }
                // 스킬 쿨타임이 2미만 이라면
                else if (skill_cool < 2)
                {
                    // 공격을 준비하는 함수를 실행한다
                    PrepareAttack();
                }
            }

        }

        IEnumerator Groggy_Idle()
        {

            // 그로기 타임라인을 정지시킨뒤 재생을 시킨다
            Groggy_anim.Stop();
            Groggy_anim.Play();

            // 그로기 타임라인이 실행중일때
            while (Groggy_anim.state == PlayState.Playing)
            {
                // 매 프래임 대기
                yield return null;
            }

            Debug.Log("코루틴 실행");

            // 그로기 타임라인을 끝까지 진행시킨다
            Groggy_anim.time = Groggy_anim.duration;
            // 타임라인을 현재 시간에 맞게 상태를 업데이트
            Groggy_anim.Evaluate();

            // 스킬 쿨타임이 2이상 이면
            if (skill_cool >= 2)
            {
                // 스킬을 준비하는 함수를 실행한다
                PrepareSkill();
            }
            // 스킬 쿨타임이 2미만 이라면
            else if (skill_cool < 2)
            {
                // 공격을 준비하는 함수를 실행한다
                PrepareAttack();
            }
        }

        #region 행동하는 함수 (스킬, 공격, 궁극기, 엑스트라 어택)

        /// <summary>
        /// 스킬을 사용할때
        /// </summary>
        public override void CastSkill()
        {
            // 부모 클래스에서 CastSkill 실행후 실행
            base.CastSkill();
            Debug.Log(this.name + "스킬 공격");

            // 에너미가 플레이어 앞에 오도록 한다
            animator.gameObject.transform.position = target.transform.position - new Vector3(8.47f, 0f);

            // 스킬 공격 대미지 계수
            Damage_factor = Data.AttackTable.skillAttack;

            // 스킬 공격 애니메이션을 멈춘뒤
            skillAttack.Stop();
            // 스킬 공격 애니메이션 재생
            skillAttack.Play();

            // 마지막 공격이 스킬공격임을 알린다
            _lastAttack = CharacterState.CastSkill;

        }

        /// <summary>
        /// 공격을 시작할때
        /// </summary>
        public override void DoAttack()
        {
            base.DoAttack();

            Debug.Log(this.name + "일반 공격!");

            // 에니메이터인 몬스터가 에너미가 플레이어 앞에 오도록 한다            
            animator.gameObject.transform.position = target.transform.position - new Vector3(8.47f, 0f);

            // 일반 공격 대미지 계수
            // (나중에 버프라던가 디버프가 생기면 이곳을 더하거나 빼는 방식도 생각해보자)
            Damage_factor = Data.AttackTable.normalAttack;

            // 일반공격 애니메이션을 멈춘뒤
            normalAttack.Stop();
            // 일반공격 애니메이션이 실행
            normalAttack.Play();

            // 마지막 공격이 일반공격임을 보낸다
            _lastAttack = CharacterState.DoAttack;

        }

        #endregion


        #region 준비하는 함수 (공격, 스킬, 궁극기)

        /// <summary>
        /// 공격을 준비하는 함수
        /// </summary>
        public override void PrepareAttack()
        {
            // 턴을 받았다면
            if (myTurn == true)
            {
                base.PrepareAttack();

                // 생존해 있는 플레이어를 가져온다
                target = TargetManager.instance.SetPlayerTarget();

                // 공격하는 함수
                DoAttack();
            }
            // 턴을 받지 않았다면
            else
                return;
        }
        /// <summary>
        /// 스킬을 준비하는 함수
        /// </summary>
        public override void PrepareSkill()
        {
            // 턴을 받았다면
            if (myTurn == true)
            {
                base.PrepareSkill();

                // 생존해 있는 플레이어를 가져온다
                target = TargetManager.instance.SetPlayerTarget();

                // 스킬을 사용하는 함수
                CastSkill();
            }
            // 턴을 받지 않았다면
            else
                return;

        }
        /// <summary>
        /// 궁극기를 준비하는 함수
        /// </summary>
        public override void PrepareUltAttack()
        {
            base.PrepareUltAttack();
        }

        #endregion

        /// <summary>
        /// 혹시 몰라서 만든 데미지 함수 (때린 놈의 정보를 가져온다)
        /// </summary>
        /// <param name="attacker">때린놈</param>
        /// <param name="result"></param>
        public override void Damage(Character attacker, DamageResult result)
        {
            // 부모 클래스의 Dagage를 실행후 실행
            base.Damage(attacker, result);

            // 캐릭터의 상태가 dead상태가 아닐때
            if (this.CurrentState != CharacterState.Dead)
            {
                // 데미지 애니메이션의 트리거를 켠다
                animator.SetTrigger("Damage");
            }

            Debug.Log("데미지를 입었다");

        }

        /// <summary>
        /// 그로기 함수
        /// </summary>
        public override void Groggy()
        {
            base.Groggy();

            Debug.Log("그로기 상태 진입");

            // 그로기 애니메이션 트리거를 켠다
            animator.SetBool("GroggyBool", true);

            Debug.Log("캐릭터의 현재 상태 : " + this.CurrentState);
        }

        /// <summary>
        /// 사망시 호출되는 함수
        /// </summary>
        public override void Dead()
        {
            base.Dead();

            Debug.Log("데드 진입 애니메이션 실행");

            // 데드 애니메이션의 트리거를 켠다
            animator.SetTrigger("Dead");
        }

    }
}