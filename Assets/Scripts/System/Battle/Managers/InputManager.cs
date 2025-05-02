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
            return currentCharacter.WantCmd && currentCharacter.Data.Team == Data.CharacterTeam.Player;
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
            if (CombatManager.instance.SkillPoint > 0) {
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
            //TODO: Send notification on no sp
        }

        private void TryCastUlt(Character character) {
            if (character != null &&
                character.CurrentState != Character.CharacterState.PrepareUltAttack &&
                character.CurrentState != Character.CharacterState.PrepareUltSkill) {
                if (CombatManager.CanCharacterUseUlt(character)) {
                    TurnManager.instance.AddUltTurn(character);
                }
                else {
                    SoundManager.instance.Play2DSound("UIUltNotReady");
                }
            }
        }

        private void OnUlt1(InputValue inputValue) {
            TryCastUlt(CharacterManager.instance.GetAllyCharacters()[0]);
        }

        private void OnUlt2(InputValue inputValue) {
            TryCastUlt(CharacterManager.instance.GetAllyCharacters()[1]);
        }

        private void OnUlt3(InputValue inputValue) {
            TryCastUlt(CharacterManager.instance.GetAllyCharacters()[2]);
        }
    }
}
