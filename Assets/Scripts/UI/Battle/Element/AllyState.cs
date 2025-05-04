using System.Collections.Generic;
using TurnBased.Battle.Managers;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;


namespace TurnBased.Battle.UI.Element {
    public class AllyState : MonoBehaviour {
        [SerializeField]
        private RawImage portraitImage;
        [SerializeField]
        private Slider ultSlider;
        [SerializeField]
        private RawImage ultImage;
        [SerializeField]
        private Slider hpSlider;
        [SerializeField]
        private Text hpText;

        private Character _character;
        private float _prevUltPts;


        private void UpdateCharacterHP(float value) {
            hpSlider.value = value / _character.Data.HP.CurrentMax;
            hpText.text = Mathf.Ceil(_character.Data.HP.Current).ToString();
        }

        private void UpdateCharacterUltPts(float value) {
            if (_prevUltPts < _character.Data.UltThreshold && value >= _character.Data.UltThreshold) {
                SoundManager.instance.Play2DSound("UIUltStandby");
            }
            _prevUltPts = value;
            ultSlider.value = value / _character.Data.UltPts.CurrentMax;
        }

        private void InitializeCharacterImages() {
            // 캐릭터 데이터에서 이미지 경로 가져오기
            string imagePath = _character.Data.BaseData.CharacterImagePath;

            // 이미지 로드
            Texture portraitTex = Resources.Load<Texture>(imagePath);

            if (portraitTex != null) {
                // UI에 이미지 설정
                portraitImage.texture = portraitTex;
                portraitImage.gameObject.SetActive(true);
            }
            else {
                portraitImage.gameObject.SetActive(false);
            }

            // 캐릭터 데이터에서 이미지 경로 가져오기
            imagePath = _character.Data.BaseData.UltimateImagePath;

            // 이미지 로드
            Texture ultTex = Resources.Load<Texture>(imagePath);

            if (ultTex != null) {
                // UI에 이미지 설정
                ultImage.texture = ultTex;
                ultImage.gameObject.SetActive(true);
            }
            else {
                ultImage.gameObject.SetActive(false);
            }
        }

        private void RegisterListeners() {
            _character.Data.HP.OnValueChanged += UpdateCharacterHP;
            UpdateCharacterHP(_character.Data.HP.Current);
            _character.Data.UltPts.OnValueChanged += UpdateCharacterUltPts;
            UpdateCharacterUltPts(_character.Data.UltPts.Current);
        }

        public void InitializeAllyUI(Character c) {
            _character = c;

            InitializeCharacterImages();
            RegisterListeners();
        }
    }

}