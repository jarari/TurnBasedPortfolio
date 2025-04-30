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

        public void ApplyBuff(string identifier, Character caster) {
            BuffData buffData = BuffTableManager.instance.GetBuffData(identifier);
            if (buffData == null) {
                return;
            }

            BuffInstance instance;
            if (_activeBuffs.ContainsKey(identifier)) {
                instance = _activeBuffs[identifier];
                instance.ResetDuration();
                if (!buffData.stackable) {
                    return;
                }
            }
            else {
                instance = new BuffInstance(buffData, caster, _owner, identifier);
                _activeBuffs.Add(identifier, instance);
                instance.OnStackDecreased += CheckBuffStack;
            }
            instance.OnApply();

            OnBuffApplied?.Invoke(instance);
        }

        public void RemoveBuff(string identifier) {
            if (_activeBuffs.ContainsKey(identifier)) {
                var instance = _activeBuffs[identifier];
                RemoveBuff(instance);
            }
        }

        public void RemoveBuff(BuffInstance instance) {
            instance.OnStackDecreased -= CheckBuffStack;
            instance.OnRemove();
            _activeBuffs.Remove(instance.Id);

            OnBuffRemoved?.Invoke(instance);
        }

        public void CheckBuffStack(BuffInstance instance) {
            if (instance.Stacks <= 0) {
                RemoveBuff(instance);
            }
        }

        public bool HasBuff(string identifier) {
            return _activeBuffs.ContainsKey(identifier);
        }
    }
}