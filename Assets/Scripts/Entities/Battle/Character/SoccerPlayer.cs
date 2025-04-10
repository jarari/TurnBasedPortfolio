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

        private IEnumerator DelayedReturnFromAttack() {
            yield return null;
            foreach (var c in CharacterManager.instance.GetEnemyCharacters()) {
                c.SetMeshLayer(MeshLayer.Default);
            }
            SetMeshLayer(MeshLayer.Default);
            meshParent.transform.localPosition = Vector3.zero;
            normalAttack.time = normalAttack.duration;
            normalAttack.Evaluate();
        }

        protected override void Awake() {
            base.Awake();
            OnAnimationEvent += OnAnimationEvent_Impl;
        }

        public override void CastSkill() {
            base.CastSkill();
            EndTurn();
        }

        public override void CastUlt() {
            base.CastUlt();
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
        }

        public override void DoExtraAttack() {
            base.DoExtraAttack();
        }

        public override void PrepareAttack() {
            base.PrepareAttack();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void PrepareSkill() {
            base.PrepareSkill();
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, CharacterTeam.Enemy);
        }

        public override void PrepareUlt() {
            base.PrepareUlt();
        }

        public void OnAnimationEvent_Impl(Character c, string animEvent, string payload) {
            if (animEvent == "NormalAttackEnd") {
                StartCoroutine(DelayedReturnFromAttack());
                EndTurn();
            }
        }
    }
}
