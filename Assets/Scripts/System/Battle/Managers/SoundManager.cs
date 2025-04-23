using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Battle.Managers {
    public class SoundManager : MonoBehaviour {
        public static SoundManager instance;
        public const float MusicVolumeDefault = 0.7f;

        [System.Serializable]
        public class SoundData {
            public string name;
            public float volume = 1.0f;
            public List<AudioClip> audioClips;
            public AudioClip GetRandomClip() {
                if (audioClips.Count == 1) {
                    return audioClips[0];
                }
                return audioClips[Random.Range(0, audioClips.Count)];
            }
        }
        public List<SoundData> soundData = new List<SoundData>();
        [SerializeField]
        private AudioSource _2dAudioSource;
        [SerializeField]
        private AudioSource _musicAudioSource;

        private Dictionary<string, SoundData> _soundDict = new Dictionary<string, SoundData>();

        private Coroutine _musicVolumeCoroutine;
        private float _musicVolumeTarget;
        private float _musicVolumePrev;
        private float _musicVolumeWeight;


        private void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }
            instance = this;

            foreach (var sound in soundData) {
                if (!_soundDict.ContainsKey(sound.name)) {
                    _soundDict.Add(sound.name, sound);
                }
            }

            _musicAudioSource.volume = MusicVolumeDefault;
        }

        private IEnumerator InterpolateMusicVolume() {
            while (_musicAudioSource.volume != _musicVolumeTarget) {
                _musicAudioSource.volume = Mathf.Lerp(_musicVolumePrev, _musicVolumeTarget, _musicVolumeWeight);
                _musicVolumeWeight = Mathf.Min(_musicVolumeWeight + Time.unscaledDeltaTime * 2f, 1f);
                yield return null;
            }
            _musicVolumeCoroutine = null;
        }

        public SoundData GetSoundData(string name) {
            if (_soundDict.ContainsKey(name)) {
                return _soundDict[name];
            }
            return null;
        }

        public void Play2DSound(AudioClip clip, float volume = 1.0f) {
            _2dAudioSource.PlayOneShot(clip, volume);
        }

        public void Play2DSound(string soundName) {
            var soundData = GetSoundData(soundName);
            if (soundData != null) {
                _2dAudioSource.PlayOneShot(soundData.GetRandomClip(), soundData.volume);
            }
        }

        public void PlayMusic(AudioClip clip) {
            _musicAudioSource.Stop();
            _musicAudioSource.clip = clip;
            _musicAudioSource.Play();
        }

        public void ChangeMusicVolume(float target) {
            _musicVolumePrev = _musicAudioSource.volume;
            _musicVolumeTarget = target;
            _musicVolumeWeight = 0f;
            if (_musicVolumeCoroutine == null) {
                _musicVolumeCoroutine = StartCoroutine(InterpolateMusicVolume());
            }
        }
    }
}