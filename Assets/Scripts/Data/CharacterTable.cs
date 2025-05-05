using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

namespace TurnBased.Battle {

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterTable")]
    public class CharacterTable : ScriptableObject {
        public List<CharacterData> entries;
    }
}