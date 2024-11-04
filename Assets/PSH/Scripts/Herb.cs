using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : MonoBehaviour
{
    public string stackName;       // 스택의 이름을 지정하는 변수
    private int stackCount = 0;    // 스택 개수 초기화

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

    // 스택을 하나 감소시키는 메서드
    public void ReduceStack()
    {
        if (stackCount > 0)
        {
            stackCount--;
            Debug.Log($"{stackName} 스택이 감소했습니다. 현재 개수: {stackCount}");
        }
    }
}
