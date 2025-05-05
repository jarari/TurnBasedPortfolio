using System;
using System.Collections.Generic;
using TurnBased.Data;
using UnityEngine;

namespace TurnBased.Battle.Managers {
    /// <summary>
    /// 등록된 캐릭터 관리
    /// </summary>
    public class CharacterManager : MonoBehaviour {
        public static CharacterManager instance;

        public List<GameObject> allySpawnPoints = new List<GameObject>();
        public List<GameObject> enemySpawnPoints = new List<GameObject>();

        public event Action<Character, int> OnCharacterSpawn;

        private List<Character> _characters = new List<Character>();
        private Dictionary<Character, int> _allyIdxDict = new Dictionary<Character, int>();
        private Dictionary<int, Character> _idxAllyDict = new Dictionary<int, Character>();
        private Dictionary<Character, int> _enemyIdxDict = new Dictionary<Character, int>();
        private Dictionary<int, Character> _idxEnemyDict = new Dictionary<int, Character>();

        private void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }
            instance = this;
        }

        private void Start() {
            CombatManager.instance.OnCharacterDeathComplete += HandleCharacterDeathComplete;
        }

        private void HandleCharacterDeathComplete(Character c) {
            RemoveCharacter(c);
        }

        public Character SpawnCharacter(string name) {
            var data = CharacterDataManager.Instance.GetCharacterData(name);
            if (data == null) {
                return null;
            }

            GameObject go, spawnPoint;
            Character c;
            if (data.team == CharacterTeam.Player) {
                int[] idxToTry = { 0, 1, 2 };
                int tryIdx = 0;
                while (GetAllyAtIndex(idxToTry[tryIdx]) != null && tryIdx < idxToTry.Length) {
                    tryIdx++;
                }
                if (tryIdx == idxToTry.Length) {
                    return null;
                }
                int spawnIdx = idxToTry[tryIdx];
                spawnPoint = allySpawnPoints[spawnIdx];

                go = Instantiate(data.battlePrefab);
                go.transform.SetParent(spawnPoint.transform);
                c = go.GetComponentInParent<Character>();

                TurnManager.instance.AddCharacter(c);
                var contextCam = c.GetComponentInParent<ContextualIdleCamera>();
                if (contextCam != null) {
                    contextCam.InitializeCamera();
                }
                _characters.Add(c);
                _allyIdxDict.Add(c, spawnIdx);
                _idxAllyDict.Add(spawnIdx, c);
                OnCharacterSpawn?.Invoke(c, spawnIdx);
            }
            else {
                int[] idxToTry = { 2, 1, 3, 0, 4 };
                int tryIdx = 0;
                while (GetEnemyAtIndex(idxToTry[tryIdx]) != null && tryIdx < idxToTry.Length) {
                    tryIdx++;
                }
                if (tryIdx == idxToTry.Length) {
                    return null;
                }
                int spawnIdx = idxToTry[tryIdx];
                spawnPoint = enemySpawnPoints[spawnIdx];

                go = Instantiate(data.battlePrefab);
                go.transform.SetParent(spawnPoint.transform);
                c = go.GetComponentInParent<Character>();

                TurnManager.instance.AddCharacter(c);
                var contextCam = c.GetComponentInParent<ContextualIdleCamera>();
                if (contextCam != null) {
                    contextCam.InitializeEnemyCamera();
                }
                _characters.Add(c);
                _enemyIdxDict.Add(c, spawnIdx);
                _idxEnemyDict.Add(spawnIdx, c);
                OnCharacterSpawn?.Invoke(c, spawnIdx);
            }
            go.transform.position = spawnPoint.transform.position;
            return c;
        }

        public int GetMaxAllyCount() {
            return allySpawnPoints.Count;
        }

        public int GetMaxEnemyCount() {
            return enemySpawnPoints.Count;
        }

        public void RemoveCharacter(Character c) {
            if (c.Data.Team == CharacterTeam.Player) {
                int idx = GetAllyIndex(c);
                _allyIdxDict.Remove(c);
                _idxAllyDict.Remove(idx);
            }
            else {
                int idx = GetEnemyIndex(c);
                _enemyIdxDict.Remove(c);
                _idxEnemyDict.Remove(idx);
            }
            _characters.Remove(c);
            c.transform.SetParent(null);
            c.gameObject.SetActive(false);
        }

        public List<Character> GetCharacters() {
            List<Character> list = new List<Character>();
            foreach (Character c in _characters) {
                if (!c.IsDead) {
                    list.Add(c);
                }
            }
            return list;
        }

        public List<Character> GetAllCharacters() {
            return _characters;
        }

        public List<Character> GetAllyCharacters() {
            List<Character> list = new List<Character>();
            foreach (Character c in _characters) {
                if (!c.IsDead && c.Data.Team == CharacterTeam.Player) {
                    list.Add(c);
                }
            }
            return list;
        }

        public List<Character> GetAllAllyCharacters() {
            List<Character> list = new List<Character>();
            foreach (Character c in _characters) {
                if (c.Data.Team == CharacterTeam.Player) {
                    list.Add(c);
                }
            }
            return list;
        }

        public List<Character> GetEnemyCharacters() {
            List<Character> list = new List<Character>();
            foreach (Character c in _characters) {
                if (!c.IsDead && c.Data.Team == CharacterTeam.Enemy) {
                    list.Add(c);
                }
            }
            return list;
        }

        public List<Character> GetAllEnemyCharacters() {
            List<Character> list = new List<Character>();
            foreach (Character c in _characters) {
                if (c.Data.Team == CharacterTeam.Enemy) {
                    list.Add(c);
                }
            }
            return list;
        }

        /// <summary>
        /// 아군 캐릭터의 칸 확인
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int GetAllyIndex(Character c) {
            return _allyIdxDict.TryGetValue(c, out var idx) ? idx : -1;
        }

        /// <summary>
        /// 아군 캐릭터 칸에 있는 캐릭터 확인
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Character GetAllyAtIndex(int idx) {
            return _idxAllyDict.TryGetValue(idx, out var character) ? character : null;
        }

        /// <summary>
        /// 적 캐릭터의 칸 확인
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int GetEnemyIndex(Character c) {
            return _enemyIdxDict.TryGetValue(c, out var idx) ? idx : -1;
        }

        /// <summary>
        /// 적 캐릭터 칸에 있는 캐릭터 확인
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Character GetEnemyAtIndex(int idx) {
            return _idxEnemyDict.TryGetValue(idx, out var character) ? character : null;
        }
    }
}
