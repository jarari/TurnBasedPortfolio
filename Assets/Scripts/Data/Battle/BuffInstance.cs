using System;
using System.Collections.Generic;

namespace TurnBased.Battle {
    public class BuffInstance : IBuff {
        public bool IsExpired => _data.duration > 0 ? _elapsed >= _data.duration : false;
        public string Id { get; }
        public int Stacks { get; private set; }

        public Action<BuffInstance> OnStackDecreased;

        private readonly BuffData _data;
        private readonly List<IBuffEffect> _effects = new();
        private readonly Character _caster;
        private readonly Character _owner;
        private float _elapsed;

        public BuffInstance(BuffData data, Character caster, Character owner, string id) {
            _data = data;
            _caster = caster;
            _owner = owner;
            Id = id;

            foreach (var def in data.extraEffects)
                _effects.Add(def.Create(this));
        }

        public void OnApply() {
            if (_data.maxStacks > 0 && Stacks >= _data.maxStacks) {
                return;
            }

            Stacks++;

            foreach (var modData in _data.modifiers)
                _owner.Data.AddModifier(modData);

            foreach (var e in _effects)
                e.OnApply(_caster, _owner);
        }

        public void OnRemove() {
            foreach (var modData in _data.modifiers)
                _owner.Data.RemoveModifier(modData);

            foreach (var e in _effects)
                e.OnRemove(_caster, _owner);
        }

        public void OnTurnStart(TurnContext ctx) {
            foreach (var e in _effects)
                e.OnTurnStart(_caster, _owner, ctx);
        }

        public void OnTurnEnd(TurnContext ctx) {
            foreach (var e in _effects)
                e.OnTurnEnd(_caster, _owner, ctx);

            if (_data.duration > 0)
                _elapsed += 1;
        }

        public void ResetDuration() {
            if (_data.duration > 0)
                _elapsed = 0;
        }

        public void DecreaseStack(int count) {
            Stacks -= count;
            OnStackDecreased?.Invoke(this);
        }
    }
}
