using UnityEngine;

namespace TurnBased.Battle {
    public interface IBuffEffect {
        /// <summary>
        /// �ش� ȿ���� ������ BuffInstance
        /// </summary>
        BuffInstance Instance { get; }
        void OnApply(Character caster, Character owner);
        void OnRemove(Character caster, Character owner);

        void OnTurnStart(Character caster, Character owner, TurnContext ctx);

        void OnTurnEnd(Character caster, Character owner, TurnContext ctx);
    }

    public abstract class BuffEffectDefinition : ScriptableObject {
        public abstract IBuffEffect Create(BuffInstance instance);
    }
}