using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;

namespace TurnBased.Entities.Battle {

    /// <summary>
    /// Dead �ִϸ��̼� ������ �������� �̺�Ʈ�� �����Ҷ� ����� Ŭ����
    /// </summary>
    public class Enemy_Signal : Character
    {
        public void SignalDead()
        {
            Debug.Log("����!");
            this.gameObject.SetActive(false);
        }

    }

}