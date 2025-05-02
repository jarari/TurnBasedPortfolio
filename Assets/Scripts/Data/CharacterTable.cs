using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

namespace TurnBased.Battle {
    [System.Serializable]
    public class CharacterTableEntry {
        public string name;
        public CharacterData characterData;
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterTable")]
    public class CharacterTable : ScriptableObject {
        public List<CharacterTableEntry> entries;
    }
}