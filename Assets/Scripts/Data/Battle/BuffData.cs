using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

namespace TurnBased.Battle {

    public interface IBuff {
        void OnApply();
        void OnRemove();
        bool IsExpired { get; }

        void OnTurnStart(TurnContext ctx);

        void OnTurnEnd(TurnContext ctx);
        void ResetDuration();
    }


    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuffData")]
    public class BuffData : ScriptableObject {
        public List<StatModifier> modifiers;
        public List<BuffEffectDefinition> extraEffects;
        public float duration;
        public bool stackable;
        public int maxStacks;
        public Sprite icon;
    }
}