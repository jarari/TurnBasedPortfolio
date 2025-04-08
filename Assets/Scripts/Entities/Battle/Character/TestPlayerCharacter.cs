using UnityEngine;
using TurnBased.Battle;

namespace TurnBased.Entities.Battle {
    public class TestPlayerCharacter : Character {
        public override void CastSkill() {
            base.CastSkill();
        }

        public override void CastUlt() {
            base.CastUlt();
        }

        public override void DoAttack() {
            base.DoAttack();
            EndTurn();
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
