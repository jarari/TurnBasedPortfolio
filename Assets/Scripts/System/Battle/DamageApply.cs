using UnityEngine;
using TurnBased.Battle;

// ���� �������� �����ϱ����� ���� ���̽�
public interface IDamageApply
{
    // ���⿡ �����ϰ� ���Ǵ� ���ʹ� �ʿ��� �Ѵ�
    // ��ȣ ���� ���� ��
    // ������ ��꿡 ���� ĳ������ ä��, ���ε��� ��� ������ �� �������̽�
    public void Apply(Character ch);
}
