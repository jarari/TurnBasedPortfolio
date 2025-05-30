using System.Collections.Generic;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBased.Battle {
    public class ContextualIdleCamera : CinemachineCameraManagerBase {
        public List<ContextualIdleCamera> hideOnTurn;

        private CinemachineVirtualCameraBase cmIdleCam;
        private CinemachineVirtualCameraBase cmTurnChangeCam;
        private CinemachineVirtualCameraBase cmSelfCam;
        private CinemachineVirtualCameraBase cmAllCam;
        private CinemachineVirtualCameraBase cmUltIdleCam;

        public bool Initialized { get; private set; }


        private Character _character;
        private CinemachineSplineDolly _idleSplineDolly;
        private CinemachineSplineDolly _ultIdleSplineDolly;

        private enum Context {
            WaitingTurn,
            TurnStart,
            TargetEnemy,
            TargetAlly,
            TargetSelf,
            Ult
        }
        private Context _currentContext;
        private Context _delayedContext;

        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime) {
            if (Initialized && _character.Data.Team == CharacterTeam.Player) {
                switch (_currentContext) {
                    case Context.WaitingTurn:
                    default:
                        return (CinemachineVirtualCameraBase)LiveChild;
                    case Context.TurnStart:
                        _currentContext = _delayedContext;
                        CheckVisibleCharacters();
                        return cmTurnChangeCam;
                    case Context.TargetEnemy:
                        return cmIdleCam;
                    case Context.TargetAlly:
                        return cmAllCam;
                    case Context.TargetSelf:
                        return cmSelfCam;
                    case Context.Ult:
                        return cmUltIdleCam;
                }
            }
            return cmIdleCam;
        }

        private void Awake() {
            for (int i = 0; i < ChildCameras.Count; ++i) {
                var cam = ChildCameras[i];
                if (cam.Name == "CM Cam Idle") {
                    cmIdleCam = cam;
                    _idleSplineDolly = cmIdleCam.GetComponent<CinemachineSplineDolly>();
                }
                else if (cam.Name == "CM Cam Turn Changed") {
                    cmTurnChangeCam = cam;
                }
                else if (cam.Name == "CM Cam Self") {
                    cmSelfCam = cam;
                }
                else if (cam.Name == "CM Cam All") {
                    cmAllCam = cam;
                }
                else if (cam.Name == "CM Cam Ult Idle") {
                    cmUltIdleCam = cam;
                    _ultIdleSplineDolly = cmUltIdleCam.GetComponent<CinemachineSplineDolly>();
                }
            }
        }


        private void HideAllyCharacters() {
            foreach (var c in CharacterManager.instance.GetAllyCharacters()) {
                c.SetVisible(false);
            }
        }

        private void ShowAllyCharacters() {
            foreach (var c in CharacterManager.instance.GetAllAllyCharacters()) {
                c.SetVisible(true);
            }
        }

        private void ResetAllCharacterLayers() {
            foreach (var c in CharacterManager.instance.GetCharacters()) {
                c.SetMeshLayer(Character.MeshLayer.Default);
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

        private void OnCharacterTurnEnd(Character c) {
            _currentContext = Context.WaitingTurn;
        }

        private void ProcessTurnStartCommon() {
            Priority = 4;
            Prioritize();
            if (CinemachineCore.IsLive(this)) {
                _character.ProcessCamChanged();
                _character.ProcessCamGain();
            }
            ResetAllCharacterLayers();
        }

        private void OnCharacterTurnStart(Character c) {
            ProcessTurnStartCommon();
            if (_currentContext == Context.TargetEnemy) {
                _currentContext = Context.TurnStart;
                _delayedContext = Context.TargetEnemy;
            }
        }

        private void OnCharacterUltTurn(Character c) {
            ProcessTurnStartCommon();
            _currentContext = Context.Ult;
            _ultIdleSplineDolly.CameraPosition = 0;
        }

        private void OnCharacterExtraAttackTurn(Character c) {
            ProcessTurnStartCommon();
            _currentContext = Context.TurnStart;
        }

        private void OnCharacterTransitionTurn(Character c) {
            ProcessTurnStartCommon();
            _currentContext = Context.TargetSelf;
        }

        private void OnCamTargetUpdate(float pos) {
            // 타겟 매니저와 카메라 돌리 위치 동기화
            if (TargetManager.instance.TargetTeam == CharacterTeam.Enemy) {
                _idleSplineDolly.CameraPosition = pos;
            }
        }

        private void OnTargetSettingChanged() {
            if (TurnManager.instance.CurrentCharacter == _character
                && _character.CurrentState != Character.CharacterState.PrepareUltAttack
                && _character.CurrentState != Character.CharacterState.PrepareUltSkill) {
                var tm = TargetManager.instance;
                if (tm.Mode == TargetManager.TargetMode.Self) {
                    _currentContext = Context.TargetSelf;
                    ShowAllyCharacters();
                }
                else if (tm.TargetTeam == CharacterTeam.Player) {
                    _currentContext = Context.TargetAlly;
                    ShowAllyCharacters();
                }
                else {
                    _currentContext = Context.TargetEnemy;
                    CheckVisibleCharacters();
                }
            }
        }


        public void InitializeCamera() {
            _character = GetComponentInChildren<Character>();
            _character.OnTurnStart += OnCharacterTurnStart;
            _character.OnTurnEnd += OnCharacterTurnEnd;
            _character.OnUltTurn += OnCharacterUltTurn;
            _character.OnExtraAttackTurn += OnCharacterExtraAttackTurn;
            _character.OnTransitionTurn += OnCharacterTransitionTurn;
            if (_character.ultIdleOverride != null) {
                cmUltIdleCam = _character.ultIdleOverride;
                _ultIdleSplineDolly = cmUltIdleCam.GetComponent<CinemachineSplineDolly>();
            }
            TargetManager.instance.OnCamTargetUpdate += OnCamTargetUpdate;
            TargetManager.instance.OnTargetSettingChanged += OnTargetSettingChanged;
            Initialized = true;
        }

        public void InitializeEnemyCamera() {
            _character = GetComponentInChildren<Character>();
            _character.OnTurnStart += OnCharacterTurnStart;
            _character.OnTurnEnd += OnCharacterTurnEnd;
            _character.OnUltTurn += OnCharacterTurnStart;
            _character.OnExtraAttackTurn += OnCharacterTurnStart;
            _character.OnTransitionTurn += OnCharacterTransitionTurn;
            Initialized = true;
        }

        public void SetCharacterVisible(bool visibility) {
            _character?.SetVisible(visibility);
        }

        public void NotifyCamChanged() {
            _character.ProcessCamChanged();
        }


        public override void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime) {
            base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
            var contextCam = fromCam as ContextualIdleCamera;
            if (contextCam != null) {
                contextCam.Priority = 0;
                contextCam.NotifyCamChanged();
            }
            if (_currentContext == Context.Ult) {
                _ultIdleSplineDolly.CameraPosition = 0;
            }
            _character.ProcessCamGain();
        }
    }
}
