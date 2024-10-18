using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNPC : MonoBehaviour
{
    private bool isInInteractZone = false;  // 상호작용 가능 여부
    private GameObject currentNPC;          // 현재 상호작용 가능한 NPC

    void Update()
    {
        // F 키를 누르면 상호작용 시도
        if (isInInteractZone && Input.GetKeyDown(KeyCode.F))
        {
            // 상호작용 테스트 메시지 출력
            Debug.Log("NPC와 상호작용합니다: " + currentNPC.name);
            currentNPC.GetComponent<InteractUser>().Chat("비염에 좋은 약재를 추천해 주세요.");
        }
    }

    // 상호작용 영역에 들어왔을 때
    void OnTriggerEnter(Collider other)
    {
        // NPC 태그가 있는 오브젝트와 충돌했는지 확인
        if (other.CompareTag("NPC"))
        {
            isInInteractZone = true;
            currentNPC = other.gameObject;
            Debug.Log("NPC 상호작용 가능: " + currentNPC.name);
            
        }
    }

    // 상호작용 영역에서 나갔을 때
    void OnTriggerExit(Collider other)
    {
        // NPC 태그가 있는 오브젝트에서 나가면 상호작용 불가능
        if (other.CompareTag("NPC"))
        {
            isInInteractZone = false;
            currentNPC = null;
            Debug.Log("NPC 상호작용 불가");
        }
    }
}