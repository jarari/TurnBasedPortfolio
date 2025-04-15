using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;

namespace TurnBased.Entities.Battle {
    public class TestPlayerCharacter : Character {
        public override void CastSkill() {
            base.CastSkill();
            EndTurn();
        }

        public override void CastUltAttack() {
            base.CastUltAttack();
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
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void PrepareSkill() {
            base.PrepareSkill();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void PrepareUltAttack() {
            base.PrepareUltAttack();
        }
    }
}
