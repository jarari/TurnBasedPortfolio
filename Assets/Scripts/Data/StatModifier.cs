using UnityEngine;

namespace TurnBased.Data {
    public enum ModifierType {
        Additive,
        Multiply,
        Set
    }

    [System.Serializable]
    public class StatModifier {
        public StatType stat;
        public ModifierType modType;
        public float value;

        public StatType Stat => stat;
        public ModifierType ModType => modType;
        public float Value => value;
    }
}