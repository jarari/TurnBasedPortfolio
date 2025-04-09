using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;

namespace TurnBased.Entities.Battle {
    public class TestPlayerCharacterTargetAlly : Character {
        public override void CastSkill() {
            base.CastSkill();
            EndTurn();
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
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Self);
        }

        public override void PrepareSkill() {
            base.PrepareSkill();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Triple);
        }

        public override void PrepareUlt() {
            base.PrepareUlt();
        }
    }
}