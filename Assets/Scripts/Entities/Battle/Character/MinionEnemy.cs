using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;


namespace TurnBased.Entities.Battle { 
    
    /// <summary> 
    /// 일반 몬스터
    /// </summary>
    public class MinionEnemy : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;    // 일반 공격 애니메이션
        public PlayableDirector skillAttack;        // 스킬 공격 애니메이션       

        [Header("Components")]
        public Animator animator;   // 캐릭터의 애니메이터

        // 본인의 회전값을 담을 변수        
        private Vector3 EnRotate;

        // 캐릭터의 마지막 공격 상태를 담을 변수
        private CharacterState _lastAttack;
                
        // 데미지 피해를 가할시 일반공격과 스킬 데미지 계수를 담을 변수
        public float Damage_factor = 0f;

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
                var player = TargetManager.instance.Target;

                // 데미지를 계산하는 함수를 호출하고
                DamageResult result = CombatManager.CalculateDamage(this, player, 1.5f);

                Debug.Log(player.name + " 에게 " + result.FinalDamage + " 데미지");

                // 플레이어의 데미지 함수에 때린놈을 자신으로 하고 호출
                player.Damage(this, result);                
            }
            // 타임라인에서 공격이 끝난 신호를 받게된다면 실행
            if (animEvent == "NormalAttackEnd" || animEvent == "SkillAttackEnd")
            {                
                // 공격후 애니메이션 처리 코루틴을 호출후
                StartCoroutine(DelayReturnFromAttack());

                Debug.Log("턴 종료");
                                
                // 타임라인의 상태가 Pause일때 (재생이 종료 되었을때)
                if (normalAttack.state == PlayState.Paused)
                {
                    // 에너미의 부모객체를 기준으로 상대적인 위치를 0으로 맞춘다
                    meshParent.transform.localPosition = Vector3.zero;
                    // 공격하기 위해 틀었던 회전값을 원래대로 가져온다
                    meshParent.transform.eulerAngles = EnRotate;
                                        
                    // 턴을 종료한다
                    EndTurn();
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

            
            // 강인도가 0이하 라면
            if (this.Data.stats.CurrentToughness < 0)
            {
                // 현재 강인도를 최대로 한다
                this.Data.stats.CurrentToughness = this.Data.stats.MaxToughness;                
            }
                                    
            // 공격을 준비하는 함수를 실행한다
            PrepareAttack();
                        
        }

        #region 행동하는 함수 (스킬, 공격, 궁극기, 엑스트라 어택)

        /// <summary>
        /// 스킬을 사용할때
        /// </summary>
        public override void CastSkill()
        {
            // 부모 클래스에서 CastSkill 실행후 실행
            base.CastSkill();
            Debug.Log("Enemy SkillAttack");

            // 플레이어 타겟을 가져온다
            var player = TargetManager.instance.Target;

            // 에너미가 플레이어 앞에 오도록 한다
            meshParent.transform.position = player.gameObject.transform.position - new Vector3(8.47f, 0f);

            // 스킬 공격 대미지 계수
            Damage_factor = 1.5f;
            
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
            Debug.Log("Enemy Attack");

            // 플레이어 타겟을 가져온다 (1인)
            var player = TargetManager.instance.Target;

            // 에너미가 플레이어 앞에 오도록 한다
            meshParent.transform.position = player.gameObject.transform.position - new Vector3(8.47f,0f);

            // 일반 공격 대미지 계수
            // (나중에 버프라던가 디버프가 생기면 이곳을 더하거나 빼는 방식도 생각해보자)
            Damage_factor = 1.0f;

            // 일반공격 애니메이션이 실행
            normalAttack.Play();
            
            // 마지막 공격이 일반공격임을 보낸다
            _lastAttack = CharacterState.DoAttack;
            
        }

        /// <summary>
        /// 엑스트라 어텍을 할때
        /// </summary>
        public override void DoExtraAttack()
        {
            base.DoExtraAttack();
        }

        #endregion


        #region 준비하는 함수 (공격, 스킬, 궁극기)

        /// <summary>
        /// 공격을 준비하는 함수
        /// </summary>
        public override void PrepareAttack()
        {
            base.PrepareAttack();
                        
            // 타겟을 세팅한다
            TargetManager.instance.SetPlayerTarget();

            // 현재의 회전값을 저장한다
            EnRotate = meshParent.transform.eulerAngles;

            // 공격하는 함수
            DoAttack();
        }
        /// <summary>
        /// 스킬을 준비하는 함수
        /// </summary>
        public override void PrepareSkill()
        {
            base.PrepareSkill();

            // 타겟을 세팅한다 (일단 대상을 단일로)
            TargetManager.instance.SetPlayerTarget();

            // 현재의 회전값을 저장한다
            EnRotate = meshParent.transform.eulerAngles;

            // 스킬을 사용하는 함수
            CastSkill();
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

            // 데미지 애니메이션의 트리거를 켠다
            animator.SetTrigger("Damage");

            // 만약 채력이 0이하가 되었다면
            if (Data.stats.CurrentHP <= 0)
            {
                // 사망 에니메이션의 트리거를 켠다
                animator.SetTrigger("Dead");
            }
        }

        /// <summary>
        /// 그로기 함수
        /// </summary>
        public override void Groggy()
        {
            base.Groggy();
            // 그로기 애니메이션 트리거를 켠다
            animator.SetTrigger("Groggy");

            // 현재 스피드와 방어력을 절반으로 한다
            Data.stats.Speed = (Data.stats.Speed) / 2;
            Data.stats.Defense = (Data.stats.Defense) / 2;
        }

    }



}