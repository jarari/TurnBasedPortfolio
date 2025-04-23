using TurnBased.Battle;
using UnityEngine;

namespace TurnBased.Entities.Battle {
    public class AnimationEventDelegate : MonoBehaviour {
        private Character character;

        private void Awake() {
            character = GetComponentInParent<Character>();
        }

        public void SendAnimationEvent(string animEvent) {
            character?.ProcessAnimationEvent(animEvent);
        }
    }
}
