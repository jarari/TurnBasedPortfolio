using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;

namespace TurnBased.Entities.Battle {
    public class TestEnemyCharacter : Character {

        public override void TakeTurn() {           
            base.TakeTurn();          

            DoAttack();
            
            EndTurn(); 
        }

        public override void CastSkill() {
            base.CastSkill();         
        }
        public override void CastUltAttack() {
            base.CastUltAttack();          
        }
        public override void DoAttack() {
            base.DoAttack();      
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
