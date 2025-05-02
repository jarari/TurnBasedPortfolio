using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnBased.Data;
using UnityEngine;


namespace TurnBased.Battle.Managers {

    // �������� ���� �ϴ� Ŭ����
    public class  DamageResult {
        public float NormalAttack;  // �׳� ���� ������
        public float AfterDamage;   // ���� ���� �ѹ� ������ ������
         public float FinalDamage;   // ���������� �޴� ������
    }

    // �������� ����ϴ� Ŭ����
    public class CombatManager {
     
        // ������ ���ظ� ����� �Լ� (���� ��� ���� ���� �����´�)
        public static DamageResult CalculateDamage(Character attacker, Character defender, float attackMult = 1f)
        {
            // ���� ���� ���ݷ��� �����´�
            float normalAttack = attacker.Data.Attack.Current * attackMult;

            // ���� ���� ���� ��ŭ ���� ���� ���ݷ��� ������ �װſ� 0�� �� ū���� ��ȯ�Ѵ�
            float afterDamage = Mathf.Max(0, normalAttack - defender.Data.Defense.Current);

            // ���������� ���� �������� �����´�
            float finalDamage = afterDamage;

            // DamageResult�� ��ȯ�Ѵ�
            return new DamageResult
            {
                NormalAttack = normalAttack,
                AfterDamage = afterDamage,
                FinalDamage = finalDamage
            };
        }

        public static bool CheckElementMatch(ElementType type1, ElementType type2) {
            return (type1 | type2) > 0;
        }
    }

}