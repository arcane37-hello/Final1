using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    private Button retryButton;  // RetryButton ����
    public Text targetText;  // ������ �ؽ�Ʈ ������Ʈ
    public string nextText = "Ʃ�丮���� �������ϴ� ���� ���� �����ô�";  // ���ŵ� �ؽ�Ʈ

    void Start()
    {
        // RetryButton�� ã�Ƽ� ��Ȱ��ȭ
        retryButton = GameObject.Find("RetryButton")?.GetComponent<Button>();
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(false); // ���� �� ��Ȱ��ȭ
        }
        else
        {
            Debug.LogError("RetryButton�� ã�� �� �����ϴ�.");
        }

        // targetText�� �Ҵ���� �ʾ��� ��� ���� �α� ���
        if (targetText == null)
        {
            Debug.LogError("������ �ؽ�Ʈ ������Ʈ�� �������� �ʾҽ��ϴ�.");
        }
    }

    // "�Ϻ��ϰ� ����̳׿� ���ϼ̽��ϴ�" �ؽ�Ʈ�� ��µ� �� ����Ǵ� �Լ�
    public void OnEvaluationComplete()
    {
        if (targetText != null)
        {
            // 5�� ��� �� �ؽ�Ʈ ���� �� ��ư Ȱ��ȭ
            StartCoroutine(UpdateTextAndActivateButton());
        }
    }

    // �ؽ�Ʈ ���� �� ��ư Ȱ��ȭ �ڷ�ƾ
    IEnumerator UpdateTextAndActivateButton()
    {
        // 5�� ���
        yield return new WaitForSeconds(5f);

        // ������ �ؽ�Ʈ�� ����
        targetText.text = nextText;

        // RetryButton Ȱ��ȭ
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(true);
        }
    }
}