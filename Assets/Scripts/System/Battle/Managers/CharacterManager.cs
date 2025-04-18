using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Battle.Managers {
    /// <summary>
    /// 등록된 캐릭터 관리
    /// </summary>
    public class CharacterManager : MonoBehaviour {
        public static CharacterManager instance;

        public List<GameObject> allySpawnPoints = new List<GameObject>();
        public List<GameObject> enemySpawnPoints = new List<GameObject>();

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
            // 임시 코드
            for (int i = 0; i < allySpawnPoints.Count; ++i) {
                var c = allySpawnPoints[i].GetComponentInChildren<Character>();
                if (c != null) {
                    TurnManager.instance.AddCharacter(c);
                    var contextCam = c.GetComponentInParent<ContextualIdleCamera>();
                    if (contextCam != null) {
                        contextCam.InitializeCamera();
                    }
                    _characters.Add(c);
                    _allyIdxDict.Add(c, i);
                    _idxAllyDict.Add(i, c);
                }
            }
            for (int i = 0; i < enemySpawnPoints.Count; ++i) {
                var c = enemySpawnPoints[i].GetComponentInChildren<Character>();
                if (c != null) {
                    TurnManager.instance.AddCharacter(c);
                    var contextCam = c.GetComponentInParent<ContextualIdleCamera>();
                    if (contextCam != null) {
                        contextCam.InitializeEnemyCamera();
                    }
                    _characters.Add(c);
                    _enemyIdxDict.Add(c, i);
                    _idxEnemyDict.Add(i, c);
                }
            }
            TurnManager.instance.InitializeTurnQueue();
            TargetManager.instance.InitializeTarget();
        }

        public void SpawnCharacter(GameObject prefab, int spawnPoint) {

        }

        public void AddCharacter(Character c, int idx) {

        }

        public int GetMaxAllyCount() {
            return allySpawnPoints.Count;
        }

        public int GetMaxEnemyCount() {
            return enemySpawnPoints.Count;
        }

        public void RemoveCharacter(Character c) {
            if (c.Data.team == Data.CharacterTeam.Player) {
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
                if (!c.IsDead && c.Data.team == Data.CharacterTeam.Player) {
                    list.Add(c);
                }
            }
            return list;
        }

        public List<Character> GetAllAllyCharacters() {
            List<Character> list = new List<Character>();
            foreach (Character c in _characters) {
                if (c.Data.team == Data.CharacterTeam.Player) {
                    list.Add(c);
                }
            }
            return list;
        }

        public List<Character> GetEnemyCharacters() {
            List<Character> list = new List<Character>();
            foreach (Character c in _characters) {
                if (!c.IsDead && c.Data.team == Data.CharacterTeam.Enemy) {
                    list.Add(c);
                }
            }
            return list;
        }

        public List<Character> GetAllEnemyCharacters() {
            List<Character> list = new List<Character>();
            foreach (Character c in _characters) {
                if (c.Data.team == Data.CharacterTeam.Enemy) {
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
            return _allyIdxDict[c];
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
            return _enemyIdxDict[c];
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
