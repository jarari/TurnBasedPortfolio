using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TurnBased.Battle.Managers {
    public class TurnManager : MonoBehaviour {
        public static TurnManager instance;
        public const float Round1Time = 150f;
        public const float RoundRestTime = 100f;

        public bool HasRound;
        public int CurrentRound { get; private set; } = 1;
        public Character CurrentCharacter { get; private set; }

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

        public void AddCharacter(Character character) {
            _turnQueue.Add(new TurnData(character, TurnType.Normal));
        }

        public void AddUltTurn(Character character) {
            _turnQueue.Insert(0, new TurnData(character, TurnType.Ult));
        }

        public void AddExtraAtackTurn(Character character) {
            _turnQueue.Insert(0, new TurnData(character, TurnType.ExtraAttack));
        }

        public void InitializeTurnQueue() {
            _turnQueue = _turnQueue.OrderBy(td => td.RemainingTimeToAct).ToList();
            _roundRemaining = Round1Time;
            StartNextTurn();
        }

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
                PrintTurnQueue();
            }
            else if (first.Type == TurnType.Ult) {
                first.Character.PrepareUlt();
            }
            else if (first.Type == TurnType.ExtraAttack) {
                first.Character.DoExtraAttack();
            }
        }

        private IEnumerator StartNextTurnDelayed() {
            yield return null;
            StartNextTurn();
        }

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

        public List<TurnData> GetPredictedTurnQueue() {
            List<TurnData> predictedQueue = new List<TurnData>();
            foreach (var data in _turnQueue) {
                predictedQueue.Add(new TurnData(data));
            }
            var first = predictedQueue.First();
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
