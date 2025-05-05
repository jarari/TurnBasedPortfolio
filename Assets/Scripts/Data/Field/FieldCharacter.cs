using TurnBased.Data;
using UnityEngine;

namespace TurnBased.Field {
    public class FieldCharacter : MonoBehaviour {
        [SerializeField]
        private CharacterData _baseData;

        public CharacterData BaseData {
            get {
                return _baseData;
            }
        }

        public CharacterDataInstance Data { get; private set; }
        protected void Awake() {
            Data = new CharacterDataInstance(_baseData);
        }
    }

}