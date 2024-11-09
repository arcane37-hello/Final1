using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractNPC : MonoBehaviour
{
    private bool isInInteractZone = false;  // 상호작용 가능 여부
    private GameObject currentNPC;          // 현재 상호작용 가능한 NPC
    public GameObject miniGameUI;           // 연결할 UI 오브젝트 (비활성화된 상태여야 함)
    public GameObject chatUI;               // 한의사 NPC와 채팅하는 코드 구현
    private PlayerMoveNph playerMove;          // PlayerMove 스크립트 참조

    void Start()
    {
        playerMove = FindObjectOfType<PlayerMoveNph>(); // PlayerMove 스크립트 찾기
    }

    void Update()
    {
        // F 키를 누르면 상호작용 시도
        if (isInInteractZone && Input.GetKeyDown(KeyCode.F))
        {
            if (currentNPC != null)
            {
                // MinigameNPC와 상호작용 시 특정 UI 활성화
                if (currentNPC.name == "MinigameNPC")
                {
                    miniGameUI.SetActive(true);
                    playerMove.isMovementEnabled = false; // 플레이어 이동 비활성화
                    Debug.Log("MinigameNPC와 상호작용하여 UI 창 활성화 및 이동 비활성화");
                }
                else if (currentNPC.CompareTag("NPC"))
                {
                    // NPC 태그가 있는 다른 오브젝트와 상호작용
                    chatUI.SetActive(true);
                    playerMove.isMovementEnabled = false; // 플레이어 이동 비활성화
                    Debug.Log(currentNPC.name + "와 상호작용했습니다. (일반 NPC)");
                    // 나중에 다른 상호작용 추가 가능
                    // currentNPC.GetComponent<InteractUser>().Chat("비염에 좋은 약재를 추천해 주세요.");
                }
            }
        }

        // ESC 키를 눌렀을 때 UI 창을 비활성화하고 플레이어 이동 활성화
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            miniGameUI.SetActive(false);
            chatUI.SetActive(false);
            playerMove.isMovementEnabled = true; // 플레이어 이동 활성화
            Debug.Log("ESC 키를 눌러 UI 창 비활성화 및 이동 활성화");
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

    // 버튼을 눌렀을 때 씬 전환
    public void LoadSusemiScene()
    {
        SceneManager.LoadScene("Susemi");  // Susemi 씬으로 전환
        Debug.Log("Susemi 씬으로 전환");
    }
}