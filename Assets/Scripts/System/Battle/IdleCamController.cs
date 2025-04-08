using System.Collections;
using TurnBased.Battle.Managers;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBased.Battle {
    public class IdleCamController : MonoBehaviour {
        public GameObject cmIdleCam;
        public GameObject cmTurnChangeCam;
        public Transform dollySwing;

        public Character tempCharacter;

        private Character _character;
        private CinemachineSplineDolly _idleSplineDolly;
        private CinemachineCamera _idleCam;
        private CinemachineCamera _turnChangeCam;

        private Coroutine _idleAnimationCoroutine;
        private float _swingSpeed = 0.75f;
        private float _swingAmount = 0.15f;

        private IEnumerator AnimateIdleCam() {
            yield return null;
            _turnChangeCam.Priority = 0;
            while (true) {
                dollySwing.localPosition = new Vector3(0, 0, Mathf.Cos(Time.time * _swingSpeed) * _swingAmount);
                yield return null;
            }
        }

        private void OnCharacterTurnStart(Character c) {
            _idleCam.Priority = 1;
            _turnChangeCam.Priority = 2;
            if (_idleAnimationCoroutine == null) {
                _idleAnimationCoroutine = StartCoroutine(AnimateIdleCam());
            }
        }

        private void OnCharacterTurnEnd(Character c) {
            _idleCam.Priority = 0;
            _turnChangeCam.Priority = 0;
            StopCoroutine(_idleAnimationCoroutine);
            _idleAnimationCoroutine = null;
        }

        private void OnCamTargetUpdate(float pos) {
            _idleSplineDolly.CameraPosition = pos;
        }

        public void InitializeController() {
            _character = GetComponentInChildren<Character>();
            _character.OnTurnStart += OnCharacterTurnStart;
            _character.OnTurnEnd += OnCharacterTurnEnd;
            _idleSplineDolly = cmIdleCam.GetComponent<CinemachineSplineDolly>();
            _idleCam = cmIdleCam.GetComponent<CinemachineCamera>();
            _turnChangeCam = cmTurnChangeCam.GetComponent<CinemachineCamera>();
            TargetManager.instance.OnCamTargetUpdate += OnCamTargetUpdate;
        }
    }
}
