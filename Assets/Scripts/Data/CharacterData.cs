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
        /// ���� ���ε�
        /// </summary>
        public float CurrentToughness { get; set; }
        /// <summary>
        /// �ִ� ���ε�
        /// </summary>
        [field: SerializeField]
        public float MaxToughness { get; set; }
        /// <summary>
        /// ���� �ñر� ����Ʈ
        /// </summary>
        public float CurrentUltPts { get; set; }
        /// <summary>
        /// �ñر� �ߵ� ���� ����Ʈ
        /// </summary>
        [field: SerializeField]
        public float UseUltThreshold { get; set; }
        /// <summary>
        /// ���ݷ�
        /// </summary>
        [field: SerializeField]
        public float Attack { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [field: SerializeField]
        public float Defense { get; set; }
        /// <summary>
        /// �ӵ� (�ൿ�� ��꿡 ����)
        /// </summary>
        [field: SerializeField]
        public float Speed { get; set; }
        /// <summary>
        /// ���� �Ӽ�
        /// </summary>
        [field: SerializeField]
        public ElementType ElementType { get; set; }
        /// <summary>
        /// ���� ����Ʈ
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