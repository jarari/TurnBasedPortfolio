using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnBased.Data;
using UnityEngine;
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
     
        // ������ ���ظ� ����� �Լ� (���� ��� ���� ���� �����´�)
        public static DamageResult CalculateDamage(Character attacker, Character defender, float attackMult = 1f)
        {
            // ���� ���� ���ݷ��� �����´�
            float baseDamage = attacker.Data.Attack.Current * attackMult;

            float reducedPercentage = defender.Data.Defense.Current / (defender.Data.Defense.Current + 1000);

            float victimDef = defender.Data.Defense.Current;

            bool isCrit = false;
            if (Random.Range(float.Epsilon, 1f) <= attacker.Data.CritChance.Current) {
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
                ToughnessDamage = baseDamage,
                IsCrit = isCrit
            };
        }

        public static bool CheckElementMatch(ElementType type1, ElementType type2) {
            return (type1 | type2) > 0;
        }
    }

}