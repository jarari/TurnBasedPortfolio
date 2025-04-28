using UnityEngine;

namespace TurnBased.Data {
    public enum StatType {
        HP,
        Toughness,
        Attack,
        Defense,
        Speed
    }

    [System.Serializable]
    public struct BaseStats {
        public float CurrentHP { get; set; }
        [field: SerializeField]
        public float MaxHP { get; set; }
        /// <summary>
        /// 현재 강인도
        /// </summary>
        public float CurrentToughness { get; set; }
        /// <summary>
        /// 최대 강인도
        /// </summary>
        [field: SerializeField]
        public float MaxToughness { get; set; }
        /// <summary>
        /// 현재 궁극기 포인트
        /// </summary>
        public float CurrentUltPts { get; set; }
        /// <summary>
        /// 궁극기 발동 가능 포인트
        /// </summary>
        [field: SerializeField]
        public float UseUltThreshold { get; set; }
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

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
    public class CharacterData : ScriptableObject {
        public BaseStats stats;
        public CharacterTeam team;
    }
}