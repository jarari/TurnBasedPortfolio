using UnityEngine;
using TurnBased.Battle;
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
        public bool ram = false;
                
        protected override void Start()
        {
            base.Start();
            // 시작시 에너미의 상태를 Nomal로 한다
            e_State = EnemyState.Nomal;            
            
        }

        // 공격이 종료 되었을때
        private IEnumerator WaitAttackEnd() {
            yield return null;
            EndTurn();
        }

        // 턴을 받았을때
        public override void TakeTurn() {
            // 부모 클래스에서 TakeTurn 실행 후 실행
            base.TakeTurn();

            // 강인도가 0이하가 되었을때
            if (Data.stats.CurrentToughness <= 0)
            {
                // 턴을 끝낸다
                EndTurn();
            }
            // 강인도가 0이하가 아닐때
            else
            { 
                // 공격하는 함수
                DoAttack();
            }
        }

        #region 행동하는 함수 (스킬, 공격, 궁극기, 엑스트라 어택)

        // 스킬을 사용할때
        public override void CastSkill() {
            // 부모 클래스에서 CastSkill 실행후 실행
            base.CastSkill();

            // 에너미의 상태에 따라 공격을 나누기
            switch (e_State)
            {
                case EnemyState.Nomal:
                    break;
                case EnemyState.Rampage:
                    break;
            }

        }
        // 궁극기
        public override void CastUlt() {
            base.CastUlt();
        }
        // 공격을 시작할때
        public override void DoAttack() {
            base.DoAttack();
            Debug.Log("Enemy Attack");

            // 에너미의 상태에 따라 공격을 나누기
            switch (e_State)
            {
                case EnemyState.Nomal:
                    break;
                case EnemyState.Rampage:
                    break;
            }
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


        // 데미지를 받았을때
        // 플레이어의 공격력을 넣을 damage를 임시로 넣음 추후 변경예정
        public void Damaged(float damage)
        {
            // 현재 채력에서 공격받은 캐릭터의 공격력만큼 채력을 뺀다
            Data.stats.CurrentHP -= damage;
            
            // 현재 채력이 0보다 클때
            if (Data.stats.CurrentHP > 0)
            { 
            }
            // 현재 채력이 0이하가 되었을때
            else
            {
                // 죽음을 다룰 함수
                Dead();
            }
            
            // 만약 에너미 종류가 보스일때
            if (e_Type == EnemyType.Boss)
            { 
                // 에너미의 현제 채력이 전채 채력의 절반 이하가 되고 광폭화 불값이 false일때
                if (Data.stats.CurrentHP <= (Data.stats.MaxHP) / 2 && ram == false)
                {
                    // 캐릭터의 상태를 광폭화 로 갱신한다
                    e_State = EnemyState.Rampage;
                    // 불값을 변경한다
                    ram = true;
                }
            }

        }

        // 죽음을 다룰함수
        public void Dead()
        {
            // 캐릭터 모델을 비활성화
            SetVisible(false);
        }

    }
}
