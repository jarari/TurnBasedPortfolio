using UnityEngine;

namespace TurnBased.Battle {
    public interface IBuffEffect {
        void OnApply(Character caster, Character owner);
        void OnRemove(Character caster, Character owner);

        void OnTurnStart(Character caster, Character owner, TurnType type);

        void OnTurnEnd(Character caster, Character owner, TurnType type);
    }

    public abstract class BuffEffectDefinition : ScriptableObject {
        public abstract IBuffEffect Create();
    }
}