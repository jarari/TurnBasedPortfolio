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
        /// �ִ� ü��
        /// </summary>
        [field: SerializeField]
        public float MaxHP { get; set; }
        /// <summary>
        /// �ִ� ���ε�
        /// </summary>
        [field: SerializeField]
        public float MaxToughness { get; set; }
        /// <summary>
        /// �ñر� �ߵ� ���� ����Ʈ
        /// </summary>
        [field: SerializeField]
        public float UseUltThreshold { get; set; }
        /// <summary>
        /// �ñر� ����Ʈ �ִ뷮
        /// </summary>
        [field: SerializeField]
        public float MaxUltPts { get; set; }
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
        /// ġ��Ÿ Ȯ��
        /// </summary>
        [field: SerializeField]
        public float CritChance { get; set; }
        /// <summary>
        /// ġ��Ÿ ���� (�⺻ 150%)
        /// </summary>
        [field: SerializeField]
        public float CritMult { get; set; } = 1.5f;
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
    }
}