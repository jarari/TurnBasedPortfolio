using System.Collections;
using System.Collections.Generic;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBased.Battle {
    /// <summary>
    /// ���忡 ���������� ��ġ�ϴ� ���޻��� ī�޶� ��Ʈ�ѷ�
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
                // ī�޶� ��ȯ�ñ��� ĳ���͸� ����
                HideAllyCharacters();
                // ī�޶� ��ȯ ��ٸ���
                yield return new WaitWhile(() => !CinemachineCore.IsLive(_turnChangeCam));
                CheckVisibleCharacters();
            }
            // �� ��ȯ ī�޶� -> ���޻��� ī�޶� ��ȯ
            _turnChangeCam.Priority = 0;
            while (_characterTurn) {
                // �¿�� �����̴� ī�޶� ��� �߰�
                dollySwing.localPosition = new Vector3(0, 0, Mathf.Cos(Time.time * _swingSpeed) * _swingAmount);
                yield return null;
            }
        }

        private void OnCharacterTurnStart(Character c) {
            //�� ���� �� ���޻��� ī�޶�� �� ��ȯ ī�޶� Ȱ��ȭ
            _idleCam.Priority = 1;
            _turnChangeCam.Priority = 2;
            _characterTurn = true;
            if (_idleAnimationCoroutine == null) {
                _idleAnimationCoroutine = StartCoroutine(AnimateIdleCam());
            }
        }

        private void OnCharacterTurnEnd(Character c) {
            // �� ���� �� ī�޶� �� �ڷ�ƾ ����
            _idleCam.Priority = 0;
            _turnChangeCam.Priority = 0;
            cmSelfCam.Priority = 0;
            cmAllCam.Priority = 0;
            _characterTurn = false;
            _idleAnimationCoroutine = null;
        }

        private void OnCamTargetUpdate(float pos) {
            // Ÿ�� �Ŵ����� ī�޶� ���� ��ġ ����ȭ
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
