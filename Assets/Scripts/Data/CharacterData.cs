using System.Collections.Generic;
using TurnBased.Battle;
using UnityEngine;

namespace TurnBased.Data {
    public enum StatType {
        HP,
        Toughness,
        Attack,
        Defense,
        Speed,
        CritChance,
        CritMult
    }

    [System.Serializable]
    public class BaseStats {
        /// <summary>
        /// 최대 체력
        /// </summary>
        [field: SerializeField]
        public float MaxHP { get; set; }
        /// <summary>
        /// 최대 강인도
        /// </summary>
        [field: SerializeField]
        public float MaxToughness { get; set; }
        /// <summary>
        /// 궁극기 발동 가능 포인트
        /// </summary>
        [field: SerializeField]
        public float UseUltThreshold { get; set; }
        /// <summary>
        /// 궁극기 포인트 최대량
        /// </summary>
        [field: SerializeField]
        public float MaxUltPts { get; set; }
        /// <summary>
        /// 공격력
        /// </summary>
        [field: SerializeField]
        public float Attack { get; set; }
        /// <summary>
        /// 방어력
        /// </summary>
        [field: SerializeField]
        public float Defense { get; set; }
        /// <summary>
        /// 속도 (행동력 계산에 쓰임)
        /// </summary>
        [field: SerializeField]
        public float Speed { get; set; }
        /// <summary>
        /// 치명타 확률
        /// </summary>
        [field: SerializeField]
        public float CritChance { get; set; }
        /// <summary>
        /// 치명타 배율 (기본 150%)
        /// </summary>
        [field: SerializeField]
        public float CritMult { get; set; } = 1.5f;
        /// <summary>
        /// 공격 속성
        /// </summary>
        [field: SerializeField]
        public ElementType ElementType { get; set; }
        /// <summary>
        /// 약점 리스트
        /// </summary>
        [field: SerializeField]
        public ElementType Weakness { get; set; }
    }

    public enum CharacterTeam {
        Player,
        Enemy
    }

    [System.Serializable]
    public class CharacterAttackTable {
        public AttackData normalAttack;
        public AttackData skillAttack;
        public AttackData ultAttack;
        public AttackData extraAttack;
        public List<AttackData> additionalData;
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
    public class CharacterData : ScriptableObject {
        public BaseStats stats;
        public CharacterTeam team;
        public CharacterAttackTable attackTable;
        public GameObject battlePrefab;
        public GameObject fieldPrefab;
        public string Name; // 캐릭터 이름
        public string Attribute; // 캐릭터 속성
        public int BreakEffect; // 격파 특수효과
        public string CharacterImagePath; // 캐릭터 이미지 경로
        public string CharacterRenderTexturePath; // 캐릭터 렌더 텍스쳐 경로
        public string AttributeImagePath; // 속성 이미지 경로
        public string BasicAttackImagePath; // 일반 공격 이미지 경로
        public string SkillImagePath; // 전투 스킬 이미지 경로
        public string UltimateImagePath; // 필살기 이미지 경로
        public string TalentImagePath; // 특성 이미지 경로
        public string TechniqueImagePath; // 비술 이미지 경로
    }
}