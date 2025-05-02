using TurnBased.Battle.Managers;
using UnityEngine;

namespace TurnBased.Battle.BuffEffects {

    public class IncreaseStackOnHitEffect : IBuffEffect {
        public BuffInstance Instance { get; }
        private int _lastAttackTurn = -1;
        private string _sfxOnIncrease;

        public IncreaseStackOnHitEffect(BuffInstance instance, string sfxOnIncrease) {
            Instance = instance;
            _sfxOnIncrease = sfxOnIncrease;
        }

        public void OnApply(Character caster, Character owner) {
            owner.OnDamage += HandleOwnerDamaged;
        }

        private void HandleOwnerDamaged(Character victim, Character attacker, DamageResult result) {
            int currentTurn = TurnManager.instance.TurnPassed;
            if (currentTurn == _lastAttackTurn) {
                return;
            }
            Instance.OnApply();
            _lastAttackTurn = currentTurn;

            if (_sfxOnIncrease.Length > 0) {
                SoundManager.instance.Play2DSound(_sfxOnIncrease);
            }
        }

        public void OnRemove(Character caster, Character owner) { }

        public void OnTurnStart(Character caster, Character owner, TurnContext ctx) { }

        public void OnTurnEnd(Character caster, Character owner, TurnContext ctx) { }
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/BuffEffects/IncreaseStackOnHit")]
    public class IncreaseStackOnHit : BuffEffectDefinition {
        public string sfxOnIncrease;
        public override IBuffEffect Create(BuffInstance instance)
            => new IncreaseStackOnHitEffect(instance, sfxOnIncrease);
    }
}