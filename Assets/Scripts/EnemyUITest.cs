using UnityEngine;
using TurnBased.Battle;
using TurnBased.Data;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TurnBased.Battle.Managers;

public class EnemyUITest : MonoBehaviour {
    [SerializeField]
    private GameObject uiRoot;
    [SerializeField]
    private Transform weaknessRoot;
    [SerializeField]
    private GameObject weaknessPrefab;
    [SerializeField]
    private List<Sprite> weaknessSprites;
    [SerializeField]
    private float weaknessPadding = 0.15f;
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private Slider toughnessSlider;

    private Character _character;

    private void Awake() {
        _character = GetComponentInParent<Character>();
    }

    private void Start() {
        UpdateWeakness();
        HP_OnValueChanged(_character.Data.HP.Current);
        Toughness_OnValueChanged(_character.Data.Toughness.Current);
        _character.Data.HP.OnValueChanged += HP_OnValueChanged;
        _character.Data.Toughness.OnValueChanged += Toughness_OnValueChanged;
        _character.OnCharacterStateChanged += HandleCharacterStateChanged;
    }

    private void HandleCharacterStateChanged(Character c, Character.CharacterState state) {
        if (state == Character.CharacterState.DoAttack ||
            state == Character.CharacterState.CastSkill ||
            state == Character.CharacterState.CastUltAttack ||
            state == Character.CharacterState.CastUltSkill) {
            uiRoot.SetActive(false);
        }
        else {
            uiRoot.SetActive(true);
        }
    }

    private void Toughness_OnValueChanged(float value) {
        toughnessSlider.value = value / _character.Data.Toughness.CurrentMax;
    }

    private void HP_OnValueChanged(float value) {
        hpSlider.value = value / _character.Data.HP.CurrentMax;
    }

    private void Update() {
        transform.LookAt(Camera.main.transform.position);
        transform.forward = -transform.forward;
        transform.position = _character.meshParent.transform.position;
    }

    private void UpdateWeakness() {
        for (int i = 0; i < weaknessRoot.childCount; ++i) {
            Destroy(weaknessRoot.GetChild(i));
        }

        ElementType iterator = ElementType.Imaginary;
        int idx = 0;
        int found = 0;
        while (iterator > 0) {
            if ((iterator & _character.Data.Weakness) > 0) {
                var go = Instantiate(weaknessPrefab, weaknessRoot.position, weaknessRoot.rotation);
                var rect = go.GetComponent<RectTransform>();
                rect.SetParent(weaknessRoot);
                rect.anchoredPosition = new Vector2(-weaknessPadding + -weaknessPadding * 2 * found, 0);
                go.GetComponent<Image>().sprite = weaknessSprites[idx];
                found++;
            }
            iterator = (ElementType)((uint)iterator >> 1);
            idx++;
        }
    }
}
