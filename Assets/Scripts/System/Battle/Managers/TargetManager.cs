using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnBased.Data;
using static UnityEngine.EventSystems.EventTrigger;
using NUnit.Framework;

namespace TurnBased.Battle.Managers {
    /// <summary>
    /// 조준된 타겟 관리
    /// </summary>
    public class TargetManager : MonoBehaviour {
        public static TargetManager instance;

        public enum TargetMode {
            Self,
            Single,
            Triple,
            All
        }

        public enum TargetFilter {
            AliveOnly,
            DeadOnly,
            All
        }

        public class SearchResult {
            public Character Character { get; set; }
            public int Index { get; set; }
            public SearchResult(Character c, int idx) {
                Character = c;
                Index = idx;
            }
        }

        public GameObject camTarget;
        public GameObject camTargetAlly;
        public List<Transform> camPos;
        public List<Transform> camPosAlly;

        public Action<Character> OnTargetChanged;
        public Action OnTargetSettingChanged;
        public Action<float> OnCamTargetUpdate;

        /// <summary>
        /// 타게팅 된 캐릭터
        /// </summary>
        public Character Target { get; private set; }
        /// <summary>
        /// 캐릭터의 칸 번호
        /// </summary>
        public int TargetIndex { get; private set; }
        public CharacterTeam TargetTeam { get; private set; }
        public TargetMode Mode { get; private set; }
        public TargetFilter Filter { get; private set; }
        /// <summary>
        /// 선형보간된 칸 포지션
        /// </summary>
        public float InterpolatedTargetPos { get; private set; }

        private const float _targetTrackTime = 0.1f;
        private float _targetTrackWeight = 0f;
        private float _prevTarget = 2f;
        private float _currentTarget = 2f;
        private float _targetAllDollyClampMin = 1.5f;
        private float _targetAllDollyClampMax = 2.5f;
        private Coroutine _targetCoroutine;

        private Func<Character, bool> _targetFilter;

        private enum SearchDirection {
            Left,
            Right
        }

