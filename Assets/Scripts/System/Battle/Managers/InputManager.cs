using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnBased.Battle.Managers {
    public class InputManager : MonoBehaviour {
        public static InputManager instance;
        private void Awake() {
            if (instance != null) {
                Destroy(this);
                return;
            }
            instance = this;
        }

        /// <summary>
        /// Å¸°Ù ÀüÈ¯ - ÁÂ
        /// </summary>
        private void OnLeft(InputValue inputValue) {
            TargetManager.instance.SelectLeftTarget();
        }

        /// <summary>
        /// Å¸°Ù ÀüÈ¯ - ¿ì
        /// </summary>
        private void OnRight(InputValue inputValue) {
            TargetManager.instance.SelectRightTarget();
        }

        /// <summary>
        /// ÀÏ¹Ý °ø°Ý
        /// </summary>
        /// <param name="inputValue"></param>
        private void OnAttack(InputValue inputValue) {
            var currentCharacter = TurnManager.instance.CurrentCharacter;
            if (currentCharacter.WantCmd && currentCharacter.Data.team == Data.CharacterTeam.Player) {
                if (currentCharacter.CurrentState == Character.CharacterState.PrepareAttack) {
                    currentCharacter.DoAttack();
                }
                else {
                    currentCharacter.PrepareAttack();
                }
            }
        }
    }
}
