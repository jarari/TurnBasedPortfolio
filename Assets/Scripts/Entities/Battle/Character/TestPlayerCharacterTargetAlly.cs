using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;

namespace TurnBased.Entities.Battle {
    public class TestPlayerCharacterTargetAlly : Character {
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

        public override void DoExtraAttack(Character target) {
            base.DoExtraAttack(target);
        }

        public override void PrepareAttack() {
            base.PrepareAttack();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Triple, TurnBased.Data.CharacterTeam.Player);
        }

        public override void PrepareSkill() {
            base.PrepareSkill();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Self);
        }

        public override void PrepareUltAttack() {
            base.PrepareUltAttack();
        }

        // 구동 확인 태스트용
        public override void Damage(Character attacker, DamageResult result)
        {
            base.Damage(attacker, result);
            Debug.Log("때린놈 : " + attacker.name);
        }
    }
}