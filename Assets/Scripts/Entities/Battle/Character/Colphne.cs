using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;

namespace TurnBased.Entities.Battle {
    public class Colphne : Character {
        public override void CastSkill() {
            base.CastSkill();
            EndTurn();
        }

        public override void CastUltAttack() {
            base.CastUltAttack();
            EndTurn();
        }

        public override void CastUltSkill() {
            base.CastUltAttack();
            EndTurn();
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
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, TurnBased.Data.CharacterTeam.Enemy);
        }

        public override void PrepareSkill() {
            base.PrepareSkill();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, TurnBased.Data.CharacterTeam.Player);
        }

        public override void PrepareUltAttack() {
            base.PrepareUltAttack();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.All, TurnBased.Data.CharacterTeam.Player);
        }

        public override void PrepareUltSkill() {
            base.PrepareUltAttack();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.All, TurnBased.Data.CharacterTeam.Player);
        }
    }
}
