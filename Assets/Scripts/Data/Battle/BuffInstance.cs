using System.Collections.Generic;

namespace TurnBased.Battle {
    public class BuffInstance : IBuff {
        public bool IsExpired => _data.duration > 0 ? _elapsed >= _data.duration : false;
        public int Stacks { get; private set; }

        private readonly BuffData _data;
        private readonly List<IBuffEffect> _effects = new();
        private readonly Character _caster;
        private readonly Character _owner;
        private float _elapsed;

        public BuffInstance(BuffData data, Character caster, Character owner) {
            _data = data;
            _caster = caster;
            _owner = owner;

            foreach (var def in data.extraEffects)
                _effects.Add(def.Create(this));
        }

        public void OnApply() {
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

        public void OnTurnStart(TurnType type) {
            foreach (var e in _effects)
                e.OnTurnStart(_caster, _owner, type);
        }

        public void OnTurnEnd(TurnType type) {
            foreach (var e in _effects)
                e.OnTurnEnd(_caster, _owner, type);

            if (_data.duration > 0)
                _elapsed += 1;
        }

        public void ResetDuration() {
            if (_data.duration > 0)
                _elapsed = 0;
        }
    }
}
