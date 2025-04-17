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

        public override void DoExtraAttack() {
            base.DoExtraAttack();
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

        // ���� Ȯ�� �½�Ʈ��
        public override void Damage(Character pl)
        {
            base.Damage(pl);
            Debug.Log("������ : " + pl.name);
        }
    }
}