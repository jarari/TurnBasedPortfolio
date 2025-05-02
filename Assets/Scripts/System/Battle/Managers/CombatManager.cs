using System;
using TurnBased.Data;
using Random = UnityEngine.Random;


namespace TurnBased.Battle.Managers {

    // �������� ���� �ϴ� Ŭ����
    public class  DamageResult {
        public float BaseDamage;        // �׳� ���� ������
        public float ReducedDamage;     // �������� ������ ������
        public float FinalDamage;       // ���������� �޴� ������
        public float ToughnessDamage;   // ���ε� ������
        public bool IsCrit;            // ġ��Ÿ ����
    }

    // �������� ����ϴ� Ŭ����
    public class CombatManager {

        public static event Action<int> OnSkillPointChanged;
        public static event Action<int> OnSkillPointMaxChanged;

        public static int SkillPoint { get; private set; } = 3;

        public static int SkillPointMax { get; private set; } = 5;

        public static void SetSkillPoint(int p) {
            SkillPoint = Math.Clamp(p, 0, SkillPointMax);
            OnSkillPointChanged?.Invoke(SkillPoint);
        }

        public static void SetSkillPointMax(int pMax) {
            SkillPointMax = pMax;
            OnSkillPointMaxChanged?.Invoke(pMax);
        }

        public static void ModifySkillPoint(int delta) {
            SkillPoint = Math.Clamp(SkillPoint + delta, 0, SkillPointMax);
            OnSkillPointChanged?.Invoke(SkillPoint);
        }

        public static void CleanSkillPointEvents() {
            OnSkillPointChanged = null;
            OnSkillPointMaxChanged = null;
        }
     
        // ������ ���ظ� ����� �Լ� (���� ��� ���� ���� �����´�)
        public static DamageResult CalculateDamage(Character attacker, Character defender, AttackData attackData, int attackNum = 0)
        {
            // ���� ���� ���ݷ��� �����´�
            float baseDamage = attacker.Data.Attack.Current * attackData.damageMult[attackNum];

            float reducedPercentage = defender.Data.Defense.Current / (defender.Data.Defense.Current + 1000);

            float victimDef = defender.Data.Defense.Current;

            bool isCrit = false;
            if (attackData.canCrit && Random.Range(float.Epsilon, 1f) <= attacker.Data.CritChance.Current) {
                baseDamage *= attacker.Data.CritMult.Current;
                isCrit = true;
            }
            float damageReduction = victimDef / (victimDef + 1000f);

            float damageReduced = baseDamage * damageReduction;

            // DamageResult�� ��ȯ�Ѵ�
            return new DamageResult
            {
                BaseDamage = baseDamage,
                ReducedDamage = damageReduced,
                FinalDamage = baseDamage - damageReduced,
                ToughnessDamage = attackData.toughnessDamage[attackNum],
                IsCrit = isCrit
            };
        }

        public static DamageResult CalculateTrueDamage(float damage, float toughnessDamage) {
            return new DamageResult {
                BaseDamage = damage,
                ReducedDamage = 0,
                FinalDamage = damage,
                ToughnessDamage = toughnessDamage,
                IsCrit = false
            };
        }

        public static bool CheckElementMatch(ElementType type1, ElementType type2) {
            return (type1 & type2) > 0;
        }

        public static bool CanCharacterUseUlt(Character c) {
            return c.Data.UltPts.Current >= c.Data.UltThreshold;
        }
    }

}