using TurnBased.Data;
using UnityEngine;
using UnityEngine.Playables;

namespace TurnBased.Battle.Managers {
    public class StageManager : MonoBehaviour {
        public static StageManager instance;

        [SerializeField]
        private StageData _testStageData;
        [SerializeField]
        private Transform _worldRoot;

        private int _waveNum;
        private int _aliveAllyCount;
        private int _aliveEnemyCount;
        private StageData _stageData;
        private PlayableDirector _enemySpawnTimeline;

        private void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }
            instance = this;
            _enemySpawnTimeline = GetComponent<PlayableDirector>();
        }

        private void Start() {
            if (_testStageData != null) {
                _stageData = _testStageData;
            }
            else {
                //TODO: Use stage data passed from field scene
            }

            CombatManager.instance.OnCharacterDeath += HandleCharacterDeath;
            CombatManager.instance.OnCharacterDeathComplete += HandleCharacterDeathComplete;

            CreateStage();
            SpawnAllyCharacters();
            SpawnNextWave();
            StartTurn();
        }

        private void HandleCharacterDeath(Character c, Character killer) {
            if (c.Data.Team == CharacterTeam.Player) {
                _aliveAllyCount--;
            }
            else if (c.Data.Team == CharacterTeam.Enemy) {
                _aliveEnemyCount--;
            }
            killer.WantCmd = false;
        }

        private void HandleCharacterDeathComplete(Character c) {
            if (_aliveAllyCount == 0) {
                //TODO: Stage fail
            }
            else if (_aliveEnemyCount == 0) {
                if (_waveNum < _stageData.waves.Count) {
                    _enemySpawnTimeline.Play();
                }
                else {
                    //TODO: Stage clear
                }
            }
        }

        private void CreateStage() {
            var go = Instantiate(_stageData.stagePrefab);
            go.transform.SetParent(_worldRoot);
        }

        private void SpawnAllyCharacters() {
            CharacterManager.instance.SpawnCharacter("Ally_Colphne");
            _aliveAllyCount++;
            CharacterManager.instance.SpawnCharacter("Ally_Vanguard");
            _aliveAllyCount++;
            CharacterManager.instance.SpawnCharacter("Ally_MarkerMan");
            _aliveAllyCount++;
        }

        public void SpawnNextWave() {
            foreach (var enemy in _stageData.waves[_waveNum].enemies) {
                CharacterManager.instance.SpawnCharacter(enemy);
                _aliveEnemyCount++;
            }
            _waveNum++;
        }

        public void StartTurn() {
            TurnManager.instance.InitializeTurnQueue();
            TargetManager.instance.InitializeTarget();
        }
    }
}