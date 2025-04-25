using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TurnBased.Battle.Managers {
    /// <summary>
    /// �ൿ�� ��� �� ������
    /// </summary>
    public class TurnManager : MonoBehaviour {
        public static TurnManager instance;
        public const float Round1Time = 150f;
        public const float RoundRestTime = 100f;
        /// <summary>
        /// ���� �ý��� ��� ����
        /// </summary>
        public bool HasRound;
        public int CurrentRound { get; private set; } = 1;
        /// <summary>
        /// ���� �ൿ���� ĳ����
        /// </summary>
        public Character CurrentCharacter { get; private set; }
        /// <summary>
        /// ���� ���� �� �̺�Ʈ
        /// </summary>
        public Action OnRoundChanged;

        private float _roundRemaining;

        private List<TurnData> _turnQueue = new List<TurnData>();

        private void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }
            instance = this;
        }

        /// <summary>
        /// �Ϲ� ���� ĳ���� �߰� (ȣ�� �� �� ����)
        /// </summary>
        /// <param name="character"></param>
        public void AddCharacter(Character character) {
            _turnQueue.Add(new TurnData(character, TurnType.Normal));
        }

        public void RemoveCharacter(Character character) {
            _turnQueue.RemoveAll((data) => data.Character == character);
        }

        public void RemoveCharacterUltExtraAttackOnly(Character character) {
            _turnQueue.RemoveAll((data) => data.Character == character && (data.Type == TurnType.Ult || data.Type == TurnType.ExtraAttack));
        }

        /// <summary>
        /// �ñر� �� �߰� (�ñر� �ߵ��� ȣ��)
        /// </summary>
        /// <param name="character"></param>
        public void AddUltTurn(Character character) {
            int idx = 0;
            IEnumerator<TurnData> enumerator = _turnQueue.GetEnumerator();
            while (enumerator.MoveNext() && enumerator.Current.Type != TurnType.Normal) {
                idx++;
                enumerator.MoveNext();
            }
            _turnQueue.Insert(idx, new TurnData(character, TurnType.Ult));
            if (CurrentCharacter.WantCmd == true 
                && CurrentCharacter.CurrentState != Character.CharacterState.PrepareUltAttack 
                && CurrentCharacter.CurrentState != Character.CharacterState.PrepareUltSkill) {
                var _characterTurn = _turnQueue.Find((t) => t.Character == CurrentCharacter && t.Type == TurnType.Normal);
                _turnQueue.Remove(_characterTurn);
                _characterTurn.ModRemainingTime(0);
                _turnQueue.Insert(1, _characterTurn);
                StartNextTurn();
            }
            SoundManager.instance.Play2DSound("UIUltStandby");
        }

        /// <summary>
        /// �߰� ���� �� �߰� (�߰� ���� �ߵ��� ȣ��)
        /// </summary>
        /// <param name="character"></param>
        public void AddExtraAtackTurn(Character character) {
            _turnQueue.Insert(0, new TurnData(character, TurnType.ExtraAttack));
        }

        public void InitializeTurnQueue() {
            _turnQueue = _turnQueue.OrderBy(td => td.RemainingTimeToAct).ToList();
            _roundRemaining = Round1Time;
            StartNextTurn();
        }

        /// <summary>
        /// �ൿ�� ������ �� �տ� �ִ� ĳ���� �ൿ
        /// </summary>
        public void StartNextTurn() {
            var first = _turnQueue.First();
            _turnQueue.Remove(first);
            CurrentCharacter = first.Character;
            if (first.Type == TurnType.Normal) {
                _roundRemaining = _roundRemaining - first.RemainingTimeToAct;
                foreach (var turnData in _turnQueue) {
                    turnData.AdvanceTurn(first.RemainingTimeToAct);
                }
                first.Character.TakeTurn();
                first.ResetAV();
                bool inserted = false;
                for (int i = 0; i < _turnQueue.Count; ++i) {
                    if (_turnQueue[i].RemainingTimeToAct > first.RemainingTimeToAct) {
                        _turnQueue.Insert(i, new TurnData(first));
                        inserted = true;
                        break;
                    }
                }
                if (!inserted) {
                    _turnQueue.Add(first);
                }
                _turnQueue = _turnQueue.OrderBy(td => td.RemainingTimeToAct).ToList();
            }
            else if (first.Type == TurnType.Ult) {
                first.Character.TakeUltTurn();
            }
            else if (first.Type == TurnType.ExtraAttack) {
                first.Character.DoExtraAttack();
            }
        }

        public float GetRemainingTime(Character c) {
            foreach (var turnData in _turnQueue) {
                if (turnData.Type == TurnType.Normal && turnData.Character == c) {
                    return turnData.RemainingTimeToAct;
                }
            }
            return 10000f;
        }

        public void ModRemainingTime(Character c, float f) {
            foreach (var turnData in _turnQueue) {
                if (turnData.Type == TurnType.Normal && turnData.Character == c) {
                    turnData.ModRemainingTime(f);
                    break;
                }
            }
        }

        public void ProcessAVSpeedChange() {
            foreach (var turnData in _turnQueue) {
                turnData.AdvanceTurn(0);
            }
            _turnQueue = _turnQueue.OrderBy(td => td.RemainingTimeToAct).ToList();
        }

        private IEnumerator StartNextTurnDelayed() {
            yield return null;
            StartNextTurn();
        }

        /// <summary>
        /// ���� �� ���� �� ���� �� ����
        /// </summary>
        public void EndTurn() {
            if (_roundRemaining <= 0) {
                _roundRemaining += RoundRestTime;
                CurrentRound++;
                if (HasRound) {
                    OnRoundChanged?.Invoke();
                }
            }
            StartCoroutine(StartNextTurnDelayed());
        }

        /// <summary>
        /// �ൿ �� ���� �� ����Ʈ
        /// </summary>
        /// <returns></returns>
        public List<TurnData> GetPredictedTurnQueue() {
            List<TurnData> predictedQueue = new List<TurnData>();
            foreach (var data in _turnQueue) {
                predictedQueue.Add(new TurnData(data));
            }
            var first = predictedQueue.First();
            if (first.Type == TurnType.Normal) {
                first.ResetAV();
                bool inserted = false;
                for (int i = 1; i < predictedQueue.Count; ++i) {
                    if (predictedQueue[i].RemainingTimeToAct > first.RemainingTimeToAct) {
                        predictedQueue.Insert(i, new TurnData(first));
                        inserted = true;
                        break;
                    }
                }
                if (!inserted) {
                    _turnQueue.Add(first);
                }
            }
            predictedQueue.Remove(first);
            predictedQueue = predictedQueue.OrderBy(td => td.RemainingTimeToAct).ToList();
            return predictedQueue;
        }

        public void PrintTurnQueue() {
            Debug.Log("=== Turn Queue ===");
            foreach (var td in _turnQueue) {
                Debug.Log($"{td.Character.name}: AV = {td.CurrentAV}, Remaining = {td.RemainingTimeToAct}");
            }
        }
    }
}
