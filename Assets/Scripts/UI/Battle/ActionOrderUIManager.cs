using System.Collections.Generic;
using TurnBased.Battle.Managers;
using UnityEngine;

namespace TurnBased.Battle.UI
{
    public class ActionOrderUIManager : MonoBehaviour
    {
        public static ActionOrderUIManager instance; // �̱��� �ν��Ͻ�

        public List<GameObject> actionOrderUIObjects; // �ൿ ���� UI ������Ʈ ����Ʈ

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this; // �̱��� �ν��Ͻ� ����
        }

        public void UpdateActionOrderUI()
        {
            List<Character> turnOrder = TurnManager.instance.GetActionOrder(); // TurnManager���� �ൿ ���� ����Ʈ ��������

            //Character currentCharacter = TurnManager.instance.CurrentCharacter; // ���� ĳ���� ��������

            //Debug.Log("Current Character: " + currentCharacter.name); // ���� ĳ���� �̸� ���

            //// ���� ĳ���͸� ���� ���� ǥ��
            //GameObject currentCharacterUI = actionOrderUIObjects.Find(obj => obj.name == currentCharacter.name); // ���� ĳ���� UI ������Ʈ ã��
            //if (currentCharacterUI != null)
            //{
            //    RectTransform rectTransform = currentCharacterUI.GetComponent<RectTransform>(); // RectTransform ��������
            //    if (rectTransform != null)
            //        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -40); // ��ġ ����
            //}

            //// �� �������� ���� ĳ���͸� ������ ������ ĳ���͵��� ������� ǥ��
            //int positionIndex = 1; // ��ġ �ε��� �ʱ�ȭ
            //foreach (var character in turnOrder)
            //{
            //    if (character.name == currentCharacter.name)
            //        continue; // ���� ���� ��� �ִ� ĳ���ʹ� �̹� �� ���� ǥ�������� ����

            //    GameObject uiObject = actionOrderUIObjects.Find(obj => obj.name == character.name); // UI ������Ʈ ã��
            //    if (uiObject != null)
            //    {
            //        RectTransform rectTransform = uiObject.GetComponent<RectTransform>(); // RectTransform ��������
            //        if (rectTransform != null)
            //        {
            //            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -40 - (positionIndex * 90)); // ��ġ ����
            //            positionIndex++; // ��ġ �ε��� ����
            //        }
            //    }
            //}
        }
    }
}
