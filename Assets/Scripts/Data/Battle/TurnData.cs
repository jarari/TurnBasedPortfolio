using UnityEngine;

namespace TurnBased.Battle {

    public enum TurnType {
        Normal,
        Ult,
        ExtraAttack
    }

    public class TurnData {
        public const float AVCap = 10000f;
        /// <summary>
        /// �ൿ �� ĳ����
        /// </summary>
        public Character Character { get; private set; }
        /// <summary>
        /// ���� �ൿ��
        /// </summary>
        public float CurrentAV { get; private set; }
        /// <summary>
        /// �ൿ�� ���ѱ��� ���� �ӵ� �������� �����ϴµ��� �ɸ��� �ð�
        /// </summary>
        public float RemainingTimeToAct { get; private set; }
        /// <summary>
        /// �� Ÿ��
        /// </summary>
        public TurnType Type { get; private set; }

        public TurnData(Character character, TurnType type) {
            Character = character;
            ResetAV();
            Type = type;
        }

        public TurnData(TurnData clone) {
            Character = clone.Character;
            CurrentAV = clone.CurrentAV;
            RemainingTimeToAct = clone.RemainingTimeToAct;
            Type = clone.Type;
        }
        /// <summary>
        /// �� �����Ű��
        /// </summary>
        /// <param name="deltaTime"></param>
        public void AdvanceTurn(float deltaTime) {
            CurrentAV = Mathf.Min(CurrentAV + deltaTime * Character.Data.stats.Speed, AVCap);
            RemainingTimeToAct = (AVCap - CurrentAV) / Character.Data.stats.Speed;
        }
        /// <summary>
        /// �ൿ�� �ʱ�ȭ
        /// </summary>
        public void ResetAV() {
            CurrentAV = 0f;
            RemainingTimeToAct = AVCap / Character.Data.stats.Speed;
        }
    }

}
