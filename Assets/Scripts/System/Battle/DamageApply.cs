using UnityEngine;
using TurnBased.Battle;

// 받은 데미지를 적용하기위한 인터 페이스
public interface IDamageApply
{
    // 여기에 선언하고 정의는 에너미 쪽에서 한다
    // 괄호 안은 때린 놈
    // 데미지 계산에 따라 캐릭터의 채력, 강인도를 까는 역할을 할 인터페이스
    public void Apply(Character ch);
}
