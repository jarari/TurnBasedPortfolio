using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Battle.Managers {
    public class TargetManager : MonoBehaviour {
        public static TargetManager instance;

        public GameObject camTarget;
        public List<Transform> camPos;

        public Action<Character> OnTargetChanged;
        public Action<float> OnCamTargetUpdate;

        public Character Target { get; private set; }
        public int TargetIndex { get; private set; }

        public float InterpolatedTargetPos { get; private set; }

        private const float _targetTrackTime = 0.1f;
        private float _targetTrackWeight = 0f;
        private float _prevTarget = 2f;
        private float _currentTarget = 2f;
        private Coroutine _targetCoroutine;

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
                if (posInt < camPos.Count - 1) {
                    camTarget.transform.position = Vector3.Lerp(camPos[posInt].position, camPos[posInt + 1].position, posFloat);
                }
                else {
                    camTarget.transform.position = camPos[camPos.Count - 1].position;
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
            if (_targetCoroutine == null) {
                _targetCoroutine = StartCoroutine(TrackTarget());
            }
        }

        private void UpdateTarget(Character c, int idx) {
            Target = c;
            TargetIndex = idx;
            StartTargetPosUpdate();
            OnTargetChanged?.Invoke(Target);
        }

        public void InitializeTarget() {
            int[] idxToTry = { 2, 1, 3, 0, 4 };
            Character enemy = null;
            int tryIdx = 0;
            do {
                enemy = CharacterManager.instance.GetEnemyAtIndex(idxToTry[tryIdx]);
                if (enemy == null) {
                    tryIdx++;
                }
            } while (enemy == null);
            UpdateTarget(enemy, idxToTry[tryIdx]);
        }

        public bool SelectLeftTarget() {
            int idxToTry = TargetIndex - 1;
            Character enemy = null;
            while (enemy == null && idxToTry >= 0) {
                enemy = CharacterManager.instance.GetEnemyAtIndex(idxToTry);
                if (enemy == null) {
                    idxToTry--;
                }
            }
            if (enemy != null) {
                UpdateTarget(enemy, idxToTry);
                return true;
            }
            return false;
        }

        public bool SelectRightTarget() {
            int idxToTry = TargetIndex + 1;
            Character enemy = null;
            while (enemy == null && idxToTry <= 4) {
                enemy = CharacterManager.instance.GetEnemyAtIndex(idxToTry);
                if (enemy == null) {
                    idxToTry++;
                }
            }
            if (enemy != null) {
                UpdateTarget(enemy, idxToTry);
                return true;
            }
            return false;
        }
    }
}
