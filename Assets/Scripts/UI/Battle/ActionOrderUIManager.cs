using System.Collections.Generic;
using TurnBased.Battle.Managers;
using UnityEngine;

namespace TurnBased.Battle.UI
{
    public class ActionOrderUIManager : MonoBehaviour
    {
        public static ActionOrderUIManager instance; // 싱글톤 인스턴스

        public List<GameObject> actionOrderUIObjects; // 행동 서열 UI 오브젝트 리스트

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this; // 싱글톤 인스턴스 설정
        }

        public void UpdateActionOrderUI()
        {
            List<Character> turnOrder = TurnManager.instance.GetActionOrder(); // TurnManager에서 행동 서열 리스트 가져오기

            //Character currentCharacter = TurnManager.instance.CurrentCharacter; // 현재 캐릭터 가져오기

            //Debug.Log("Current Character: " + currentCharacter.name); // 현재 캐릭터 이름 출력

            //// 현재 캐릭터를 가장 위에 표시
            //GameObject currentCharacterUI = actionOrderUIObjects.Find(obj => obj.name == currentCharacter.name); // 현재 캐릭터 UI 오브젝트 찾기
            //if (currentCharacterUI != null)
            //{
            //    RectTransform rectTransform = currentCharacterUI.GetComponent<RectTransform>(); // RectTransform 가져오기
            //    if (rectTransform != null)
            //        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -40); // 위치 설정
            //}

            //// 턴 순서에서 현재 캐릭터를 제외한 나머지 캐릭터들을 순서대로 표시
            //int positionIndex = 1; // 위치 인덱스 초기화
            //foreach (var character in turnOrder)
            //{
            //    if (character.name == currentCharacter.name)
            //        continue; // 현재 턴을 잡고 있는 캐릭터는 이미 젤 위에 표시했으니 제외

            //    GameObject uiObject = actionOrderUIObjects.Find(obj => obj.name == character.name); // UI 오브젝트 찾기
            //    if (uiObject != null)
            //    {
            //        RectTransform rectTransform = uiObject.GetComponent<RectTransform>(); // RectTransform 가져오기
            //        if (rectTransform != null)
            //        {
            //            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -40 - (positionIndex * 90)); // 위치 설정
            //            positionIndex++; // 위치 인덱스 증가
            //        }
            //    }
            //}
        }
    }
}
