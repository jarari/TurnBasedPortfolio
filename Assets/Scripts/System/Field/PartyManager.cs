using UnityEngine;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance; // �̱��� �ν��Ͻ�

    private string[] characters = new string[4]; // �����Ϳ��� ������ ĳ���� ID�� ������ �迭
    private string[] party = new string[3]; // ��Ƽ�� ���� ĳ������ ID�� ������ �迭

    private void Awake()
    {
        if (Instance == null)
            Instance = this; // �ν��Ͻ� ����
        else
            Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
    }

    private void Start()
    {
        /*
        // ������ �ε�
        CharacterDataManager.Instance.LoadCharacterData();

        // �����Ϳ��� ĳ���� ID�� �����ͼ� �迭�� ����
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = CharacterDataManager.Instance.GetCharacterDataByIndex(i).ID;
        }
        */
    }
    
    public bool IsPartyFull()
    {
        for (int i = 0; i < party.Length; i++) // ��Ƽ �迭�� ��ȸ
        {
            if (party[i] == null) // �� ������ ������
                return false; // ��Ƽ�� ���� ���� ����
        }
        return true; // ��Ƽ�� ���� ��
    }

    public bool IsCharacterInParty(string ID)
    {
        for (int i = 0; i < party.Length; i++) // ��Ƽ �迭�� ��ȸ
        {
            if (party[i] == ID) // ��Ƽ�� �ִ� ĳ���� ID�� ��
                return true; // �̹� ��Ƽ�� ����
        }
        return false; // ��Ƽ�� ����
    }

    public void AddCharacterToParty(string ID)
    {
        for (int i = 0; i < party.Length; i++) // ��Ƽ �迭�� ��ȸ
        {
            if (party[i] == null) // �� ������ ã����
            {
                party[i] = ID; // �� ���Կ� ĳ���� ID �߰�
                break;
            }
        }
    }

    public void RemoveCharacterFromParty(string ID)
    {
        for (int i = 0; i < party.Length; i++) // ��Ƽ �迭�� ��ȸ
        {
            if (party[i] == ID) // ��ġ�ϴ� ĳ���� ID�� ã����
            {
                party[i] = null; // ���� ����
                break;
            }
        }
    }

    public int GetPartyNumber(string ID)
    {
        for (int i = 0; i < party.Length; i++) // ��Ƽ �迭�� ��ȸ
        {
            if (party[i] == ID) // ��ġ�ϴ� ĳ���� ID�� ã����
                return i; // ���� ��ȣ ��ȯ
        }
        return -1; // ��Ƽ�� ����
    }

    public string[] GetParty()
    {
        return party; // ���� ��Ƽ ��ȯ
    }
}
