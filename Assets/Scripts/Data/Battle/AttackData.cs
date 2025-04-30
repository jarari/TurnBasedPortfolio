using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TurnBased.Battle {
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AttackData", order = 1)]
    public class AttackData : ScriptableObject {
        public List<float> damageMult;
        public List<float> toughnessDamage;
        public bool canCrit;

        [SerializeField]
        private float _totalToughnessDamage;

        public float TotalToughnessDamage => _totalToughnessDamage;

        private void OnEnable() {
            RecalculateCache();
        }

#if UNITY_EDITOR
        private void OnValidate() {
            RecalculateCache();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        private void RecalculateCache() {
            _totalToughnessDamage = toughnessDamage?.Sum() ?? 0f;
        }
    }
}
