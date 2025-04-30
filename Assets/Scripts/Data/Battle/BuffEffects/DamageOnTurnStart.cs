using TurnBased.Battle.Managers;
using UnityEngine;
using UnityEngine.VFX;

namespace TurnBased.Battle.BuffEffects {

    public class DamageOnTurnStartEffect : IBuffEffect {
        public BuffInstance Instance { get; }
        private GameObject _vfxPrefab;
        private string _sfxName;
        private AttackData _damageData;
        private GameObject _vfxInstance;

        public DamageOnTurnStartEffect(BuffInstance instance, GameObject vfxPrefab, string sfxName, AttackData damageData) {
            Instance = instance;
            _sfxName = sfxName;
            _vfxPrefab = vfxPrefab;
            _damageData = damageData;
        }

        public void OnApply(Character caster, Character owner) {
            if (_vfxPrefab != null) {
                _vfxInstance = Object.Instantiate(_vfxPrefab);
                _vfxInstance.transform.SetParent(owner.Chest);
                _vfxInstance.transform.position = owner.Chest.position;
                _vfxInstance.transform.rotation = owner.transform.rotation;
            }
        }

        public void OnRemove(Character caster, Character owner) {
            if (_vfxInstance != null) {
                Object.Destroy(_vfxInstance);
            }
        }

        public void OnTurnStart(Character caster, Character owner, TurnType type) {
            var result = CombatManager.CalculateDamage(caster, owner, _damageData);

            owner.Damage(caster, result);

            if (_vfxInstance != null) {
                var vfx = _vfxInstance.GetComponent<VisualEffect>();
                if (vfx != null) {
                    vfx.Play();
                }
                var particle = _vfxInstance.GetComponent<ParticleSystem>();
                if (particle != null) {
                    particle.Play();
                }
            }

            if (_sfxName.Length > 0) {
                SoundManager.instance.Play2DSound(_sfxName);
            }
        }

        public void OnTurnEnd(Character caster, Character owner, TurnType type) { }
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/BuffEffects/DamageOnTurnStart")]
    public class DamageOnTurnStart : BuffEffectDefinition {
        public GameObject vfxPrefab;
        public string sfxName;
        public AttackData damageData;
        public override IBuffEffect Create(BuffInstance instance)
            => new DamageOnTurnStartEffect(instance, vfxPrefab, sfxName, damageData);
    }
}