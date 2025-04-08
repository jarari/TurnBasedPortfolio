using UnityEngine;
using TurnBased.Battle;
using System.Collections;

namespace TurnBased.Entities.Battle {
    public class TestEnemyCharacter : Character {
        private IEnumerator WaitAttackEnd() {
            yield return null;
            EndTurn();
        }
        public override void TakeTurn() {
            base.TakeTurn();
            DoAttack();
        }
        public override void CastSkill() {
            base.CastSkill();
        }

        public override void CastUlt() {
            base.CastUlt();
        }

        public override void DoAttack() {
            base.DoAttack();
            Debug.Log("Enemy Attack");
            StartCoroutine(WaitAttackEnd());
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

        public override void PrepareUlt() {
            base.PrepareUlt();
        }
    }
}
