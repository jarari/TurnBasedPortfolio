using UnityEngine;
using TurnBased.Data;
using System;
using TurnBased.Battle.Managers;

namespace TurnBased.Battle {
    public abstract class Character : MonoBehaviour {
        public GameObject meshParent;

        [Header("Character Data")]
        [SerializeField]
        protected CharacterData _baseData;

        public enum CharacterState {
            Idle,
            PrepareAttack,
            DoAttack,
            DoExtraAttack,
            PrepareSkill,
            CastSkill,
            PrepareUlt,
            CastUlt
        }

        public Action<Character> OnTurnStart;
        public Action<Character> OnTurnEnd;
        public Action<Character, string, string> OnAnimationEvent;
        public Action<Character, bool> OnVisibilityChange;

        public CharacterData Data { get; private set; }

        public CharacterState CurrentState { get; protected set; }
        public bool WantCmd { get; set; }
        public CharacterState WantState { get; protected set; } = CharacterState.PrepareAttack;
        public bool IsVisible { get; set; }


        protected virtual void Awake() {
            Data = Instantiate(_baseData);
            Data.stats.CurrentToughness = Data.stats.MaxToughness;
            Data.stats.CurrentHP = Data.stats.MaxHP;
        }

        protected virtual void Start() {
            SetVisible(true);
        }

        public virtual void TakeTurn() {
            WantCmd = true;
            if (Data.team == CharacterTeam.Player) {
                if (WantState == CharacterState.PrepareAttack) {
                    PrepareAttack();
                }
                else if (WantState == CharacterState.PrepareSkill) {
                    PrepareSkill();
                }
            }
            OnTurnStart?.Invoke(this);
        }

        public virtual void EndTurn() {
            CurrentState = CharacterState.Idle;
            TurnManager.instance.EndTurn();
            OnTurnEnd?.Invoke(this);
        }

        public virtual void ProcessAnimationEvent(string animEvent) {
            string[] args = animEvent.Split(".");
            string argument = args[0];
            string payload = "";
            if (args.Length >= 2) {
                payload = args[1];
            }
            OnAnimationEvent?.Invoke(this, argument, payload);
        }

        public virtual void PrepareAttack() {
            CurrentState = CharacterState.PrepareAttack;
        }
        public virtual void DoAttack() {
            CurrentState = CharacterState.DoAttack;
            WantState = CharacterState.PrepareAttack;
            WantCmd = false;
        }
        public virtual void DoExtraAttack() {
            CurrentState = CharacterState.DoExtraAttack;
        }
        public virtual void PrepareSkill() {
            CurrentState = CharacterState.PrepareSkill;
        }
        public virtual void CastSkill() {
            CurrentState = CharacterState.CastSkill;
            WantState = CharacterState.PrepareSkill;
            WantCmd = false;
        }
        public virtual void PrepareUlt() {
            CurrentState = CharacterState.PrepareUlt;
        }
        public virtual void CastUlt() {
            CurrentState = CharacterState.CastUlt;
            WantCmd = false;
        }

        public virtual void SetVisible(bool visibility) {
            meshParent?.SetActive(visibility);
            IsVisible = visibility;
            OnVisibilityChange?.Invoke(this, visibility);
        }
    }
}
