using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMinigame : MonoBehaviour
{
    public Text dialogueText;  // UI �ؽ�Ʈ ����
    public GameObject[] grabObjects;  // GrabObject ��ũ��Ʈ�� ���� ������Ʈ��
    public Button yesButton;   // YesButton ����
    public Button noButton;    // NoButton ����
    public GameObject realGame;  // ü�� ��带 ���� RealGame ��ũ��Ʈ�� ���� ������Ʈ
    public Kettle kettle;
    void Start()
    {
        // ó������ GrabObject ��ũ��Ʈ�� ��Ȱ��ȭ�� ���·� ����
        ToggleGrabObjects(false);

        // ó������ ��ư�� ��Ȱ��ȭ
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // ù ��° �ؽ�Ʈ ���
        StartCoroutine(DisplayTextSequence());
    }

    // �ؽ�Ʈ ������� ����ϴ� �ڷ�ƾ
    IEnumerator DisplayTextSequence()
    {
        // ù ��° �ؽ�Ʈ ���
        dialogueText.text = "�������� ����� ü�迡 ���� �� ȯ���մϴ�";
        yield return new WaitForSeconds(3f);  // 3�� ���

        // �� ��° ���� �ؽ�Ʈ ���
        dialogueText.text = "�������� ����ô� �� ó���̽Ű���?";

        // YesButton�� NoButton Ȱ��ȭ
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        // YesButton Ŭ�� �� Ʃ�丮�� ����
        yesButton.onClick.AddListener(StartTutorial);
        noButton.onClick.AddListener(StartExperienceMode);
    }

    // YesButton Ŭ�� �� Ʃ�丮�� ����
    void StartTutorial()
    {
        // ��ư ��Ȱ��ȭ
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // �� ��° �ؽ�Ʈ ��� ����
        dialogueText.text = "�˰ڽ��ϴ� �׷��� Ʃ�丮���� �����ϰڽ��ϴ�";

        // Ʃ�丮�� ����
        StartCoroutine(StartTutorialSequence());
    }

    // Ʃ�丮�� ����� �������� ���� �ڷ�ƾ
    IEnumerator StartTutorialSequence()
    {
        // �� ��° �ؽ�Ʈ ���
        yield return new WaitForSeconds(3f);
        dialogueText.text = "���� ���� �����̸� �����ڿ� �־� �����?";
        yield return new WaitForSeconds(3f);

        // �� ��° �ؽ�Ʈ ���
        dialogueText.text = "������ ���� �� �� �� 3g �Դϴ� 5���� �־�� �մϴ�";
        yield return new WaitForSeconds(3f);

        // GrabObject ��ũ��Ʈ Ȱ��ȭ
        ToggleGrabObjects(true);
    }

    // NoButton Ŭ�� �� ü�� ��� ����
    public void StartExperienceMode()
    {
        // ��ư ��Ȱ��ȭ
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        kettle.isExperienceMode = true;

        // ü�� ��� ���� �ؽ�Ʈ ���
        dialogueText.text = "ü�� ��带 �����ϰڽ��ϴ�.";
        StartCoroutine(StartExperienceSequence());
    }

    // ü�� ���� ��ȯ�ϴ� �ڷ�ƾ
    IEnumerator StartExperienceSequence()
    {
        yield return new WaitForSeconds(3f);
        dialogueText.text = "���������� �����ô�";

        // PlayMinigame ��Ȱ��ȭ, RealGame Ȱ��ȭ
        this.gameObject.SetActive(false);  // PlayMinigame ������Ʈ ��Ȱ��ȭ
        realGame.SetActive(true);  // RealGame ������Ʈ Ȱ��ȭ
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

    // �ܺο��� �ؽ�Ʈ�� ������ �� �ִ� �Լ�
    public void UpdateText(string newText)
    {
        dialogueText.text = newText;  // �ؽ�Ʈ ����
    }
}