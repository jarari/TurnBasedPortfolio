using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using System.Collections;

namespace TurnBased.Entities.Battle {
    public class TestEnemyCharacter : Character {
        
        // 에너미의 종류 ( 일반 몹, 보스)
        public enum EnemyType { Minion,Boss }
        // ↑로 만든 변수
        public EnemyType e_Type;

        // 에너미의 상태 (노말, 광폭)
        public enum EnemyState { Nomal, Rampage }
        // ↑로 만든 변수
        public EnemyState e_State;

        // 광폭화 상태를 제어할 불값
        bool ram = false;

        // 타겟팅할 플레이어를 담을 변수
        public Character ch_player;
        public Character[] ch_players;

        // 본인의 위치와 회전값을 담을 변수
        public Vector3 EnPosition;
        public Vector3 EnRotate;
                
        protected override void Start()
        {
            base.Start();
            // 시작시 에너미의 상태를 Nomal로 한다
            e_State = EnemyState.Nomal;
            // 시작시 에너미의 현재 위치와 회전값을 저장한다
            EnPosition = transform.position;            
            EnRotate = transform.eulerAngles;
        }
                

        // 공격이 종료 되었을때
        private IEnumerator WaitAttackEnd() {
            yield return null;
            // 공격하기 위해 틀었던 회전값을 원래대로 가져온다
            transform.eulerAngles = EnRotate;
            // 턴을 끝낸다
            EndTurn();
        }

        // 턴을 받았을때
        public override void TakeTurn() {
            // 부모 클래스에서 TakeTurn 실행 후 실행
            base.TakeTurn();
             
            
            // 만약 에너미 종류가 보스일때
            if (e_Type == EnemyType.Boss)
            {
                // 불값을 이용해 한번만 호출 되도록한다
                // 에너미의 현제 채력이 전채 채력의 절반 이하가 되고 광폭화 불값이 false일때
                if (Data.stats.CurrentHP <= (Data.stats.MaxHP / 2) && ram == false)
                {
                    // 캐릭터의 상태를 광폭화 로 갱신한다
                    e_State = EnemyState.Rampage;
                    // 광폭화시 공격력을 1.5배한다
                    Data.stats.Attack += (Data.stats.Attack / 2);

                    // 불값을 변경한다
                    ram = true;
                }
            }

            // 공격하는 함수
            DoAttack();
            
            // 턴을 끝낸다
            EndTurn();
 
        }

        #region 행동하는 함수 (스킬, 공격, 궁극기, 엑스트라 어택)

        // 스킬을 사용할때
        public override void CastSkill() {
            // 부모 클래스에서 CastSkill 실행후 실행
            base.CastSkill();

        }
        // 궁극기
        public override void CastUlt() {
            base.CastUlt();
        }
        // 공격을 시작할때
        public override void DoAttack() {
            base.DoAttack();
            Debug.Log("Enemy Attack");
            
            // 타겟으로 삼을 플레이어 캐릭터를 랜덤하게 고른다
            ChPlayer_S();
            // 에너미가 플레이어를 바라보게한다
            transform.forward = ch_player.gameObject.transform.position;

            // 현제 공격을 진행하는 오브젝트의 이름 (테스트용)
            Debug.Log(gameObject.name + " 의 공격! ");

            // ( 나중에 데미지 관련 클래스등이 만들어지면 호출하자) //
                       

            // 공격을 끝낼 코루틴을 실행한다
            StartCoroutine(WaitAttackEnd());
        }

        // 엑스트라 어텍을 할때
        public override void DoExtraAttack() {
            base.DoExtraAttack();
        }

        #endregion


        #region 준비하는 함수 (공격, 스킬, 궁극기)

        // 공격을 준비하는 함수
        public override void PrepareAttack() {
            base.PrepareAttack();
        }
        // 스킬을 준비하는 함수
        public override void PrepareSkill() {
            base.PrepareSkill();
        }
        // 궁극기를 준비하는 함수
        public override void PrepareUlt() {
            base.PrepareUlt();
        }

        #endregion

        // 최종적으로 계산 된 데미지를 받을 함수


        // 죽음을 다룰 코루틴        
        IEnumerator DeadProcess()
        {
            yield return null;
            // 캐릭터 모델을 비활성화
            SetVisible(false);            
        }

        // 그로기 코루틴
        IEnumerator GroggyProcess()
        {
            yield return null;
            // 현재 스피드와 방어력을 절반으로 한다
            Data.stats.Speed = (Data.stats.Speed) / 2;
            Data.stats.Defense = (Data.stats.Defense) / 2;
        }

        // 아군 캐릭터칸에 있는 플레이어 캐릭터를 랜덤하게 하나를 가져온다
        public void ChPlayer_S()
        {
            // 랜덤한 숫자를 뽑아
            int ran = Random.Range(0, 2);
            // 캐릭터 메니저의 등록된 모든 아군 캐릭터 리스트를 가져온다 (후에 생존한 캐릭터만 가져올 예정)
            var player = CharacterManager.instance.GetAllyCharacters();
            // 랜덤한 아군 캐릭터를 리스트에서 가져온다
            ch_player = player[ran];
        }
        // 아군 캐릭터칸에 있는 플레이어 캐릭터를 랜덤하게 지정한 만큼 가져온다
        public void ChPlayer_M(int x)
        {
            // 랜덤한 숫자를 뽑아
            int ran = Random.Range(0, 2);
            // 캐릭터 메니저의 등록된 모든 아군 캐릭터 리스트를 가져온다 (후에 생존한 캐릭터만 가져올 예정)
            var player = CharacterManager.instance.GetAllyCharacters();
            
        }

    }
}
