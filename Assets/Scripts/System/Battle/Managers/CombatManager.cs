using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnBased.Data;
using UnityEngine;


namespace TurnBased.Battle.Managers {

    // 데미지를 정의 하는 클래스
    public class  DamageResult {
        public float NormalAttack;  // 그냥 공격 데미지
        public float AfterDamage;   // 방어력 으로 한번 억제된 데미지
         public float FinalDamage;   // 최종적으로 받는 데미지
    }

    // 데미지를 계산하는 클래스
    public class CombatManager {
     
        // 데미지 피해를 계산할 함수 (때린 놈과 맞은 놈을 가져온다)
        public static DamageResult CalculateDamage(Character attacker, Character defender, float attackMult = 1f)
        {
            // 때린 놈의 공격력을 가져온다
            float normalAttack = attacker.Data.Attack.Current * attackMult;

            // 맞은 놈의 방어력 만큼 때린 놈의 공격력을 내리고 그거와 0중 더 큰값을 반환한다
            float afterDamage = Mathf.Max(0, normalAttack - defender.Data.Defense.Current);

            // 최종적으로 받을 데미지를 가져온다
            float finalDamage = afterDamage;

            // DamageResult를 반환한다
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