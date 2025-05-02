using UnityEngine;

namespace TurnBased.Battle.BuffEffects {

    public class DecreaseStackOnTurnStartEffect : IBuffEffect {
        public BuffInstance Instance { get; }

        public DecreaseStackOnTurnStartEffect(BuffInstance instance) {
            Instance = instance;
        }

        public void OnApply(Character caster, Character owner) { }

        public void OnRemove(Character caster, Character owner) { }

        public void OnTurnStart(Character caster, Character owner, TurnContext ctx) {
            Instance.DecreaseStack(1);
        }

        public void OnTurnEnd(Character caster, Character owner, TurnContext ctx) { }
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/BuffEffects/DecreaseStackOnTurnStart")]
    public class DecreaseStackOnTurnStart : BuffEffectDefinition {
        public override IBuffEffect Create(BuffInstance instance)
            => new DecreaseStackOnTurnStartEffect(instance);
    }
}