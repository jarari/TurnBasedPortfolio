using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using UnityEngine.UI;

public class TargetUITest : MonoBehaviour {
    public Image target;

    private Character character;

    private void Start() {
        TargetManager.instance.OnTargetChanged += OnTargetChanged;
    }

    private void Update() {
        target.transform.position = Camera.main.WorldToScreenPoint(character.transform.position + Vector3.up * 1.0f);
    }

    private void OnTargetChanged(Character c){
        character = c;
        target.transform.position = Camera.main.WorldToScreenPoint(c.transform.position + Vector3.up * 1.0f);
    }
}
