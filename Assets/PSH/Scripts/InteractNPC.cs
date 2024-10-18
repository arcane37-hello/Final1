using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNPC : MonoBehaviour
{
    private bool isInInteractZone = false;  // ��ȣ�ۿ� ���� ����
    private GameObject currentNPC;          // ���� ��ȣ�ۿ� ������ NPC
    public GameObject miniGameUI;           // ������ UI ������Ʈ (��Ȱ��ȭ�� ���¿��� ��)

    void Update()
    {
        // F Ű�� ������ ��ȣ�ۿ� �õ�
        if (isInInteractZone && Input.GetKeyDown(KeyCode.F))
        {
            if (currentNPC != null && currentNPC.name == "MinigameNPC")
            {
                // UI â�� Ȱ��ȭ
                miniGameUI.SetActive(true);
                Debug.Log("MinigameNPC�� ��ȣ�ۿ��Ͽ� UI â Ȱ��ȭ");
            }
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
}