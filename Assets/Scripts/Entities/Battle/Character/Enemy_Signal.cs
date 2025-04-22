using UnityEngine;
using TurnBased.Battle;
using TurnBased.Battle.Managers;

namespace TurnBased.Entities.Battle {

    /// <summary>
    /// Dead 애니메이션 실행후 마지막의 이벤트를 실행할때 실행될 클래스
    /// </summary>
    public class Enemy_Signal : Character
    {
        public void SignalDead()
        {
            Debug.Log("수신!");
            // 오브젝트 모델을 비활성화 한다
            gameObject.SetActive(false);            
        }

    }

}