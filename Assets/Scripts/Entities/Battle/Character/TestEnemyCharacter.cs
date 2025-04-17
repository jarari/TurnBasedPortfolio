using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using System.Collections;

namespace TurnBased.Entities.Battle {
    public class TestEnemyCharacter : Character {

        private IEnumerator TestCoroutine() {
            yield return new WaitForSeconds(2f);
            EndTurn();
        }

        public override void TakeTurn() {           
            base.TakeTurn();          

            DoAttack();
        }

        public override void CastSkill() {
            base.CastSkill();         
        }
        public override void CastUltAttack() {
            base.CastUltAttack();          
        }
        public override void DoAttack() {
            base.DoAttack();
            StartCoroutine(TestCoroutine());
            Debug.Log("Enemy Attack");
        }
        public override void DoExtraAttack() {
            base.DoExtraAttack();
        }
        public override void PrepareAttack() {
            base.PrepareAttack();
        }
        public override void PrepareSkill() {
            base.PrepareSkill();
        }
        public override void PrepareUltAttack() {
            base.PrepareUltAttack();
        }
    }
}
