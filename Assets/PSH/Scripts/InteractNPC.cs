using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractNPC : MonoBehaviour
{
    private bool isInInteractZone = false;  // ��ȣ�ۿ� ���� ����
    private GameObject currentNPC;          // ���� ��ȣ�ۿ� ������ NPC
    public GameObject miniGameUI;           // ������ UI ������Ʈ (��Ȱ��ȭ�� ���¿��� ��)
    private PlayerMove playerMove;          // PlayerMove ��ũ��Ʈ ����

    void Start()
    {
        playerMove = FindObjectOfType<PlayerMove>(); // PlayerMove ��ũ��Ʈ ã��
    }

    void Update()
    {
        // F Ű�� ������ ��ȣ�ۿ� �õ�
        if (isInInteractZone && Input.GetKeyDown(KeyCode.F))
        {
            if (currentNPC != null)
            {
                // MinigameNPC�� ��ȣ�ۿ� �� Ư�� UI Ȱ��ȭ
                if (currentNPC.name == "MinigameNPC")
                {
                    miniGameUI.SetActive(true);
                    playerMove.isMovementEnabled = false; // �÷��̾� �̵� ��Ȱ��ȭ
                    Debug.Log("MinigameNPC�� ��ȣ�ۿ��Ͽ� UI â Ȱ��ȭ �� �̵� ��Ȱ��ȭ");
                }
                else if (currentNPC.CompareTag("NPC"))
                {
                    // NPC �±װ� �ִ� �ٸ� ������Ʈ�� ��ȣ�ۿ�
                    Debug.Log(currentNPC.name + "�� ��ȣ�ۿ��߽��ϴ�. (�Ϲ� NPC)");
                    // ���߿� �ٸ� ��ȣ�ۿ� �߰� ����
                }
            }
        }

        // ESC Ű�� ������ �� UI â�� ��Ȱ��ȭ�ϰ� �÷��̾� �̵� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            miniGameUI.SetActive(false);
            playerMove.isMovementEnabled = true; // �÷��̾� �̵� Ȱ��ȭ
            Debug.Log("ESC Ű�� ���� UI â ��Ȱ��ȭ �� �̵� Ȱ��ȭ");
        }
    }

    // ��ȣ�ۿ� ������ ������ ��
    void OnTriggerEnter(Collider other)
    {
        // NPC �±װ� �ִ� ������Ʈ�� �浹�ߴ��� Ȯ��
        if (other.CompareTag("NPC"))
        {
            isInInteractZone = true;
            currentNPC = other.gameObject;
            Debug.Log(currentNPC.name + "�� ��ȣ�ۿ� ����");
        }
    }

    // ��ȣ�ۿ� �������� ������ ��
    void OnTriggerExit(Collider other)
    {
        // NPC �±װ� �ִ� ������Ʈ���� ������ ��ȣ�ۿ� �Ұ���
        if (other.CompareTag("NPC"))
        {
            isInInteractZone = false;
            currentNPC = null;
            Debug.Log("��ȣ�ۿ� �Ұ�");
        }
    }

    // ��ư�� ������ �� �� ��ȯ
    public void LoadSusemiScene()
    {
        SceneManager.LoadScene("Susemi");  // Susemi ������ ��ȯ
        Debug.Log("Susemi ������ ��ȯ");
    }
}