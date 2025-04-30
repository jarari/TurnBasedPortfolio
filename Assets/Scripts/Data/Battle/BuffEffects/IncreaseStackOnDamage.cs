using TurnBased.Battle.Managers;
using UnityEngine;

namespace TurnBased.Battle.BuffEffects {

    public class IncreaseStackOnHitEffect : IBuffEffect {
        public BuffInstance Instance { get; }
        private int _lastAttackTurn = -1;

        public IncreaseStackOnHitEffect(BuffInstance instance) {
            Instance = instance;
        }

        public void OnApply(Character caster, Character owner) {
            owner.OnDamage += HandleOwnerDamaged;
        }

        private void HandleOwnerDamaged(Character victim, Character attacker, DamageResult result) {
            int currentTurn = TurnManager.instance.TurnPassed;
            if (currentTurn == _lastAttackTurn) {
                return;
            }
            Instance.OnApply();
            _lastAttackTurn = currentTurn;
        }

        public void OnRemove(Character caster, Character owner) { }

        public void OnTurnStart(Character caster, Character owner, TurnType type) { }

        public void OnTurnEnd(Character caster, Character owner, TurnType type) { }
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/BuffEffects/IncreaseStackOnHit")]
    public class IncreaseStackOnHit : BuffEffectDefinition {
        public override IBuffEffect Create(BuffInstance instance)
            => new IncreaseStackOnHitEffect(instance);
    }
}