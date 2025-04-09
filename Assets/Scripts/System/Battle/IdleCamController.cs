using System.Collections;
using System.Collections.Generic;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBased.Battle {
    /// <summary>
    /// 월드에 고정적으로 배치하는 유휴상태 카메라 컨트롤러
    /// </summary>
    public class IdleCamController : MonoBehaviour {
        public GameObject cmIdleCam;
        public GameObject cmTurnChangeCam;
        public CinemachineCamera cmSelfCam;
        public CinemachineCamera cmAllCam;
        public Transform dollySwing;
        public List<IdleCamController> hideOnTurn;

        public bool Initialized { get; private set; }

        private Character _character;
        private CinemachineSplineDolly _idleSplineDolly;
        private CinemachineCamera _idleCam;
        private CinemachineCamera _turnChangeCam;

        private Coroutine _idleAnimationCoroutine;
        private float _swingSpeed = 0.75f;
        private float _swingAmount = 0.15f;

        private bool _characterTurn;

        private void HideAllyCharacters() {
            foreach (var c in CharacterManager.instance.GetAllyCharacters()) {
                c.SetVisible(false);
            }
        }

        private void ShowAllyCharacters() {
            foreach (var c in CharacterManager.instance.GetAllyCharacters()) {
                c.SetVisible(true);
            }
        }

        private void CheckVisibleCharacters() {
            ShowAllyCharacters();
            foreach (var con in hideOnTurn) {
                if (con.Initialized) {
                    con.SetCharacterVisible(false);
                }
            }
        }

        private IEnumerator AnimateIdleCam() {
            if (TargetManager.instance.TargetTeam == CharacterTeam.Enemy) {
                // 카메라 전환시까지 캐릭터를 숨김
                HideAllyCharacters();
                // 카메라 전환 기다리기
                yield return new WaitWhile(() => !CinemachineCore.IsLive(_turnChangeCam));
                CheckVisibleCharacters();
            }
            // 턴 전환 카메라 -> 유휴상태 카메라 전환
            _turnChangeCam.Priority = 0;
            while (_characterTurn) {
                // 좌우로 움직이는 카메라 모션 추가
                dollySwing.localPosition = new Vector3(0, 0, Mathf.Cos(Time.time * _swingSpeed) * _swingAmount);
                yield return null;
            }
        }

        private void OnCharacterTurnStart(Character c) {
            //턴 시작 시 유휴상태 카메라와 턴 전환 카메라 활성화
            _idleCam.Priority = 1;
            _turnChangeCam.Priority = 2;
            _characterTurn = true;
            if (_idleAnimationCoroutine == null) {
                _idleAnimationCoroutine = StartCoroutine(AnimateIdleCam());
            }
        }

        private void OnCharacterTurnEnd(Character c) {
            // 턴 종료 시 카메라 및 코루틴 정리
            _idleCam.Priority = 0;
            _turnChangeCam.Priority = 0;
            cmSelfCam.Priority = 0;
            cmAllCam.Priority = 0;
            _characterTurn = false;
            _idleAnimationCoroutine = null;
        }

        private void OnCamTargetUpdate(float pos) {
            // 타겟 매니저와 카메라 돌리 위치 동기화
            if (TargetManager.instance.TargetTeam == CharacterTeam.Enemy) {
                _idleSplineDolly.CameraPosition = pos;
            }
        }

        private void OnTargetSettingChanged() {
            if (TurnManager.instance.CurrentCharacter == _character) {
                var tm = TargetManager.instance;
                if (tm.Mode == TargetManager.TargetMode.Self) {
                    cmSelfCam.Priority = 3;
                    cmAllCam.Priority = 0;
                    ShowAllyCharacters();
                }
                else if (tm.TargetTeam == CharacterTeam.Player) {
                    cmSelfCam.Priority = 0;
                    cmAllCam.Priority = 3;
                    ShowAllyCharacters();
                }
                else {
                    cmSelfCam.Priority = 0;
                    cmAllCam.Priority = 0;
                    CheckVisibleCharacters();
                }
            }
        }

        public void InitializeController() {
            _character = GetComponentInChildren<Character>();
            _character.OnTurnStart += OnCharacterTurnStart;
            _character.OnTurnEnd += OnCharacterTurnEnd;
            _idleSplineDolly = cmIdleCam.GetComponent<CinemachineSplineDolly>();
            _idleCam = cmIdleCam.GetComponent<CinemachineCamera>();
            _turnChangeCam = cmTurnChangeCam.GetComponent<CinemachineCamera>();
            TargetManager.instance.OnCamTargetUpdate += OnCamTargetUpdate;
            TargetManager.instance.OnTargetSettingChanged += OnTargetSettingChanged;
            Initialized = true;
        }

        public void SetCharacterVisible(bool visibility) {
            _character?.SetVisible(visibility);
        }
    }
}
