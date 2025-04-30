using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

namespace TurnBased.Battle.Managers {
    public class BuffTableManager : MonoBehaviour {
        public static BuffTableManager instance;

        [SerializeField]
        public BuffTable _buffTable;

        private Dictionary<string, BuffData> _buffDict = new();

        private void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }
            instance = this;

            foreach (var entry in _buffTable.entries) {
                if (!_buffDict.ContainsKey(entry.name)) {
                    _buffDict.Add(entry.name, entry.buffData);
                }
            }
        }

        public BuffData GetBuffData(string name) {
            if (_buffDict.TryGetValue(name, out var data)) {
                return data;
            }
            return null;
        }
    }
}
