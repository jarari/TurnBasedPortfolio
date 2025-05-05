using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Data {
    [System.Serializable]
    public class Wave {
        public List<string> enemies;
    }
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StageData")]
    public class StageData : ScriptableObject {
        public GameObject stagePrefab;
        public AudioClip stageBGM;
        public List<Wave> waves;
    }
}