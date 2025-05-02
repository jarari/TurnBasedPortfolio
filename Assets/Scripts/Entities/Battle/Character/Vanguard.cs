using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using UnityEngine.Playables;
using Unity.Cinemachine;
using UnityEngine.VFX;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;

namespace TurnBased.Entities.Battle
{
    public class Vanguard : Character
    {
        [Header("Timelines")]
        public PlayableDirector normalAttack;   // �⺻ ����
        public PlayableDirector skillAttack;    // ��ų ����
        public PlayableDirector ultAttack;      // �ʻ��

        [Header("AttackObjects")]
        public GameObject attackObject_1;     // ���ݿ� ���� ������Ʈ 1
        

        [Header("Components")]
        public Animator animator;   // ĳ������ �ִϸ�����

        // ������ ���ʹ̸� ���� �Լ�
        public Character e_target;

        // ������ ���� ���¸� ���� ����
        private CharacterState _lastAttack;
        
        private void OnAnimationEvent_Impl(Character c, string animEvent, string payload)
        {
            // ������ ���� �Ǿ��ٴ� ��ȣ�� �޾Ҵٸ�
            if (animEvent == "AttackEnd")
            {
                // ���� �����Ѵ�
                EndTurn();
            }
            // Ÿ�Ӷ��ο��� ������ �ñ׳��� �ް� �ȴٸ�
            else if (animEvent == "Damage")
            {
                Debug.Log("�������� ������.");

                // �⺻������ �Ϲݰ������� �س��´�
                var attackData = Data.AttackTable.normalAttack;

                if (payload == "Skill")
                {
                    attackData = Data.AttackTable.skillAttack;
                    Debug.Log("��ų ������");
                }
                else if (payload == "Ult")
                {
                    attackData = Data.AttackTable.ultAttack;
                    Debug.Log("�ʻ�� ������");
                }
                // Ÿ�������� �����´�
                var targets = TargetManager.instance.GetTargets();
                // Ÿ�� ������ ��ȸ �ϸ鼭
                foreach (var t in targets)
                {
                    // ������ ������ ����� ���� �������� ����Ѵ�
                    DamageResult result = CombatManager.CalculateDamage(c, t, attackData);
                    // ���ʹ̿��� ������� ������
                    t.Damage(c, result);

                    // �̺�Ʈ�� �����Ų��
                    OnInflictedDamage?.Invoke(this, t, result);
                }

            }

            // Ÿ�Ӷ��ο��� ����� �ñ׳��� �ް� �ȴٸ�
            else if (animEvent == "Radioactivity")
            {
                // Ÿ���� ���ʹ̿� �ֺ��� ���ʹ̵� �����´�
                var targets = TargetManager.instance.GetTargets();

                // Ÿ�ٵ��� ��ȸ�ϸ鼭
                foreach (var t in targets)
                {
                    // �����ڸ� �ڽ����� �ϰ� ���ʹ̿��� ������� �Ǵ�
                    t.GetComponent<CharacterBuffSystem>().ApplyBuff("Radioactivity", this);
                }
            }

            #region ����

            else if (animEvent == "PlaySound1")
            {
                SoundManager.instance.PlayVOSound(this,"VanguardNormalAttack1");
            }
            else if (animEvent == "PlaySound2")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardNormalAttack2");
            }

