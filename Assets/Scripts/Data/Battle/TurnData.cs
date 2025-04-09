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
        /// 행동 할 캐릭터
        /// </summary>
        public Character Character { get; private set; }
        /// <summary>
        /// 현재 행동력
        /// </summary>
        public float CurrentAV { get; private set; }
        /// <summary>
        /// 행동력 상한까지 현재 속도 스탯으로 도달하는데에 걸리는 시간
        /// </summary>
        public float RemainingTimeToAct { get; private set; }
        /// <summary>
        /// 턴 타입
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
        /// 턴 진행시키기
        /// </summary>
        /// <param name="deltaTime"></param>
        public void AdvanceTurn(float deltaTime) {
            CurrentAV = Mathf.Min(CurrentAV + deltaTime * Character.Data.stats.Speed, AVCap);
            RemainingTimeToAct = (AVCap - CurrentAV) / Character.Data.stats.Speed;
        }
        /// <summary>
        /// 행동력 초기화
        /// </summary>
        public void ResetAV() {
            CurrentAV = 0f;
            RemainingTimeToAct = AVCap / Character.Data.stats.Speed;
        }
    }

}
