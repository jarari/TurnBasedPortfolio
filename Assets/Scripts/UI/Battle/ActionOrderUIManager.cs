using System.Collections.Generic;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBased.Battle.UI
{
    public class ActionOrderUIManager : MonoBehaviour
    {
        public static ActionOrderUIManager instance; // 싱글톤 인스턴스

        public List<GameObject> actionOrderUIObjects; // 행동 서열 UI 오브젝트 리스트

        private void Awake()
        {
            if (instance == null)
            {
                instance = this; // 싱글톤 인스턴스 설정
            }
            else
            {
                Destroy(gameObject); // 중복 인스턴스 제거
            }
        }

        private void Start()
        {
            UpdateActionOrderUIPositions(); // 행동 서열 UI 초기화
        }

        public void UpdateActionOrderUIPositions()
        {
            List<Character> actionOrder = TurnManager.instance.GetActionOrder(); // 행동 서열 가져오기

            // 행동 서열이 1개 이상일 때, 마지막 캐릭터를 첫 번째로 이동
            if (actionOrder.Count > 1)
            {
                Character lastCharacter = actionOrder[actionOrder.Count - 1]; // 마지막 캐릭터 저장
                actionOrder.RemoveAt(actionOrder.Count - 1); // 마지막 캐릭터 제거
                actionOrder.Insert(0, lastCharacter); // 마지막 캐릭터를 첫 번째로 이동
            }

            // UI 오브젝트의 활성화 상태를 업데이트
            for (int i = 0; i < actionOrderUIObjects.Count; i++)
            {
                // 행동 서열의 길이보다 UI 오브젝트의 길이가 짧을 경우, UI 오브젝트를 비활성화
                if (i < actionOrder.Count)
                {
                    Character character = actionOrder[i]; // 행동 서열에서 캐릭터 가져오기
                    CharacterData characterData = character.Data.BaseData; // 캐릭터 데이터 가져오기

                    actionOrderUIObjects[i].SetActive(true); // UI 오브젝트 활성화

                    string imagePath = characterData.CharacterImagePath; // 캐릭터 이미지 경로 가져오기
                    Sprite characterSprite = Resources.Load<Sprite>(imagePath); // 리소스에서 스프라이트 로드

                    // UI 오브젝트의 이미지 컴포넌트 설정
                    Transform imageTransform = actionOrderUIObjects[i].transform.Find("Image_Character"); // 이미지 트랜스폼 찾기
                    
                    if (imageTransform != null)
                    {
                        Image imageComponent = imageTransform.GetComponent<Image>(); // 이미지 컴포넌트 가져오기
                        if (imageComponent != null && characterSprite != null) 
                        {
                            imageComponent.sprite = characterSprite; // 스프라이트 설정
                            imageComponent.preserveAspect = true; // 비율 유지 설정
                        }
                        else if (imageComponent != null) imageComponent.sprite = null; // 스프라이트가 없을 경우 null 설정
                    }

                    // UI 오브젝트의 텍스트 컴포넌트 설정
                    Text textComponent = actionOrderUIObjects[i].GetComponentInChildren<Text>(); // 텍스트 컴포넌트 찾기

                    if (textComponent != null)
                    {
                        if (i == 0) textComponent.text = "0"; // 첫 번째 캐릭터의 경우 남은 행동력을 0으로 설정
                        else
                        {
                            float remainingTime = TurnManager.instance.GetRemainingTime(character); // 남은 시간 가져오기
                            int remainingTimeInt = Mathf.FloorToInt(remainingTime); // 남은 시간을 정수로 변환
                            textComponent.text = remainingTimeInt.ToString(); // 텍스트 설정
                        }
                    }
                }
                else actionOrderUIObjects[i].SetActive(false); // 할당되지 않은 UI 오브젝트 비활성화
            }
        }
    }
}
