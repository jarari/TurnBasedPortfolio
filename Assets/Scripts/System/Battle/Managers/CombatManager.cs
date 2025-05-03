using System;
using TurnBased.Data;
using UnityEngine;
using Random = UnityEngine.Random;


namespace TurnBased.Battle.Managers
{

    // 데미지를 정의 하는 클래스
    public class DamageResult
    {
        public float BaseDamage;        // 그냥 공격 데미지
        public float ReducedDamage;     // 방어력으로 감소한 데미지
        public float FinalDamage;       // 최종적으로 받는 데미지
        public float ToughnessDamage;   // 강인도 데미지
        public bool IsCrit;            // 치명타 여부
    }

    // 데미지를 계산하는 클래스
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager instance;

        public event Action<int> OnSkillPointChanged;
        public event Action<int> OnSkillPointMaxChanged;
        public event Action<Character, Character> OnCharacterDeath;
        public event Action<Character> OnCharacterDeathComplete;
        public event Action<Character, Character, DamageResult> OnCharacterInflictedDamage;

        public int SkillPoint { get; private set; } = 3;

        public int SkillPointMax { get; private set; } = 5;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public void SetSkillPoint(int p)
        {
            SkillPoint = Math.Clamp(p, 0, SkillPointMax);
            OnSkillPointChanged?.Invoke(SkillPoint);
            CombatUIManager.Instance?.UpdateSkillPointUI(SkillPoint);
        }

        public void SetSkillPointMax(int pMax)
        {
            SkillPointMax = pMax;
            OnSkillPointMaxChanged?.Invoke(pMax);
        }

        public void ModifySkillPoint(int delta)
        {
            SkillPoint = Math.Clamp(SkillPoint + delta, 0, SkillPointMax);
            OnSkillPointChanged?.Invoke(SkillPoint);
            CombatUIManager.Instance?.UpdateSkillPointUI(SkillPoint);
        }

        public void NotifyCharacterDeath(Character victim, Character killer)
        {
            if (killer.Data.Team == CharacterTeam.Player)
            {
                killer.Data.UltPts.ModifyCurrent(10);
            }
            OnCharacterDeath?.Invoke(victim, killer);
        }

        public void NotifyCharacterDeathComplete(Character c)
        {
            OnCharacterDeathComplete?.Invoke(c);
        }

        public void NotifyCharacterInflictedDamage(Character attacker, Character victim, DamageResult result)
        {
            if (attacker.Data.Team == CharacterTeam.Player)
            {
                switch (attacker.CurrentState)
                {
                    case Character.CharacterState.DoAttack:
                        attacker.Data.UltPts.ModifyCurrent(20);
                        break;
                    case Character.CharacterState.CastSkill:
                        attacker.Data.UltPts.ModifyCurrent(30);
                        break;
                    case Character.CharacterState.CastUltAttack:
                        attacker.Data.UltPts.ModifyCurrent(30);
                        break;
                    case Character.CharacterState.CastUltSkill:
                        attacker.Data.UltPts.ModifyCurrent(30);
                        break;
                    case Character.CharacterState.DoExtraAttack:
                        attacker.Data.UltPts.ModifyCurrent(10);
                        break;
                }
            }
            OnCharacterInflictedDamage?.Invoke(attacker, victim, result);
        }

        // 데미지 피해를 계산할 함수 (때린 놈과 맞은 놈을 가져온다)
        public static DamageResult CalculateDamage(Character attacker, Character defender, AttackData attackData, int attackNum = 0)
        {
            // 때린 놈의 공격력을 가져온다
            float baseDamage = attacker.Data.Attack.Current * attackData.damageMult[attackNum];

            float reducedPercentage = defender.Data.Defense.Current / (defender.Data.Defense.Current + 1000);

            float victimDef = defender.Data.Defense.Current;

            bool isCrit = false;
            if (attackData.canCrit && Random.Range(float.Epsilon, 1f) <= attacker.Data.CritChance.Current)
            {
                baseDamage *= attacker.Data.CritMult.Current;
                isCrit = true;
            }
            float damageReduction = victimDef / (victimDef + 1000f);

            float damageReduced = baseDamage * damageReduction;

            // DamageResult를 반환한다
            return new DamageResult
            {
                BaseDamage = baseDamage,
                ReducedDamage = damageReduced,
                FinalDamage = baseDamage - damageReduced,
                ToughnessDamage = attackData.toughnessDamage[attackNum],
                IsCrit = isCrit
            };
        }

        public static DamageResult CalculateTrueDamage(float damage, float toughnessDamage)
        {
            return new DamageResult
            {
                BaseDamage = damage,
                ReducedDamage = 0,
                FinalDamage = damage,
                ToughnessDamage = toughnessDamage,
                IsCrit = false
            };
        }

        public static bool CheckElementMatch(ElementType type1, ElementType type2)
        {
            return (type1 & type2) > 0;
        }

        public static bool CanCharacterUseUlt(Character c)
        {
            return c.Data.UltPts.Current >= c.Data.UltThreshold;
        }
    }

}