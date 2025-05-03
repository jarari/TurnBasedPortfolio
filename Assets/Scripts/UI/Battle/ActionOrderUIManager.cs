using System.Collections.Generic;
using TurnBased.Battle.Managers;
using TurnBased.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBased.Battle.UI
{
    public class ActionOrderUIManager : MonoBehaviour
    {
        public static ActionOrderUIManager instance; // �̱��� �ν��Ͻ�

        public List<GameObject> actionOrderUIObjects; // �ൿ ���� UI ������Ʈ ����Ʈ

        private void Awake()
        {
            if (instance == null)
            {
                instance = this; // �̱��� �ν��Ͻ� ����
            }
            else
            {
                Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
            }
        }

        private void Start()
        {
            UpdateActionOrderUIPositions(); // �ൿ ���� UI �ʱ�ȭ
        }

        public void UpdateActionOrderUIPositions()
        {
            List<Character> actionOrder = TurnManager.instance.GetActionOrder(); // �ൿ ���� ��������

            // �ൿ ������ 1�� �̻��� ��, ������ ĳ���͸� ù ��°�� �̵�
            if (actionOrder.Count > 1)
            {
                Character lastCharacter = actionOrder[actionOrder.Count - 1]; // ������ ĳ���� ����
                actionOrder.RemoveAt(actionOrder.Count - 1); // ������ ĳ���� ����
                actionOrder.Insert(0, lastCharacter); // ������ ĳ���͸� ù ��°�� �̵�
            }

            // UI ������Ʈ�� Ȱ��ȭ ���¸� ������Ʈ
            for (int i = 0; i < actionOrderUIObjects.Count; i++)
            {
                // �ൿ ������ ���̺��� UI ������Ʈ�� ���̰� ª�� ���, UI ������Ʈ�� ��Ȱ��ȭ
                if (i < actionOrder.Count)
                {
                    Character character = actionOrder[i]; // �ൿ �������� ĳ���� ��������
                    CharacterData characterData = character.Data.BaseData; // ĳ���� ������ ��������

                    actionOrderUIObjects[i].SetActive(true); // UI ������Ʈ Ȱ��ȭ

                    string imagePath = characterData.CharacterImagePath; // ĳ���� �̹��� ��� ��������
                    Sprite characterSprite = Resources.Load<Sprite>(imagePath); // ���ҽ����� ��������Ʈ �ε�

                    // UI ������Ʈ�� �̹��� ������Ʈ ����
                    Transform imageTransform = actionOrderUIObjects[i].transform.Find("Image_Character"); // �̹��� Ʈ������ ã��
                    
                    if (imageTransform != null)
                    {
                        Image imageComponent = imageTransform.GetComponent<Image>(); // �̹��� ������Ʈ ��������
                        if (imageComponent != null && characterSprite != null) 
                        {
                            imageComponent.sprite = characterSprite; // ��������Ʈ ����
                            imageComponent.preserveAspect = true; // ���� ���� ����
                        }
                        else if (imageComponent != null) imageComponent.sprite = null; // ��������Ʈ�� ���� ��� null ����
                    }

                    // UI ������Ʈ�� �ؽ�Ʈ ������Ʈ ����
                    Text textComponent = actionOrderUIObjects[i].GetComponentInChildren<Text>(); // �ؽ�Ʈ ������Ʈ ã��

                    if (textComponent != null)
                    {
                        if (i == 0) textComponent.text = "0"; // ù ��° ĳ������ ��� ���� �ൿ���� 0���� ����
                        else
                        {
                            float remainingTime = TurnManager.instance.GetRemainingTime(character); // ���� �ð� ��������
                            int remainingTimeInt = Mathf.FloorToInt(remainingTime); // ���� �ð��� ������ ��ȯ
                            textComponent.text = remainingTimeInt.ToString(); // �ؽ�Ʈ ����
                        }
                    }
                }
                else actionOrderUIObjects[i].SetActive(false); // �Ҵ���� ���� UI ������Ʈ ��Ȱ��ȭ
            }
        }
    }
}
