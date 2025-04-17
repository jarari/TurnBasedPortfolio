using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine.Playables;
using System.Collections;
using Unity.Cinemachine;

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
        [Header("Components")]
        public Animator animator;

        private CharacterState _lastAttack;

        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload) {
            if (animEvent == "AttackEnd") {
                EndTurn();
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

        public override void DoExtraAttack() {
            base.DoExtraAttack();
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
            Debug.Log("Prepare Ult Attack");
            animator.SetInteger("State", 2);
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void PrepareUltSkill() {
            base.PrepareUltSkill();
            Debug.Log("Prepare Ult Skill");
            animator.SetInteger("State", 2);
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void ProcessCamChanged() {
            if (_lastAttack == CharacterState.DoAttack) {
                normalAttack.time = normalAttack.duration;
                normalAttack.Evaluate();
            }
            else if (_lastAttack == CharacterState.CastSkill) {
                skillAttack.time = skillAttack.duration;
                skillAttack.Evaluate();
                var targets = TargetManager.instance.GetTargets();
                foreach (var t in targets) {
                    t.meshParent.transform.localPosition = Vector3.zero;
                }
            }
            else if (_lastAttack == CharacterState.CastUltAttack) {
                ultAttack.time = ultAttack.duration;
                ultAttack.Evaluate();
            }
            animator.SetInteger("State", 0);
            meshParent.transform.localPosition = Vector3.zero;
        }
    }
}
