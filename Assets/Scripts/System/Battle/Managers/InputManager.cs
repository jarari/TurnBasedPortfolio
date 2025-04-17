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

        private bool CanCharacterTakeInput() {
            var currentCharacter = TurnManager.instance.CurrentCharacter;
            return currentCharacter.WantCmd && currentCharacter.Data.team == Data.CharacterTeam.Player;
        }

        /// <summary>
        /// 타겟 전환 - 좌
        /// </summary>
        private void OnLeft(InputValue inputValue) {
            if (CanCharacterTakeInput()) {
                TargetManager.instance.SelectLeftTarget();
            }
        }

        /// <summary>
        /// 타겟 전환 - 우
        /// </summary>
        private void OnRight(InputValue inputValue) {
            if (CanCharacterTakeInput()) {
                TargetManager.instance.SelectRightTarget();
            }
        }

        /// <summary>
        /// 일반 공격
        /// </summary>
        /// <param name="inputValue"></param>
        private void OnAttack(InputValue inputValue) {
            var currentCharacter = TurnManager.instance.CurrentCharacter;
            if (CanCharacterTakeInput()) {
                if (currentCharacter.CurrentState != Character.CharacterState.PrepareUltAttack &&
                    currentCharacter.CurrentState != Character.CharacterState.PrepareUltSkill) {
                    if (currentCharacter.CurrentState == Character.CharacterState.PrepareAttack) {
                        currentCharacter.DoAttack();
                    }
                    else {
                        currentCharacter.PrepareAttack();
                    }
                }
                else {
                    if (currentCharacter.CurrentState == Character.CharacterState.PrepareUltAttack) {
                        currentCharacter.CastUltAttack();
                    }
                    else {
                        currentCharacter.PrepareUltAttack();
                    }
                }
            }
        }

        /// <summary>
        /// 스킬 공격
        /// </summary>
        /// <param name="inputValue"></param>
        private void OnSkill(InputValue inputValue) {
            var currentCharacter = TurnManager.instance.CurrentCharacter;
            if (CanCharacterTakeInput()) {
                if (currentCharacter.CurrentState != Character.CharacterState.PrepareUltAttack &&
                    currentCharacter.CurrentState != Character.CharacterState.PrepareUltSkill) {
                    if (currentCharacter.CurrentState == Character.CharacterState.PrepareSkill) {
                        currentCharacter.CastSkill();
                    }
                    else {
                        currentCharacter.PrepareSkill();
                    }
                }
                else {
                    if (currentCharacter.CurrentState == Character.CharacterState.PrepareUltSkill) {
                        currentCharacter.CastUltSkill();
                    }
                    else {
                        currentCharacter.PrepareUltSkill();
                    }
                }
            }
        }

        private void OnUlt1(InputValue inputValue) {
            var character = CharacterManager.instance.GetAllyCharacters()[0];
            if (character != null && 
                character.CurrentState != Character.CharacterState.PrepareUltAttack &&
                character.CurrentState != Character.CharacterState.PrepareUltSkill) {
                TurnManager.instance.AddUltTurn(character);
            }
        }

        private void OnUlt2(InputValue inputValue) {
            var character = CharacterManager.instance.GetAllyCharacters()[1];
            if (character != null &&
                character.CurrentState != Character.CharacterState.PrepareUltAttack &&
                character.CurrentState != Character.CharacterState.PrepareUltSkill) {
                TurnManager.instance.AddUltTurn(character);
            }
        }

        private void OnUlt3(InputValue inputValue) {
            var character = CharacterManager.instance.GetAllyCharacters()[2];
            if (character != null &&
                character.CurrentState != Character.CharacterState.PrepareUltAttack &&
                character.CurrentState != Character.CharacterState.PrepareUltSkill) {
                TurnManager.instance.AddUltTurn(character);
            }
        }
    }
}
