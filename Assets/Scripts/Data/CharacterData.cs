using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Data {
    [System.Serializable]
    public struct CharacterStats {
        public float CurrentHP { get; set; }
        [field: SerializeField]
        public float MaxHP { get; set; }
        public float CurrentToughness { get; set; }
        [field: SerializeField]
        public float MaxToughness { get; set; }
        public float CurrentUltPts { get; set; }
        [field: SerializeField]
        public float UseUltThreshold { get; set; }
        [field: SerializeField]
        public float Attack { get; set; }
        [field: SerializeField]
        public float Defense { get; set; }
        [field: SerializeField]
        public float Speed { get; set; }
        [field: SerializeField]
        public ElementType ElementType { get; set; }
        [field: SerializeField]
        public List<ElementType> Weakness { get; set; }
    }

    public enum CharacterTeam {
        Player,
        Enemy
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
    public class CharacterData : ScriptableObject {
        public CharacterStats stats;
        public CharacterTeam team;
    }
}