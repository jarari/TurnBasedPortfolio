using System.Collections;
using TurnBased.Data;
using UnityEngine;
using UnityEngine.Playables;

namespace TurnBased.Battle.Managers {
    [DefaultExecutionOrder(200)]
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
                #region -by 정준

                _stageData = EncounterManager.Instance.stagedata;

                #endregion
            }

            CombatManager.instance.OnCharacterDeath += HandleCharacterDeath;
            CombatManager.instance.OnCharacterDeathComplete += HandleCharacterDeathComplete;

            InitializeStage();
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
                Debug.Log("전투 실패");                
                StartCoroutine(FinishBattle(false));       // 실패로 종료   =====
            }
            else if (_aliveEnemyCount == 0) {
                if (_waveNum < _stageData.waves.Count) {
                    _enemySpawnTimeline.Play();
                }
                else {
                    Debug.Log("전투 승리");                    
                    StartCoroutine(FinishBattle(true));     // 승리로 종료   =====
                }
            }
        }
        #region - by 준
        /// <summary>
        /// 2초후 씬전환을 시킬 코루틴함수
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        IEnumerator FinishBattle(bool a)
        {
            // 2초 기다린다
            yield return new WaitForSeconds(2.0f);

            EncounterManager.Instance.FinishEncounter(a);
        }
        #endregion

        private void CreateStage() {
            var go = Instantiate(_stageData.stagePrefab);
            go.transform.SetParent(_worldRoot);

            SoundManager.instance.PlayMusic(_stageData.stageBGM);
        }
        #region -by 정준

        private void SpawnAllyCharacters()
        {
            Debug.Log("SpawnAllyCharacters 진입");

            foreach (var id in EncounterManager.Instance.PlayerTeamIds)
            {
                var character = CharacterManager.instance.SpawnCharacter(id);

                if (character != null)
                {
                    //CharacterManager.instance.SpawnCharacter(id);
                    _aliveAllyCount++;
                    Debug.Log(character.gameObject.name + " + 생성");
                }
                else
                {
                    Debug.Log("캐릭터 생성 실패" + id);
                }


            }
        
        }

        #endregion

        /*
        private void SpawnAllyCharacters() {
            CharacterManager.instance.SpawnCharacter("Ally_Colphne");
            _aliveAllyCount++;
            CharacterManager.instance.SpawnCharacter("Ally_Vanguard");
            _aliveAllyCount++;
            CharacterManager.instance.SpawnCharacter("Ally_MarkerMan");
            _aliveAllyCount++;
        }
        */


        public void InitializeStage() {
            CreateStage();
            SpawnAllyCharacters();
            SpawnNextWave();
            StartTurn();
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