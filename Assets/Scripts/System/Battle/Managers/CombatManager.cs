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
        public static DamageResult DoDamage(Character attacker, Character defender)
        {
            // ���� ���� ���ݷ��� �����´�
            float normalAttack = attacker.Data.stats.Attack;

            // ���� ���� ���� ��ŭ ���� ���� ���ݷ��� ������ �װſ� 0�� �� ū���� ��ȯ�Ѵ�
            float afterDamage = Mathf.Max(0, normalAttack - defender.Data.stats.Defense);

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