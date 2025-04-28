using System;
using System.Collections.Generic;
using System.Linq;
using TurnBased.Battle.Managers;
using UnityEngine;

namespace TurnBased.Battle {
    public class CharacterBuffSystem : MonoBehaviour {
        public event Action<BuffInstance> OnBuffApplied;
        public event Action<BuffInstance> OnBuffRemoved;

        private readonly Dictionary<string, BuffInstance> _activeBuffs = new();
        private Character _owner;

        private void Awake() {
            _owner = GetComponent<Character>();
        }

        private void Start() {
            TurnManager.instance.OnBeforeTurnStart += HandleBeforeTurnStart;
            TurnManager.instance.OnTurnEnd += HandleTurnEnd;
        }

        private void OnDestroy() {
            TurnManager.instance.OnBeforeTurnStart -= HandleBeforeTurnStart;
            TurnManager.instance.OnTurnEnd -= HandleTurnEnd;
        }

        private void HandleBeforeTurnStart(Character character, TurnType type) {
            if (type != TurnType.Normal || character != GetComponent<Character>()) {
                return;
            }

            foreach (var buff in _activeBuffs)
                buff.Value.OnTurnStart(type);
        }

        private void HandleTurnEnd(Character character, TurnType type) {
            if (type != TurnType.Normal || character != GetComponent<Character>()) {
                return;
            }

            foreach (var buff in _activeBuffs)
                buff.Value.OnTurnEnd(type);

            var keys = _activeBuffs.Keys.ToList();
            foreach (var key in keys) {
                var buff = _activeBuffs[key];
                if (buff.IsExpired) {
                    RemoveBuff(key);
                }
            }
        }

        public void ApplyBuff(string identifier, Character caster, BuffData buffData) {
            BuffInstance instance;
            if (_activeBuffs.ContainsKey(identifier)) {
                instance = _activeBuffs[identifier];
                instance.ResetDuration();
                if (!buffData.stackable) {
                    return;
                }
            }
            else {
                instance = new BuffInstance(buffData, caster, _owner);
                _activeBuffs.Add(identifier, instance);
            }
            instance.OnApply();

            OnBuffApplied?.Invoke(instance);
        }

        public void RemoveBuff(string identifier) {
            if (_activeBuffs.ContainsKey(identifier)) {
                var buff = _activeBuffs[identifier];
                buff.OnRemove();
                _activeBuffs.Remove(identifier);

                OnBuffRemoved?.Invoke(buff);
            }
        }
    }
}