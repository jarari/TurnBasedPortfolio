using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;
using UnityEngine.UI;

public class TargetUITest : MonoBehaviour {
    public Image target;

    private Character character;

    private void Start() {
        TargetManager.instance.OnTargetChanged += OnTargetChanged;
        character = TargetManager.instance.Target;
    }

    private void Update() {
        target.transform.position = Camera.main.WorldToScreenPoint(character.meshParent.transform.position + Vector3.up * 1.0f);
    }

    private void OnTargetChanged(Character c){
        character = c;
        target.transform.position = Camera.main.WorldToScreenPoint(c.meshParent.transform.position + Vector3.up * 1.0f);
    }
}
