using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Battle {
    [System.Serializable]
    public class BuffTableEntry {
        public string name;
        public BuffData buffData;
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuffTable")]
    public class BuffTable : ScriptableObject {
        public List<BuffTableEntry> entries;
    }
}