        private void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }
            instance = this;
        }

        private IEnumerator TrackTarget() {
            while (InterpolatedTargetPos != _currentTarget) {
                _targetTrackWeight = Mathf.Min(_targetTrackWeight + Time.deltaTime / _targetTrackTime, 1f);
                InterpolatedTargetPos = Mathf.Lerp(_prevTarget, _currentTarget, _targetTrackWeight);
                int posInt = Mathf.FloorToInt(InterpolatedTargetPos);
                float posFloat = InterpolatedTargetPos - posInt;
                var posList = camPos;
                var camObj = camTarget;
                if (TargetTeam == CharacterTeam.Player) {
                    posList = camPosAlly;
                    camObj = camTargetAlly;
                }
                if (posInt < posList.Count - 1) {
                    camObj.transform.position = Vector3.Lerp(posList[posInt].position, posList[posInt + 1].position, posFloat);
                }
                else {
                    camObj.transform.position = posList[posList.Count - 1].position;
                }
                OnCamTargetUpdate?.Invoke(InterpolatedTargetPos);
                yield return null;
            }
            _targetCoroutine = null;
        }

        private void StartTargetPosUpdate() {
            _targetTrackWeight = 0f;
            _prevTarget = _currentTarget;
            _currentTarget = TargetIndex;
            if (Mode == TargetMode.All && TargetTeam == CharacterTeam.Enemy) {
                _currentTarget = Mathf.Clamp(_currentTarget, _targetAllDollyClampMin, _targetAllDollyClampMax);
            }
            if (_targetCoroutine == null) {
                _targetCoroutine = StartCoroutine(TrackTarget());
            }
        }

        private void UpdateTarget(Character c, int idx, bool interpolate = true) {
            Target = c;
            TargetIndex = idx;
            if (interpolate) {
                StartTargetPosUpdate();
            }
            else {
                _currentTarget = idx;
                InterpolatedTargetPos = idx;
                if (TargetTeam == CharacterTeam.Enemy) {
                    camTarget.transform.position = camPos[idx].position;
                }
                else {
                    camTargetAlly.transform.position = camPosAlly[idx].position;
                }
                OnCamTargetUpdate?.Invoke(InterpolatedTargetPos);
            }
            OnTargetChanged?.Invoke(Target);
        }

        public void ChangeTargetSetting(TargetMode mode, CharacterTeam team = CharacterTeam.Player, TargetFilter filter = TargetFilter.AliveOnly) {
            bool shouldInit = Target == null || Target.IsDead || mode == TargetMode.Self || team != TargetTeam || Filter != filter ? true : false;
            Mode = mode;
            if (mode == TargetMode.Self) {
                TargetTeam = CharacterTeam.Player;
            }
            else {
                TargetTeam = team;
            }
            Filter = filter;
            if (filter == TargetFilter.AliveOnly) {
                _targetFilter = c => !c.IsDead;
            }
            else if (filter == TargetFilter.DeadOnly) {
                _targetFilter = c => c.IsDead;
            }
            else {
                _targetFilter = null;
            }
            OnTargetSettingChanged?.Invoke();
            if (shouldInit) {
                InitializeTarget();
            }
            else {
                UpdateTarget(Target, TargetIndex);
            }
        }

        public void InitializeTarget() {
            if (Mode == TargetMode.Self) {
                var c = TurnManager.instance.CurrentCharacter;
                UpdateTarget(c, CharacterManager.instance.GetAllyIndex(c), false);
            }
            else {
                if (TargetTeam == CharacterTeam.Enemy) {
                    int[] idxToTry = { 2, 1, 3, 0, 4 };
                    Character enemy;
                    int tryIdx = 0;
                    do {
                        enemy = CharacterManager.instance.GetEnemyAtIndex(idxToTry[tryIdx]);
                        if (enemy == null || (_targetFilter != null && !_targetFilter(enemy))) {
                            enemy = null;
                            tryIdx++;
                        }
                    } while (enemy == null && tryIdx < idxToTry.Length);
                    if (enemy == null) {
                        return;
                    }
                    UpdateTarget(enemy, idxToTry[tryIdx], false);
                }
                else {
                    int[] idxToTry = { 1, 0, 2 };
                    Character ally;
                    int tryIdx = 0;
                    do {
                        ally = CharacterManager.instance.GetAllyAtIndex(idxToTry[tryIdx]);
                        if (ally == null || (_targetFilter != null && !_targetFilter(ally))) {
                            ally = null;
                            tryIdx++;
                        }
                    } while (ally == null && tryIdx < idxToTry.Length);
                    if (ally == null) {
                        return;
                    }
                    UpdateTarget(ally, idxToTry[tryIdx], false);
                }
            }
        }

        public List<Character> GetTargets() {
            if (Mode == TargetMode.All) {
                if (TargetTeam == CharacterTeam.Enemy) {
                    if (Filter == TargetFilter.AliveOnly) {
                        return CharacterManager.instance.GetEnemyCharacters();
                    }
                    else if (Filter == TargetFilter.DeadOnly) {
                        List<Character> targets = new List<Character>();
                        foreach (var c in CharacterManager.instance.GetAllEnemyCharacters()) {
                            if (c.IsDead) {
                                targets.Add(c);
                            }
                        }
                        return targets;
                    }
                    else {
                        return CharacterManager.instance.GetAllEnemyCharacters();
                    }
                }
                else {
                    if (Filter == TargetFilter.AliveOnly) {
                        return CharacterManager.instance.GetAllyCharacters();
                    }
                    else if (Filter == TargetFilter.DeadOnly) {
                        List<Character> targets = new List<Character>();
                        foreach (var c in CharacterManager.instance.GetAllAllyCharacters()) {
                            if (c.IsDead) {
                                targets.Add(c);
                            }
                        }
                        return targets;
                    }
                    else {
                        return CharacterManager.instance.GetAllAllyCharacters();
                    }
                }
            }
            else if (Mode == TargetMode.Self || Mode == TargetMode.Single) {
                return new List<Character>() { Target };
            }
            else {
                List<Character> targets = new List<Character>();
                var onLeft = GetCharacterOnLeft(Target);
                var onRight = GetCharacterOnRight(Target);
                if (onLeft.Character != null) {
                    targets.Add(onLeft.Character);
                }
                targets.Add(Target);
                if (onRight.Character != null) {
                    targets.Add(onRight.Character);
                }
                return targets;
            }
        }

        private SearchResult GetCharacterOn_Internal(Character c, SearchDirection dir) {
            if (c.Data.team == CharacterTeam.Enemy) {
                int d = dir == SearchDirection.Left ? -1 : 1;
                int idxToTry = CharacterManager.instance.GetEnemyIndex(c) + d;
                int enemyCount = CharacterManager.instance.GetMaxEnemyCount();
                Character enemy = null;
                while (enemy == null && idxToTry >= 0 && idxToTry < enemyCount) {
                    enemy = CharacterManager.instance.GetEnemyAtIndex(idxToTry);
                    if (enemy == null || (_targetFilter != null && !_targetFilter(enemy))) {
                        enemy = null;
                        idxToTry += d;
                    }
                }
                return new SearchResult(enemy, idxToTry);
            }
            else {
                int d = dir == SearchDirection.Left ? 1 : -1;
                int idxToTry = CharacterManager.instance.GetAllyIndex(c) + d;
                int allyCount = CharacterManager.instance.GetMaxAllyCount();
                Character ally = null;
                while (ally == null && idxToTry >= 0 && idxToTry < allyCount) {
                    ally = CharacterManager.instance.GetAllyAtIndex(idxToTry);
                    if (ally == null || (_targetFilter != null && !_targetFilter(ally))) {
                        ally = null;
                        idxToTry += d;
                    }
                }
                return new SearchResult(ally, idxToTry);
            }
        }

        public SearchResult GetCharacterOnLeft(Character c) {
            return GetCharacterOn_Internal(c, SearchDirection.Left);
        }

        public SearchResult GetCharacterOnRight(Character c) {
            return GetCharacterOn_Internal(c, SearchDirection.Right);
        }

        public bool SelectLeftTarget() {
            if (Mode == TargetMode.Self) {
                return false;
            }
            var res = GetCharacterOnLeft(Target);
            if (res.Character != null) {
                UpdateTarget(res.Character, res.Index);
                return true;
            }
            return false;
        }

        public bool SelectRightTarget() {
            if (Mode == TargetMode.Self) {
                return false;
            }
            var res = GetCharacterOnRight(Target);
            if (res.Character != null) {
                UpdateTarget(res.Character, res.Index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 무작위 플레이어 하나를 타겟으로 설정하는 함수
        /// </summary>
        public Character SetPlayerTarget()
        { 
            // 살아 있는 플레이어 리스트를 가져오고
            List<Character> player_list = CharacterManager.instance.GetAllyCharacters();
            // 만약 그런 플레이어가 없다면
            if (player_list == null || player_list.Count == 0)
            {
                // null을 반환 한다
                return null;
            }
            // 유니티의 랜덤 클래스를 사용해서 0부터 살아있는 플레이어 숫자까지 랜덤한 숫자를 고른다
            int rand = UnityEngine.Random.Range(0, player_list.Count);
            // 랜덤하게 선택된 플레이어를 하나 가져오고
            Character player_target = player_list[rand];
            
            // 살아있는 캐릭터를 반환한다
            return player_target;
        }

        /// <summary>
        /// 모든 플레이어를 타겟으로 하는 함수
        /// </summary>
        /// <returns></returns>
        public List<Character> SetMPlayerTarget()
        {
            // 살아있는 플레이어 리스트를 가져오고
            List<Character> player_list = CharacterManager.instance.GetAllyCharacters();
            // 만약 그런 플레이어가 없다면
            if (player_list == null || player_list.Count == 0)
            {
                // null을 반환 한다
                return null;
            }
            // 플레이어 리스트를 반환한다
            return player_list;
        }

    }
}
