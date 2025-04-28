using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace TurnBased.Data {
    public class CharacterDataInstance {
        public class Stat {
            public float Base { get; }
            public float Current { get; private set; }
            public float CurrentMax { get; private set; }
            public event Action<float> OnValueChanged;
            public event Action<float> OnValueMaxChanged;

            public Stat(float baseValue, bool hasMax = false) {
                Base = baseValue;
                Current = baseValue;
                if (hasMax) {
                    CurrentMax = baseValue;
                }
                else {
                    CurrentMax = -1;
                }
            }

            public void ModifyCurrent(float delta) {
                if (CurrentMax == -1) {
                    Current = Current + delta;
                }
                else {
                    Current = Mathf.Clamp(Current + delta, 0, CurrentMax);
                }
                OnValueChanged?.Invoke(Current);
            }

            public void ModifyCurrentMax(float delta) {
                CurrentMax = CurrentMax + delta;
                OnValueMaxChanged?.Invoke(CurrentMax);
            }

            public void SetCurrent(float value) {
                if (CurrentMax == -1) {
                    Current = value;
                }
                else {
                    Current = Mathf.Clamp(value, 0, CurrentMax);
                }
                OnValueChanged?.Invoke(Current);
            }

            public void SetCurrentMax(float value) {
                CurrentMax = value;
                OnValueMaxChanged?.Invoke(CurrentMax);
            }

            public void Reset() => SetCurrent(CurrentMax);
        }

        public Stat HP { get; }
        public Stat Toughness { get; }
        public Stat Attack { get; }
        public Stat Defense { get; }
        public Stat Speed { get; }
        public float UltThreshold { get; }
        public ElementType ElementType { get; }
        public ElementType Weakness { get; }
        public CharacterTeam Team { get; }

        private readonly List<StatModifier> _modifiers = new List<StatModifier>();

        public CharacterDataInstance(CharacterData data) {
            HP = new Stat(data.stats.MaxHP, true);
            Toughness = new Stat(data.stats.MaxToughness, true);
            Attack = new Stat(data.stats.Attack);
            Defense = new Stat(data.stats.Defense);
            Speed = new Stat(data.stats.Speed);
            UltThreshold = data.stats.UseUltThreshold;
            ElementType = data.stats.ElementType;
            Weakness = data.stats.Weakness;
            Team = data.team;
        }

        public void AddModifier(StatModifier mod) {
            _modifiers.Add(mod);
            RecalculateStat(mod.stat);
        }

        public void RemoveModifier(StatModifier mod) {
            if (_modifiers.Remove(mod))
                RecalculateStat(mod.stat);
        }

        private void RecalculateStat(StatType type) {
            float baseValue = type switch {
                StatType.HP => HP.Base,
                StatType.Toughness => Toughness.Base,
                StatType.Attack => Attack.Base,
                StatType.Defense => Defense.Base,
                StatType.Speed => Speed.Base,
                _ => 0f
            };

            float val = baseValue;

            foreach (var m in _modifiers.Where(m => m.stat == type && m.modType == ModifierType.Multiply))
                val *= m.value;

            foreach (var a in _modifiers.Where(m => m.stat == type && m.modType == ModifierType.Additive))
                val += a.value;

            foreach (var o in _modifiers.Where(m => m.stat == type && m.modType == ModifierType.Set))
                val = o.value;

            switch (type) {
                case StatType.HP:
                    float prevHPPercentage = HP.Current / HP.CurrentMax;
                    HP.SetCurrentMax(val);
                    HP.SetCurrent(val * prevHPPercentage);
                    break;
                case StatType.Toughness:
                    float prevToughnessPercentage = Toughness.Current / Toughness.CurrentMax;
                    Toughness.SetCurrentMax(val);
                    Toughness.SetCurrent(val * prevToughnessPercentage);
                    break;
                case StatType.Attack: 
                    Attack.SetCurrent(val); 
                    break;
                case StatType.Defense: 
                    Defense.SetCurrent(val); 
                    break;
                case StatType.Speed: 
                    Speed.SetCurrent(val); 
                    break;
            }
        }
    }
}