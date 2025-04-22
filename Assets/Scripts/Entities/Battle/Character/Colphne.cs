using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using UnityEngine.Playables;
using Unity.Cinemachine;
using UnityEngine.VFX;
using UnityEngine.Animations.Rigging;

namespace TurnBased.Entities.Battle {
    public class Colphne : Character {
        [Header("Timelines")]
        public PlayableDirector normalAttack;
        public PlayableDirector skillAttack;
        public PlayableDirector ultAttack;
        [Header("Skill Objects")]
        public VisualEffect muzzleflash;
        public CinemachineImpulseSource impulseSource;
        public CinemachineCamera normalAttackCam;
        public CinemachineCamera skillTargetCam;
        public Transform healProjectileRoot;
        public Rig aimRig;
        public MultiAimConstraint headTracking;
        [Header("Components")]
        public Animator animator;

        private CharacterState _lastAttack;

        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload) {
            if (animEvent == "AttackEnd") {
                animator.SetInteger("State", 0);
                EndTurn();
            }
            else if (animEvent == "WeaponFire") {
                muzzleflash.Play();
                impulseSource.GenerateImpulse();
                var targets = TargetManager.instance.GetTargets();
                foreach (var t in targets) {
                    DamageResult result = CombatManager.CalculateDamage(c, t, 0.25f);
                    t.Damage(c, result);
                }
            }
            else if (animEvent == "MoveProjectile") {
                healProjectileRoot.transform.position = TargetManager.instance.Target.transform.position;
            }
            else if (animEvent == "Heal") {
                if (payload == "Skill") {
                    TargetManager.instance.Target.RestoreHealth(this, Data.stats.Attack);
                }
            }
        }

        protected override void Awake() {
            base.Awake();
            OnAnimationEvent += OnAnimationEvent_Impl;
        }

        protected override void Start() {
            base.Start();
            headTracking.data.sourceObjects.Clear();
            headTracking.data.sourceObjects.Add(new WeightedTransform(TargetManager.instance.camTarget.transform, 0.6f));
        }

        public override void CastSkill() {
            base.CastSkill();
            aimRig.weight = 0;
            var ally = TargetManager.instance.Target;
            healProjectileRoot.transform.localPosition = Vector3.zero;
            skillTargetCam.Follow = ally.transform;
            skillAttack.Play();
            _lastAttack = CharacterState.CastSkill;
        }

        public override void CastUltAttack() {
            base.CastUltAttack();
            EndTurn();
        }

        public override void CastUltSkill() {
            base.CastUltAttack();
            EndTurn();
        }

        public override void DoAttack() {
            base.DoAttack();
            aimRig.weight = 0;
            var enemy = TargetManager.instance.Target;
            Vector3 diff = enemy.transform.position - transform.position;
            diff.y = 0;
            diff.Normalize();
            meshParent.transform.forward = diff;
            normalAttack.Play();
            _lastAttack = CharacterState.DoAttack;
        }

        public override void DoExtraAttack() {
            base.DoExtraAttack();
        }

        public override void PrepareAttack() {
            base.PrepareAttack();
            animator.SetInteger("State", 0);
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, TurnBased.Data.CharacterTeam.Enemy);
        }

        public override void PrepareSkill() {
            base.PrepareSkill();
            animator.SetInteger("State", 1);
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, TurnBased.Data.CharacterTeam.Player);
        }

        public override void PrepareUltAttack() {
            base.PrepareUltAttack();
            aimRig.weight = 0;
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.All, TurnBased.Data.CharacterTeam.Player);
        }

        public override void PrepareUltSkill() {
            base.PrepareUltAttack();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.All, TurnBased.Data.CharacterTeam.Player);
        }

        public override void ProcessCamChanged() {
            if (_lastAttack == CharacterState.DoAttack) {
                normalAttack.time = normalAttack.duration;
                normalAttack.Evaluate();
            }
            else if (_lastAttack == CharacterState.CastSkill) {
                skillAttack.time = skillAttack.duration;
                skillAttack.Evaluate();
            }
            else if (_lastAttack == CharacterState.CastUltAttack) {
                ultAttack.time = ultAttack.duration;
                ultAttack.Evaluate();
            }
            meshParent.transform.localPosition = Vector3.zero;
            meshParent.transform.forward = -Vector3.right;
        }
    }
}
