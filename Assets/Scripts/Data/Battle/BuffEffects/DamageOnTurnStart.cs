using System.Collections;
using TurnBased.Battle.Managers;
using UnityEngine;
using UnityEngine.VFX;

namespace TurnBased.Battle.BuffEffects {

    public class DamageOnTurnStartEffect : IBuffEffect {
        public BuffInstance Instance { get; }
        private GameObject _vfxPrefab;
        private string _sfxApply;
        private string _sfxDamage;
        private AttackData _damageData;
        private bool _multiplyByStack;
        private GameObject _vfxInstance;
        private WaitForSeconds _cachedWaitForSeconds = new WaitForSeconds(2f);
        private Coroutine _pauseTurnCoroutine;

        public DamageOnTurnStartEffect(BuffInstance instance, GameObject vfxPrefab, string sfxApply, string sfxDamage, AttackData damageData, bool multiplyByStack) {
            Instance = instance;
            _sfxApply = sfxApply;
            _sfxDamage = sfxDamage;
            _vfxPrefab = vfxPrefab;
            _damageData = damageData;
            _multiplyByStack = multiplyByStack;
        }

        public void OnApply(Character caster, Character owner) {
            if (_vfxPrefab != null && _vfxInstance == null) {
                _vfxInstance = Object.Instantiate(_vfxPrefab);
                _vfxInstance.transform.SetParent(owner.Chest);
                _vfxInstance.transform.position = owner.Chest.position;
                _vfxInstance.transform.rotation = owner.transform.rotation;
            }

            if (_sfxApply.Length > 0) {
                SoundManager.instance.Play2DSound(_sfxApply);
            }
        }

        public void OnRemove(Character caster, Character owner) {
            if (_vfxInstance != null) {
                Object.Destroy(_vfxInstance);
            }
        }

        public void OnTurnStart(Character caster, Character owner, TurnContext ctx) {
            var result = CombatManager.CalculateDamage(caster, owner, _damageData);

            if (_multiplyByStack) {
                result.BaseDamage *= Instance.Stacks;
                result.ReducedDamage *= Instance.Stacks;
                result.FinalDamage *= Instance.Stacks;
            }

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

            if (_sfxDamage.Length > 0) {
                SoundManager.instance.Play2DSound(_sfxDamage);
            }

            if (_pauseTurnCoroutine == null) {
                ctx.Pause();
                _pauseTurnCoroutine = owner.StartCoroutine(ContinueTurn(ctx));
            }
        }

        public void OnTurnEnd(Character caster, Character owner, TurnContext ctx) { }

        private IEnumerator ContinueTurn(TurnContext ctx) {
            yield return _cachedWaitForSeconds;
            ctx.Continue();
            _pauseTurnCoroutine = null;
        }
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/BuffEffects/DamageOnTurnStart")]
    public class DamageOnTurnStart : BuffEffectDefinition {
        public GameObject vfxPrefab;
        public string sfxApply;
        public string sfxDamage;
        public AttackData damageData;
        public bool multiplyByStack;
        public override IBuffEffect Create(BuffInstance instance)
            => new DamageOnTurnStartEffect(instance, vfxPrefab, sfxApply, sfxDamage, damageData, multiplyByStack);
    }
}