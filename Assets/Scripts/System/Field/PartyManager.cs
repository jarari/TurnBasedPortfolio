using UnityEngine;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance; // 싱글톤 인스턴스

    private string[] characters = new string[4]; // 데이터에서 가져온 캐릭터 ID를 저장할 배열
    private string[] party = new string[3]; // 파티에 편성된 캐릭터의 ID를 저장할 배열

    private void Awake()
    {
        if (Instance == null)
            Instance = this; // 인스턴스 설정
        else
            Destroy(gameObject); // 중복된 인스턴스 삭제
    }

    private void Start()
    {
        /*
        // 데이터 로드
        CharacterDataManager.Instance.LoadCharacterData();

        // 데이터에서 캐릭터 ID를 가져와서 배열에 저장
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = CharacterDataManager.Instance.GetCharacterDataByIndex(i).ID;
        }
        */
    }
    
    public bool IsPartyFull()
    {
        for (int i = 0; i < party.Length; i++) // 파티 배열을 순회
        {
            if (party[i] == null) // 빈 슬롯이 있으면
                return false; // 파티가 가득 차지 않음
        }
        return true; // 파티가 가득 참
    }

    public bool IsCharacterInParty(string ID)
    {
        for (int i = 0; i < party.Length; i++) // 파티 배열을 순회
        {
            if (party[i] == ID) // 파티에 있는 캐릭터 ID와 비교
                return true; // 이미 파티에 있음
        }
        return false; // 파티에 없음
    }

    public void AddCharacterToParty(string ID)
    {
        for (int i = 0; i < party.Length; i++) // 파티 배열을 순회
        {
            if (party[i] == null) // 빈 슬롯을 찾으면
            {
                party[i] = ID; // 빈 슬롯에 캐릭터 ID 추가
                break;
            }
        }
    }

    public void RemoveCharacterFromParty(string ID)
    {
        for (int i = 0; i < party.Length; i++) // 파티 배열을 순회
        {
            if (party[i] == ID) // 일치하는 캐릭터 ID를 찾으면
            {
                party[i] = null; // 슬롯 비우기
                break;
            }
        }
    }

    public int GetPartyNumber(string ID)
    {
        for (int i = 0; i < party.Length; i++) // 파티 배열을 순회
        {
            if (party[i] == ID) // 일치하는 캐릭터 ID를 찾으면
                return i; // 슬롯 번호 반환
        }
        return -1; // 파티에 없음
    }

    public string[] GetParty()
    {
        return party; // 현재 파티 반환
    }
}
