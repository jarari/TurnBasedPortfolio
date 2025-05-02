using System;
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
        public Character ExtraAttackTarget { get; private set; }

        public TurnData(Character character, TurnType type, Character extraAttackTarget = null) {
            Character = character;
            Type = type;
            if (type == TurnType.Normal) {
                ResetAV();
            }
            else {
                CurrentAV = AVCap;
                RemainingTimeToAct = 0f;
            }
            ExtraAttackTarget = extraAttackTarget;
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
            CurrentAV = Mathf.Min(CurrentAV + deltaTime * Character.Data.Speed.Current, AVCap);
            RemainingTimeToAct = (AVCap - CurrentAV) / Character.Data.Speed.Current;
        }

        /// <summary>
        /// �ൿ ������ ����
        /// </summary>
        /// <param name="time"></param>
        public void ModRemainingTime(float time) {
            RemainingTimeToAct = time;
            CurrentAV = AVCap - RemainingTimeToAct * Character.Data.Speed.Current;
        }

        /// <summary>
        /// �ൿ�� �ʱ�ȭ
        /// </summary>
        public void ResetAV() {
            CurrentAV = 0f;
            RemainingTimeToAct = AVCap / Character.Data.Speed.Current;
        }
    }


    public class TurnContext {
        public Character Character { get; }
        public TurnType Type { get; }

        public int PauseCount { get; private set; }

        public bool IsPaused {
            get {
                return PauseCount > 0;
            }
        }

        private bool _hasContinued;
        private readonly Action _continueCallback;

        public TurnContext(Character ch, TurnType t, Action continueCallback) {
            Character = ch;
            Type = t;
            _continueCallback = continueCallback;
        }

        public void Pause() {
            PauseCount++;
        }

        public void Continue() {
            PauseCount--;
            if (_hasContinued || PauseCount > 0) return;
            _hasContinued = true;
            _continueCallback();
        }
    }
}
