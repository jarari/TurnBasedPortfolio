using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;

namespace TurnBased.Entities.Battle {
    public class SoccerPlayer : Character {
        [Header("Timelines")]
        public PlayableDirector normalAttack;
        public PlayableDirector skillAttack;
        public PlayableDirector ultAttack;
        [Header("Skill Objects")]
        public GameObject skillBallLeft;
        public GameObject skillBallRight;
        public GameObject ultGoalPost;
        public GameObject hitEffectPrefab;
        [Header("Components")]
        public Animator animator;

        private CharacterState _lastAttack;

        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload) {
            if (animEvent == "AttackEnd") {
                animator.SetInteger("State", 0);
                EndTurn();
            }
            else if (animEvent == "DoDamage") {
                var attackData = Data.AttackTable.normalAttack;
                if (payload == "Skill") {
                    attackData = Data.AttackTable.skillAttack;
                }
                else if (payload == "Ult") {
                    attackData = Data.AttackTable.ultAttack;
                }
                var targets = TargetManager.instance.GetTargets();
                foreach (var t in targets) {
                    DamageResult result = CombatManager.CalculateDamage(c, t, attackData);
                    t.Damage(c, result);
                    if (_lastAttack != CharacterState.CastUltAttack) {
                        var go = Instantiate(hitEffectPrefab, t.transform.position + Vector3.up * 1.2f, Quaternion.identity);
                        go.GetComponent<ParticleSystem>().Play();
                        Destroy(go, 3f);
                    }
                    CombatManager.instance.NotifyCharacterInflictedDamage(this, t, result);
                }

                if (_lastAttack != CharacterState.CastUltAttack) {
                    SoundManager.instance.Play2DSound("SoccerExplosion");
                }
            }
        }

        private void CastUlt() {
            SetMeshLayer(MeshLayer.UltTimeline);
            ultAttack.Play();
            var enemyCenter = TargetManager.instance.Target;
            meshParent.transform.position = enemyCenter.transform.position + new Vector3(10.65f, 0f);
            ultGoalPost.transform.position = Vector3.zero;
            _lastAttack = CharacterState.CastUltAttack;
            foreach (var c in CharacterManager.instance.GetEnemyCharacters()) {
                c.SetMeshLayer(MeshLayer.UltTimeline);
            }
            foreach (var c in CharacterManager.instance.GetAllAllyCharacters()) {
                if (c != this) {
                    c.SetVisible(false);
                }
            }
        }

        protected override void Awake() {
            base.Awake();
            OnAnimationEvent += OnAnimationEvent_Impl;
        }

        public void SetSkillBallVisibility(bool visible) {
            if (visible) {
                var targets = TargetManager.instance.GetTargets();
                if (targets.Count == 3) {
                    skillBallLeft.SetActive(true);
                    skillBallRight.SetActive(true);
                }
                else if (targets.Count == 2) {
                    if (targets[0] == TargetManager.instance.Target) {
                        skillBallRight.SetActive(true);
                    }
                    else {
                        skillBallLeft.SetActive(true);
                    }
                }
            }
            else {
                skillBallLeft.SetActive(false);
                skillBallRight.SetActive(false);
            }
        }

        public override void CastSkill() {
            base.CastSkill();
            var enemyCenter = TargetManager.instance.Target;
            meshParent.transform.position = enemyCenter.transform.position + new Vector3(11.207f, 0f);
            var targets = TargetManager.instance.GetTargets();
            for (int i = 0; i < targets.Count; ++i) {
                targets[i].SetMeshLayer(MeshLayer.SkillTimeine);
                if (i == 0 && targets[i] != enemyCenter) {
                    targets[i].meshParent.transform.position = enemyCenter.meshParent.transform.position - new Vector3(0, 0, 4f);
                }
                else if (i == 1 && targets[i] != enemyCenter || i == 2) {
                    targets[i].meshParent.transform.position = enemyCenter.meshParent.transform.position + new Vector3(0, 0, 4f);
                }
            }
            SetMeshLayer(MeshLayer.SkillTimeine);
            skillAttack.Play();
            _lastAttack = CharacterState.CastSkill;
        }

        public override void CastUltAttack() {
            base.CastUltAttack();
            CastUlt();
        }

        public override void CastUltSkill() {
            base.CastUltSkill();
            CastUlt();
        }

        public override void DoAttack() {
            base.DoAttack();
            var enemy = TargetManager.instance.Target;
            meshParent.transform.position = enemy.transform.position + new Vector3(11.207f, 0f);
            normalAttack.Play();
            foreach (var c in CharacterManager.instance.GetEnemyCharacters()) {
                c.SetMeshLayer(MeshLayer.SkillTimeine);
            }
            SetMeshLayer(MeshLayer.SkillTimeine);
            _lastAttack = CharacterState.DoAttack;
        }

        public override void DoExtraAttack(Character target) {
            base.DoExtraAttack(target);
        }

        public override void PrepareAttack() {
            base.PrepareAttack();
            animator.SetInteger("State", 0);
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void PrepareSkill() {
            base.PrepareSkill();
            animator.SetInteger("State", 1);
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Triple, CharacterTeam.Enemy);
        }

        public override void PrepareUltAttack() {
            base.PrepareUltAttack();
            animator.SetInteger("State", 2);
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void PrepareUltSkill() {
            base.PrepareUltSkill();
            animator.SetInteger("State", 2);
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void Damage(Character attacker, DamageResult result) {
            base.Damage(attacker, result);
            animator.SetTrigger("Hit");
        }

        public override void Dead() {
            base.Dead();
            animator.SetTrigger("Die");
        }

        public override void ProcessCamChanged() {
            if (_lastAttack == CharacterState.DoAttack) {
                normalAttack.time = normalAttack.duration;
                normalAttack.Evaluate();
                normalAttack.Stop();
            }
            else if (_lastAttack == CharacterState.CastSkill) {
                skillAttack.time = skillAttack.duration;
                skillAttack.Evaluate();
                skillAttack.Stop();
                var targets = TargetManager.instance.GetTargets();
                foreach (var t in targets) {
                    t.meshParent.transform.localPosition = Vector3.zero;
                }
            }
            else if (_lastAttack == CharacterState.CastUltAttack) {
                ultAttack.time = ultAttack.duration;
                ultAttack.Evaluate();
                ultAttack.Stop();
            }
            meshParent.transform.localPosition = Vector3.zero;
            _lastAttack = CharacterState.Idle;
        }
    }
}
