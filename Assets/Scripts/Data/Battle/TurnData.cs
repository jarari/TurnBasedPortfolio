using UnityEngine;

namespace TurnBased.Battle {

    public enum TurnType {
        Normal,
        Ult,
        ExtraAttack
    }

    public class TurnData {
        public const float AVCap = 10000f;

        public Character Character { get; private set; }
        public float CurrentAV { get; private set; }
        public float RemainingTimeToAct { get; private set; }
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

        public void AdvanceTurn(float deltaTime) {
            CurrentAV = Mathf.Min(CurrentAV + deltaTime * Character.Data.stats.Speed, AVCap);
            RemainingTimeToAct = (AVCap - CurrentAV) / Character.Data.stats.Speed;
        }

        public void ResetAV() {
            CurrentAV = 0f;
            RemainingTimeToAct = AVCap / Character.Data.stats.Speed;
        }
    }

}
