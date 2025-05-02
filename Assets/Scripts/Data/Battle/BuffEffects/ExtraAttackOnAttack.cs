using TurnBased.Battle.Managers;
using UnityEngine;

namespace TurnBased.Battle.BuffEffects {

    public class ExtraAttackOnAttackEffect : IBuffEffect {
        public BuffInstance Instance { get; }

        private Character _caster;
        private Character _owner;

        public ExtraAttackOnAttackEffect(BuffInstance instance) {
            Instance = instance;
        }

        public void OnApply(Character caster, Character owner) {
            _caster = caster;
            _owner = owner;
            _owner.OnInflictedDamage += HandleOwnerAttack;
        }

        private void HandleOwnerAttack(Character attacker, Character victim, DamageResult result) {
            if (_owner != _caster) {
                TurnManager.instance.AddExtraAtackTurn(_caster, victim);
            }
        }

        public void OnRemove(Character caster, Character owner) {
            _owner.OnInflictedDamage -= HandleOwnerAttack;
        }

        public void OnTurnStart(Character caster, Character owner, TurnContext ctx) { }

        public void OnTurnEnd(Character caster, Character owner, TurnContext ctx) { }
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/BuffEffects/ExtraAttackOnAttack")]
    public class ExtraAttackOnAttack : BuffEffectDefinition {
        public override IBuffEffect Create(BuffInstance instance)
            => new ExtraAttackOnAttackEffect(instance);
    }
}