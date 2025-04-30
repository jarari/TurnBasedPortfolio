using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Data {
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

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundPack", order = 1)]
    public class SoundPack : ScriptableObject {
        public List<SoundData> soundData;
    }
}
