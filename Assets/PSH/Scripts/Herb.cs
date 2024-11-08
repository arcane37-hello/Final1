using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : MonoBehaviour
{
    public string stackName;           // 스택의 이름을 지정하는 변수
    public GameObject spawnPrefab;     // 소환할 프리팹
    private int stackCount = 0;        // 스택 개수 초기화

    // 상호작용 시 스택 증가
    public void Interact()
    {
        stackCount++;
        Debug.Log($"{stackName} 스택이 증가했습니다. 현재 개수: {stackCount}");
    }

    // 스택이 존재하는지 확인하는 메서드
    public bool HasStack()
    {
        return stackCount > 0;
    }

    // 현재 스택 수를 반환하는 메서드
    public int GetStackCount()
    {
        return stackCount;
    }

    // 스택을 모두 소모하는 메서드
    public void ClearStack()
    {
        stackCount = 0;
        Debug.Log($"{stackName} 스택이 모두 소모되었습니다.");
    }
}
