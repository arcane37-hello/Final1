using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMinigame : MonoBehaviour
{
    public Text dialogueText;  // UI �ؽ�Ʈ�� ����
    public GameObject[] grabObjects;  // GrabObject ��ũ��Ʈ�� ���� ������Ʈ��

    void Start()
    {
        // ó������ GrabObject ��ũ��Ʈ�� ��Ȱ��ȭ�� ���·� ����
        ToggleGrabObjects(false);

        // ù ��° �ؽ�Ʈ ���
        StartCoroutine(DisplayTextSequence());
    }

    // �ؽ�Ʈ ������� ����ϴ� �ڷ�ƾ
    IEnumerator DisplayTextSequence()
    {
        // ù ��° �ؽ�Ʈ ���
        dialogueText.text = "�������� ����� ü�迡 ���� �� ȯ���մϴ�";
        yield return new WaitForSeconds(3f);  // 3�� ���

        // �� ��° �ؽ�Ʈ ���
        dialogueText.text = "ó���̽ô� Ʃ�丮���� �����ϰڽ��ϴ�";
        yield return new WaitForSeconds(3f);  // 3�� ���


        // �� ��° �ؽ�Ʈ ���
        dialogueText.text = "���� ���� �����̸� �����ڿ� �־� �����?";
        yield return new WaitForSeconds(3f);  // 3�� ���

        // �� ��° �ؽ�Ʈ ���
        dialogueText.text = "������ ���� �� �� �� 15g �Դϴ� 5���� �־�� �մϴ�";
        yield return new WaitForSeconds(3f);  // 3�� ���

        // GrabObject ��ũ��Ʈ Ȱ��ȭ
        ToggleGrabObjects(true);
    }

    // �ܺο��� �ؽ�Ʈ�� ������ �� �ִ� �Լ�
    public void UpdateText(string newText)
    {
        dialogueText.text = newText;  // �ؽ�Ʈ ����
    }

    // GrabObject ��ũ��Ʈ�� Ȱ��ȭ�ǰų� ��Ȱ��ȭ�ǵ��� �����ϴ� �Լ�
    void ToggleGrabObjects(bool isActive)
    {
        foreach (GameObject obj in grabObjects)
        {
            GrabObject grabScript = obj.GetComponent<GrabObject>();
            if (grabScript != null)
            {
                grabScript.enabled = isActive;
            }
        }
    }
}