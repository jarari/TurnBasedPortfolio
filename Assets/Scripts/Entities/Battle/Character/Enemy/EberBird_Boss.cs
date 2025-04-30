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
    /// 보스
    /// </summary>
    public class EberBird_Boss : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // 일반 공격 애니메이션
        public PlayableDirector skillAttack;        // 스킬 공격 애니메이션       
        public PlayableDirector Groggy_anim;    // 그로기 애니메이션
        public PlayableDirector Rampage_anim;   // 광폭화 애니메이션

        [Header("Components")]
        public Animator animator;   // 캐릭터의 애니메이터

        // 캐릭터의 마지막 공격 상태를 담을 변수
        private CharacterState _lastAttack;

        // 공격할 플레이어를 담을 변수
        public Character target;
        // 생존해있는 플레이어들을 담을 리스트
        public List<Character> M_targets;
        // 생존한 플레이어들의 중앙을 잡을 벡터
        Vector3 Center;

        // 데미지 피해를 가할시 일반공격과 스킬 데미지 계수를 담을 변수
        public float Damage_factor = 0f;

        public float skill_cool = 0f;

        // 턴을 받은 뒤 행동하기 위한 변수
        private bool myTurn = false;

        #region 보스의 상태 (광폭화, 채력 갯수)

        // 에너미의 상태 (노말, 광폭화)
        public enum BossState { Normal, Rampage}
        // ↑로 만든 변수
        public BossState b_State;        

        // 보스의 광폭화 상태를 나타내는 불값
        bool ram = false;

        // 광폭화용 파티클 오브젝트를 담을 변수
        public GameObject ramObj;

        #endregion

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
            if (animEvent == "Normal_Damage")
            {
                // 데미지를 계산하는 함수를 호출하고
                DamageResult result = CombatManager.CalculateDamage(this, target, Damage_factor);

                // 플레이어의 데미지 함수에 때린놈을 자신으로 하고 호출
                target.Damage(this, result);
            }
            else if (animEvent == "Skill_Damage")
            {
                // for문으로 받아온 플레이어 리스트를 순회하며
                for(int i =0; i< M_targets.Count; i++)
                {
                    // 데미지를 계산하는 함수를 호출하고
                    DamageResult result = CombatManager.CalculateDamage(this, M_targets[i], Damage_factor);

                    // 플레이어의 데미지 함수에 때린놈을 자신으로 하고 호출
                    M_targets[i].Damage(this, result);

                    Debug.Log(M_targets[i].name + " 에게 데미지");
                }

            }


            #region 사운드

            // 타임라인에서 사운드 플레이 시그널을 받게 된다면 실행
            if (animEvent == "PlaySound_1")
            {
                // 사운드 메니저에 등록 해놓은 사운드를 실행한다
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

            // 타임라인에서 공격이 끝난 신호를 받게된다면 실행
            if (animEvent == "NormalAttackEnd" || animEvent == "SkillAttackEnd")
            {
                // 공격후 애니메이션 처리 코루틴을 호출후
                StartCoroutine(DelayReturnFromAttack());


                // 스킬 쿨타임을 증가시킨다
                skill_cool++;

                // 스킬 쿨타임이 3을 초과하였을때
                if (skill_cool > 3)
                {
                    // 스킬 쿨타임을 0으로 만든다
                    skill_cool = 0;
                }

                // 타임라인의 상태가 Pause일때 (재생이 종료 되었을때)
                if (normalAttack.state == PlayState.Paused || skillAttack.state == PlayState.Paused)
                {
                    // 일반공격 또는 스킬공격 애니메이션이 끝났다면
                    if (normalAttack.time >= normalAttack.duration || skillAttack.time >= skillAttack.duration)
                    {
                        // 에너미의 부모객체를 기준으로 상대적인 위치를 0으로 맞춘다
                        animator.gameObject.transform.localPosition = Vector3.zero;

                        // 자신의 턴이 끝났음을 알린다
                        myTurn = false;

                        Debug.Log("턴 종료");
                        
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

            // ===== 광폭화 ========
            // 보스의 상태를 노말로 한다
            b_State = BossState.Normal;

            // 광폭화 파티클을 비활성화 한다
            ramObj.SetActive(false);
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
                // 스킬 쿨타임이 3이상 이면
                if (skill_cool >= 3)
                {
                    // 스킬을 준비하는 함수를 실행한다
                    PrepareSkill();
                }
                // 스킬 쿨타임이 3미만 이라면
                else if (skill_cool < 3)
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

            

            // 에너미가 플레이어들의 중앙에 오도록 한다
            animator.gameObject.transform.position = Center - new Vector3(8.47f, 0f);

            // 스킬 공격 대미지 계수
            Damage_factor = 0.25f;

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
            Damage_factor = 0.45f;

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

                // 중앙 벡터값을 초기화 한다
                Center = Vector3.zero;

                // 생존해 있는 플레이어들의 리스트를 가져온다
                M_targets = TargetManager.instance.SetMPlayerTarget();
                // 리스트를 순회하면서
                foreach (var players in M_targets)
                {
                    // 생존한 플레이어들의 벡터값을 모두 더한다
                    Center += players.transform.position;
                }
                // 리스트에 플레이어가 존재한다면
                if (M_targets.Count > 0)
                { 
                    // 모두 더 하였던 벡터값을 가져온 플레이어 숫자만큼 나누어 중앙에 해당하는 벡터값을 구한다
                    Center /= M_targets.Count;
                }
                
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
                                
                // 현재 광폭화 상태가 아니면서 채력이 최대 채력의 절반 이하라면
                if (ram == false && this.Data.HP.Current <= (this.Data.HP.CurrentMax) / 2)
                {
                    // 상태를 광폭화로 바꾼다
                    b_State = BossState.Rampage;

                    this.GetComponent<CharacterBuffSystem>().ApplyBuff("Rampage_Buff", this);
                    
                    // 현재 상태가 그로기 상태라면
                    if (this.CurrentState == CharacterState.Groggy)
                    {
                        // 광폭화 애니메이션을 재생할 코루틴을 실행
                        StartCoroutine(StartRam());

                        // 에너미의 현재 상태를 기본으로 한다
                        this.CurrentState = CharacterState.Idle;

                        animator.SetBool("GroggyBool", false);

                        Debug.Log("광폭화로 그로기 상태에서 강인도를 회복");
                    }

                    // 광폭화 상태임을 알린다
                    ram = true;
                    Debug.Log("광폭화 상태 진입");
                }
            }
            Debug.Log("데미지를 입었다");
        }

        // 광폭화 코루틴
        IEnumerator StartRam()
        {
            // 만약을 위해 광폭화 애니메이션을 중지한다
            Rampage_anim.Stop();
            Rampage_anim.Play();

            // 광폭화 타임라인이 실행중일때
            while (Rampage_anim.state == PlayState.Playing)
            { 
                // 매 프래임 대기
                yield return null;
            }

            Debug.Log("광폭화 코루틴 실행");

            // 광폭화 타임라인을 끝까지 진행시킨다
            Rampage_anim.time = Rampage_anim.duration;
            // 타임라인을 현재 시간에 맞게 상태를 업데이트
            Rampage_anim.Evaluate();

            // 광폭화용 파티클을 활성화 한다
            ramObj.SetActive(true); 
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

            // 광폭화용 파티클을 비활성화 한다
            ramObj.SetActive(false);

            // 데드 애니메이션의 트리거를 켠다
            animator.SetTrigger("Dead");
        }
    }
}