using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnBased.Battle.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace TurnBased.Battle.Managers
{
    /// <summary>
    /// 행동력 기반 턴 관리자
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager instance;
        public const float Round1Time = 150f;
        public const float RoundRestTime = 100f;
        /// <summary>
        /// 라운드 시스템 사용 여부
        /// </summary>
        public bool HasRound;
        public int CurrentRound { get; private set; } = 1;
        public int TurnPassed { get; private set; }
        /// <summary>
        /// 현재 행동중인 캐릭터
        /// </summary>
        public Character CurrentCharacter { get; private set; }
        /// <summary>
        /// 라운드 변경 시 이벤트
        /// </summary>
        public event Action OnRoundChanged;
        public event Action<TurnContext> OnBeforeTurnStart;
        public event Action<TurnContext> OnTurnEnd;
        public event Action OnTurnCannotStart;
        public event Action OnTurnQueueChanged;

        private float _roundRemaining;

        private List<TurnData> _turnQueue = new List<TurnData>();
        private TurnType _lastTurnType;

        private int _charactersInDeathSequence;
        private int _aliveAlliesCount;
        private int _aliveEnemiesCount;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            _roundRemaining = Round1Time;
        }

        private void Start()
        {
            CombatManager.instance.OnCharacterDeath += HandleCharacterDeath;
            CombatManager.instance.OnCharacterDeathComplete += HandleCharacterDeathComplete;
        }

        private void HandleCharacterDeath(Character c, Character killer)
        {
            _charactersInDeathSequence++;
            if (c.Data.Team == Data.CharacterTeam.Player)
            {
                _aliveAlliesCount--;
            }
            else if (c.Data.Team == Data.CharacterTeam.Enemy)
            {
                _aliveEnemiesCount--;
            }
        }

        private void HandleCharacterDeathComplete(Character c)
        {
            RemoveCharacter(c);
            _charactersInDeathSequence--;
        }

        /// <summary>
        /// 일반 공격 캐릭터 추가 (호출 할 일 없음)
        /// </summary>
        /// <param name="character"></param>
        public void AddCharacter(Character character)
        {
            _turnQueue.Add(new TurnData(character, TurnType.Normal));
            if (character.Data.Team == Data.CharacterTeam.Player)
            {
                _aliveAlliesCount++;
            }
            else if (character.Data.Team == Data.CharacterTeam.Enemy)
            {
                _aliveEnemiesCount++;
            }
        }

        public void RemoveCharacter(Character character)
        {
            _turnQueue.RemoveAll((data) => data.Character == character);
            _turnQueue.RemoveAll((data) => data.ExtraAttackTarget == character);
            OnTurnQueueChanged?.Invoke();
        }

        public void RemoveCharacterUltExtraAttackOnly(Character character)
        {
            _turnQueue.RemoveAll((data) => data.Character == character && (data.Type == TurnType.Ult || data.Type == TurnType.ExtraAttack));
            OnTurnQueueChanged?.Invoke();
        }

        /// <summary>
        /// 궁극기 턴 추가 (궁극기 발동시 호출)
        /// </summary>
        /// <param name="character"></param>
        public void AddUltTurn(Character character)
        {
            int idx = 0;
            while (idx < _turnQueue.Count && _turnQueue[idx].Type != TurnType.Normal)
                idx++;
            _turnQueue.Insert(idx, new TurnData(character, TurnType.Ult));
            if (idx == 0
                && CurrentCharacter.Data.Team == Data.CharacterTeam.Player
                && CurrentCharacter.WantCmd == true
                && CurrentCharacter.CurrentState != Character.CharacterState.PrepareUltAttack
                && CurrentCharacter.CurrentState != Character.CharacterState.PrepareUltSkill)
            {
                var _characterTurn = _turnQueue.Find((t) => t.Character == CurrentCharacter && t.Type == TurnType.Normal);
                _turnQueue.Remove(_characterTurn);
                _characterTurn.ModRemainingTime(0);
                _turnQueue.Insert(1, _characterTurn);
                TurnPassed--;
                StartNextTurn();
            }
            else {
                OnTurnQueueChanged?.Invoke();
            }
            character.Data.UltPts.ModifyCurrent(-character.Data.UltThreshold);
            SoundManager.instance.Play2DSound("UIUltStandby");
        }

        /// <summary>
        /// 추가 공격 턴 추가 (추가 공격 발동시 호출)
        /// </summary>
        /// <param name="character"></param>
        public void AddExtraAtackTurn(Character character, Character target)
        {
            int idx = 0;
            IEnumerator<TurnData> enumerator = _turnQueue.GetEnumerator();
            while (idx < _turnQueue.Count && _turnQueue[idx].Type == TurnType.ExtraAttack)
                idx++;
            _turnQueue.Insert(idx, new TurnData(character, TurnType.ExtraAttack, target));
            OnTurnQueueChanged?.Invoke();
        }

        public void InitializeTurnQueue()
        {
            _turnQueue = _turnQueue.OrderBy(td => td.RemainingTimeToAct).ToList();
            StartNextTurn();
        }

        /// <summary>
        /// 행동력 순으로 맨 앞에 있는 캐릭터 행동
        /// </summary>
        public void StartNextTurn()
        {
            var first = _turnQueue.First();
            _turnQueue.Remove(first);
            CurrentCharacter = first.Character;
            _lastTurnType = first.Type;
            OnTurnQueueChanged?.Invoke();
            void ProgressTurn()
            {
                if (first.Type == TurnType.Normal)
                {
                    _roundRemaining = _roundRemaining - first.RemainingTimeToAct;
                    foreach (var turnData in _turnQueue)
                    {
                        turnData.AdvanceTurn(first.RemainingTimeToAct);
                    }
                    first.Character.TakeTurn();
                    if (!first.Character.IsDead) {
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
                }
                else if (first.Type == TurnType.Ult)
                {
                    first.Character.TakeUltTurn();
                }
                else if (first.Type == TurnType.ExtraAttack)
                {
                    first.Character.TakeExtraAttackTurn();
                    first.Character.DoExtraAttack(first.ExtraAttackTarget);
                }
            }
            var ctx = new TurnContext(first.Character, first.Type, ProgressTurn);
            OnBeforeTurnStart?.Invoke(ctx);

            if (!ctx.IsPaused)
            {
                ctx.Continue();
            }
            else
            {
                first.Character.TakeTransitionTurn();
            }
        }

        public float GetRemainingTime(Character c)
        {
            foreach (var turnData in _turnQueue)
            {
                if (turnData.Type == TurnType.Normal && turnData.Character == c)
                {
                    return turnData.RemainingTimeToAct;
                }
            }
            return 10000f;
        }

        public void ModRemainingTime(Character c, float f)
        {
            foreach (var turnData in _turnQueue)
            {
                if (turnData.Type == TurnType.Normal && turnData.Character == c)
                {
                    turnData.ModRemainingTime(f);
                    break;
                }
            }
        }

        public void ProcessAVSpeedChange()
        {
            foreach (var turnData in _turnQueue)
            {
                turnData.AdvanceTurn(0);
            }
            _turnQueue = _turnQueue.OrderBy(td => td.RemainingTimeToAct).ToList();
            OnTurnQueueChanged?.Invoke();
        }

        private IEnumerator StartNextTurnDelayed()
        {
            if (_charactersInDeathSequence == 0)
            {
                yield return null;
            }
            else
            {
                CurrentCharacter.ProcessCamChanged();
                foreach (var c in CharacterManager.instance.GetAllCharacters())
                {
                    if (c.CurrentMeshLayer != Character.MeshLayer.Hidden)
                    {
                        c.SetMeshLayer(Character.MeshLayer.Default);
                    }
                }
                yield return new WaitWhile(() => _charactersInDeathSequence > 0);
            }

            if (_aliveAlliesCount == 0 || _aliveEnemiesCount == 0)
            {
                OnTurnCannotStart?.Invoke();
                yield break;
            }

            StartNextTurn();
        }

        /// <summary>
        /// 현재 턴 종료 및 다음 턴 시작
        /// </summary>
        public void EndTurn()
        {
            if (_roundRemaining <= 0)
            {
                _roundRemaining += RoundRestTime;
                CurrentRound++;
                if (HasRound)
                {
                    OnRoundChanged?.Invoke();
                }
            }
            var ctx = new TurnContext(CurrentCharacter, _lastTurnType, () => {
                StartCoroutine(StartNextTurnDelayed());
                TurnPassed++;
            });
            OnTurnEnd?.Invoke(ctx);

            if (!ctx.IsPaused)
            {
                ctx.Continue();
            }
            else
            {
                CurrentCharacter.TakeTransitionTurn();
            }
        }

        /// <summary>
        /// 행동 후 예측 턴 리스트
        /// </summary>
        /// <returns></returns>
        public List<TurnData> GetPredictedTurnQueue()
        {
            List<TurnData> predictedQueue = new List<TurnData>();
            foreach (var data in _turnQueue)
            {
                predictedQueue.Add(new TurnData(data));
            }
            var first = predictedQueue.First();
            if (first.Type == TurnType.Normal)
            {
                first.ResetAV();
                bool inserted = false;
                for (int i = 1; i < predictedQueue.Count; ++i)
                {
                    if (predictedQueue[i].RemainingTimeToAct > first.RemainingTimeToAct)
                    {
                        predictedQueue.Insert(i, new TurnData(first));
                        inserted = true;
                        break;
                    }
                }
                if (!inserted)
                {
                    _turnQueue.Add(first);
                }
            }
            predictedQueue.Remove(first);
            predictedQueue = predictedQueue.OrderBy(td => td.RemainingTimeToAct).ToList();
            return predictedQueue;
        }

        public List<TurnData> GetActionOrder()
        {
            List<TurnData> ret = _turnQueue.ToList();
            if (CurrentCharacter != null) {
                var td = new TurnData(CurrentCharacter, _lastTurnType);
                td.ModRemainingTime(0);
                ret.Insert(0, td);
            }
            return ret; // 행동 서열을 리스트로 반환
        }

        public void PrintTurnQueue()
        {
            Debug.Log("=== Turn Queue ===");
            foreach (var td in _turnQueue)
            {
                Debug.Log($"{td.Character.name}: AV = {td.CurrentAV}, Remaining = {td.RemainingTimeToAct}");
            }
        }
    }
}
