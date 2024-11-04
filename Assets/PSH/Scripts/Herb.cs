using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb : MonoBehaviour
{
    public string stackName;  // 스택의 이름을 지정하는 변수
    private int stackCount = 0;  // 스택 개수 초기화

    // Cabinet 스크립트에서 호출되는 상호작용 메서드
    public void Interact()
    {
        stackCount++;  // 상호작용 시 스택 증가
        Debug.Log($"{stackName} 스택이 증가했습니다. 현재 개수: {stackCount}");
    }
}
