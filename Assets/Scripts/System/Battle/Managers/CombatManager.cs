using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnBased.Data;
using UnityEngine;
using Random = UnityEngine.Random;


namespace TurnBased.Battle.Managers {

    // 데미지를 정의 하는 클래스
    public class  DamageResult {
        public float BaseDamage;        // 그냥 공격 데미지
        public float ReducedDamage;     // 방어력으로 감소한 데미지
        public float FinalDamage;       // 최종적으로 받는 데미지
        public float ToughnessDamage;   // 강인도 데미지
        public bool IsCrit;            // 치명타 여부
    }

    // 데미지를 계산하는 클래스
    public class CombatManager {
     
        // 데미지 피해를 계산할 함수 (때린 놈과 맞은 놈을 가져온다)
        public static DamageResult CalculateDamage(Character attacker, Character defender, float attackMult = 1f)
        {
            // 때린 놈의 공격력을 가져온다
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

            // DamageResult를 반환한다
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