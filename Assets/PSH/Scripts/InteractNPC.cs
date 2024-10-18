using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNPC : MonoBehaviour
{
    private bool isInInteractZone = false;  // 상호작용 가능 여부
    private GameObject currentNPC;          // 현재 상호작용 가능한 NPC
    public GameObject miniGameUI;           // 연결할 UI 오브젝트 (비활성화된 상태여야 함)

    void Update()
    {
        // F 키를 누르면 상호작용 시도
        if (isInInteractZone && Input.GetKeyDown(KeyCode.F))
        {
            if (currentNPC != null && currentNPC.name == "MinigameNPC")
            {
                // UI 창을 활성화
                miniGameUI.SetActive(true);
                Debug.Log("MinigameNPC와 상호작용하여 UI 창 활성화");
            }
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
            Debug.Log(currentNPC.name + "와 상호작용 가능");
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
            Debug.Log("상호작용 불가");
        }
    }
}