            else if (animEvent == "PlaySound3")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardSkillAttack1");
            }
            else if (animEvent == "PlaySound4")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardSkillAttack2");                
            }

            else if (animEvent == "PlaySound5")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardUltAttack1");
            }
            else if (animEvent == "PlaySound6")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardUltAttack2");
            }
            else if (animEvent == "PlaySound7")
            {
                SoundManager.instance.PlayVOSound(this, "VanguardUltAttack3");
            }

            #endregion

        }

        private void CastUlt()
        {
            // ���̾ �ʻ츮 Ÿ�Ӷ������� �����
            SetMeshLayer(MeshLayer.UltTimeline);

            // Ÿ�Ӷ����� �����Ѵ�
            ultAttack.Play();

            // ������ ������ �ʻ��� ��´�
            _lastAttack = CharacterState.CastUltAttack;

            // �߾ӿ� �ִ� ���ʹ̸� Ÿ������ �����´�
            var enemyCenter = TargetManager.instance.Target;
            // �ڽ��� ��ġ�� �߾����� ��´�
            meshParent.transform.position = enemyCenter.transform.position + new Vector3(11.207f, 0f);

            //  ��� ���ʹ̸� ��ȸ�ϸ鼭
            foreach (var c in CharacterManager.instance.GetEnemyCharacters())
            {                
                // ���̾ �����Ѵ�
                c.SetMeshLayer(MeshLayer.UltTimeline);
            }
            // �Ʊ� ĳ���ʹ� �ڽ��� �����ϰ� ��� ��Ȱ��ȭ �Ѵ�
            foreach (var c in CharacterManager.instance.GetAllAllyCharacters())
            {
                if (c != this)
                {
                    c.SetVisible(false);
                }
            }

            // �ڽ��� ���̾ �ʻ�� Ÿ�Ӷ��� ���̾�� ��´�
            SetMeshLayer(MeshLayer.UltTimeline);
            
        }

        protected override void Awake()
        {
            base.Awake();
            OnAnimationEvent += OnAnimationEvent_Impl;
        }

        protected override void Start()
        {
            base.Start();
            
        }

        #region �����ϴ� �Լ�

        public override void CastSkill()
        {
            base.CastSkill();
            // Ÿ���� �����´�
            var enemyCenter = TargetManager.instance.Target;
            // �÷��̾ Ÿ������ �ϴ� ���ʹ��� �߽��� �������� ��ġ�� ���
            meshParent.transform.position = enemyCenter.transform.position + new Vector3(11.207f, 0f);
            // �� ���ʹ̿� �ֺ��� Ÿ�ٵ鵵 �����´�
            var targets = TargetManager.instance.GetTargets();

            for (int i = 0; i < targets.Count; i++)
            {
                // ���ʹ��� ���̾ ��ų Ÿ�Ӷ������� �����
                targets[i].SetMeshLayer(MeshLayer.SkillTimeine);
                // ù��° ���ʹ̰� ���Ͱ� �ƴ϶��
                if (i == 0 && targets[i] != enemyCenter)
                {
                    // ��ġ�� ��´�
                    targets[i].meshParent.transform.position = enemyCenter.meshParent.transform.position - new Vector3(0, 0, 4f);
                }
                // ���鵵 ���Ͱ� �ƴ϶��
                else if (i == 1 && targets[i] != enemyCenter || i == 2)
                {
                    // ��ġ�� ��´�
                    targets[i].meshParent.transform.position = enemyCenter.meshParent.transform.position + new Vector3(0, 0, 4f);
                }
                // ���̾ ��ų Ÿ�Ӷ������� �����
                SetMeshLayer(MeshLayer.SkillTimeine);
                // Ÿ�Ӷ����� �����Ѵ�
                skillAttack.Play();
                // ������ ������ castskill�� ��´�
                _lastAttack = CharacterState.CastSkill;
            }
            
        }

        public override void CastUltAttack()
        {
            base.CastUltAttack();
            CastUlt();
        }

        public override void CastUltSkill()
        {
            base.CastUltSkill();
            CastUlt();
        }

        public override void DoAttack()
        {
            base.DoAttack();
            
            // Ÿ���� �����ͼ�
            var enemy = TargetManager.instance.Target;
            // �ڽ��� ��ġ�� ���ʹ� ������ ���
            meshParent.transform.position = enemy.transform.position + new Vector3(11.207f, 0f);
            // �Ϲݰ��� Ÿ�Ӷ����� �����Ѵ�
            normalAttack.Play();
            foreach (var c in CharacterManager.instance.GetEnemyCharacters())
            {
                // ���ʹ��� ���̾ ��ų Ÿ�Ӷ������� ��´�
                c.SetMeshLayer(MeshLayer.SkillTimeine);
            }
            // �ڽ��� ���̾ ��ų Ÿ�Ӷ��� ���̾�� ��´�
            SetMeshLayer(MeshLayer.SkillTimeine);
            // ������ ������ doattack���� ��´�
            _lastAttack = CharacterState.DoAttack;
        }
                
        public override void DoExtraAttack(Character target)
        {
            base.DoExtraAttack(target);
        }

        #endregion

        #region �غ��ϴ� �Լ�

        public override void PrepareAttack()
        {
            base.PrepareAttack();

            Debug.Log("�Ϲ� ���� �غ�");

            // �ִϸ������� int �Ķ���͸� �����Ѵ�
            animator.SetInteger("State", 0);
            // ���ʹ� �ϳ��� Ÿ������ ��´�
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Single, TurnBased.Data.CharacterTeam.Enemy);
        }

        public override void PrepareSkill()
        {
            base.PrepareSkill();

            // �ִϸ������� int �Ķ���͸� �����Ѵ�
            animator.SetInteger("State", 1);

            Debug.Log("��ų ���� �غ�");
            // ���ʹ� ������ Ÿ������ ��´�
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Triple, TurnBased.Data.CharacterTeam.Enemy);
       
        }

        public override void PrepareUltAttack()
        {
            base.PrepareUltAttack();

            // �ִϸ������� int �Ķ���͸� �����Ѵ�
            animator.SetInteger("State", 2);

            Debug.Log("�ʻ�� �غ�");
            // ��� ���ʹ̸� Ÿ������ ��´�
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.Triple, TurnBased.Data.CharacterTeam.Enemy);
        }

        public override void PrepareUltSkill()
        {
            base.PrepareUltSkill();

            // �ִϸ������� int �Ķ���͸� �����Ѵ�
            animator.SetInteger("State", 2);

            // ��� ���ʹ̸� Ÿ������ ��´�
            TargetManager.instance.ChangeTargetSetting(TargetManager.TargetMode.All, TurnBased.Data.CharacterTeam.Enemy);
        }

        #endregion

        public override void Damage(Character attacker, DamageResult result)
        {
            base.Damage(attacker, result);

            // �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Damage");
            // �ǰݽ� �Ҹ��� ����Ѵ�
            SoundManager.instance.Play2DSound("VanguardDamage");
        }

        public override void Dead()
        {
            base.Dead();
            // �ִϸ��̼��� Ʈ���Ÿ� �Ҵ�
            animator.SetTrigger("Dead");
        }

        // ī�޶� ü����
        public override void ProcessCamChanged()
        {
            if (_lastAttack == CharacterState.DoAttack || _lastAttack == CharacterState.DoExtraAttack)
            {
                normalAttack.time = normalAttack.duration;
                normalAttack.Evaluate();
                normalAttack.Stop();
            }
            else if (_lastAttack == CharacterState.CastSkill)
            {
                skillAttack.time = skillAttack.duration;
                skillAttack.Evaluate();
                skillAttack.Stop();
            }
            else if (_lastAttack == CharacterState.CastUltAttack)
            {
                ultAttack.time = ultAttack.duration;
                ultAttack.Evaluate();
                ultAttack.Stop();
            }
            // �ڽ��� ��ġ�� �ٷ� ��´�
            meshParent.transform.localPosition = Vector3.zero;
            _lastAttack = CharacterState.Idle;
        }

    }
}
