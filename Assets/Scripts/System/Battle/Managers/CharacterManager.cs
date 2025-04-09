using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Battle.Managers {
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
                    var camCon = c.GetComponentInParent<IdleCamController>();
                    if (camCon != null) {
                        camCon.InitializeController();
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
                    var camCon = c.GetComponentInParent<IdleCamController>();
                    if (camCon != null) {
                        camCon.InitializeController();
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

        public List<Character> GetCharacters() {
            return _characters;
        }

        public List<Character> GetAllyCharacters() {
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
                if (c.Data.team == Data.CharacterTeam.Enemy) {
                    list.Add(c);
                }
            }
            return list;
        }

        public int GetAllyIndex(Character c) {
            return _allyIdxDict[c];
        }

        public Character GetAllyAtIndex(int idx) {
            return _idxAllyDict.TryGetValue(idx, out var character) ? character : null;
        }

        public int GetEnemyIndex(Character c) {
            return _enemyIdxDict[c];
        }

        public Character GetEnemyAtIndex(int idx) {
            return _idxEnemyDict.TryGetValue(idx, out var character) ? character : null;
        }
    }
}
