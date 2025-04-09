using UnityEngine;
using TurnBased.Battle;
using System.Collections;

namespace TurnBased.Entities.Battle {
    public class TestEnemyCharacter : Character {

        // 에너미의 상태 (노말, 광폭)
        public enum EnemyState { Nomal, Rampage }
        // ↑로 만든 변수
        public EnemyState e_State;
        // 현제 게임 오브젝트를 가져온다
                
        private void Start()
        {
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

            // 공격하는 함수
            DoAttack();
        }
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

        // 그로기 상태 함수
        public void Groggy()
        {
            // 강인도가 0이하가 되었을때
            if (Data.stats.CurrentToughness <= 0)
            { 
            }
        }
              

    }
}
