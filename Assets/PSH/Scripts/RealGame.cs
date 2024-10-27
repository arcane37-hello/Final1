using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealGame : MonoBehaviour
{
    public Text dialogueText;
    public GameObject[] grabObjects;
    public Kettle kettle;
    public GameObject boilingEffectPrefab;
    public Transform effectSpawnPoint;
    public GameObject objectToReplace;
    public GameObject replacementPrefab;

    private bool isReadyToBoil = false;
    public bool isBoiling = false;  // �� ���̱� �� ����
    private bool isReadyToPour = false;
    private int susemiCount = 0;  // ������ �߰� Ƚ��
    private float boilingStartTime;
    private float boilingStopTime;

    void Start()
    {
        ToggleGrabObjects(false);
        StartCoroutine(StartExperienceMode());
    }

    // ü�� ��� ���� �� ���������� �����ϴ� �ڷ�ƾ
    IEnumerator StartExperienceMode()
    {
        dialogueText.text = "ü�� ��带 �����ϰڽ��ϴ�.";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "���������� �����ô�.";
        yield return new WaitForSeconds(3f);

        ToggleGrabObjects(true);

        if (kettle != null)
        {
            isReadyToBoil = true;
        }
    }

    // ������ �߰� �� ȣ��Ǵ� �޼���
    public void AddSusemi()
    {
        susemiCount++;
        Debug.Log("�����̰� �߰��Ǿ����ϴ�. ���� ������ ����: " + susemiCount);
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

    void Update()
    {
        if (isReadyToBoil && Input.GetMouseButtonDown(0) && kettle != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == kettle.gameObject)
                {
                    boilingStartTime = Time.time;
                    StartBoilingTimer();
                    isReadyToBoil = false;
                }
            }
        }

        if (isReadyToPour && Input.GetMouseButtonDown(0) && kettle != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == kettle.gameObject)
                {
                    boilingStopTime = Time.time;
                    StopBoiling();
                    StartCoroutine(PourTeaAndReplaceObject());
                    isReadyToPour = false;
                }
            }
        }
    }

    IEnumerator BoilWater()
    {
        isBoiling = true;
        int timer = 0;

        if (boilingEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effectInstance = Instantiate(boilingEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            effectInstance.transform.SetParent(kettle.transform);
        }

        while (isBoiling)
        {
            dialogueText.text = "���� ���� ���Դϴ�. " + (timer / 60).ToString("D2") + ":" + (timer % 60).ToString("D2");
            yield return new WaitForSeconds(1f);
            timer++;
        }
    }

    public void StopBoiling()
    {
        isBoiling = false;
        dialogueText.text = "���� �ϼ��Ǿ����ϴ�. �򰡸� �޾ƺ��ô�!";
        Debug.Log("���� ���� �ð��� ��ϵǾ����ϴ�: " + (boilingStopTime - boilingStartTime) + "��");
        isReadyToPour = true;

        if (kettle.realTea != null)
        {
            kettle.realTea.RecordBoilingDuration(boilingStopTime - boilingStartTime);
        }
    }

    public void StartBoilingTimer()
    {
        if (!isBoiling)
        {
            StartCoroutine(BoilWater());
        }
    }

    IEnumerator PourTeaAndReplaceObject()
    {
        yield return StartCoroutine(kettle.MoveAndRotateKettle());

        if (objectToReplace != null && replacementPrefab != null)
        {
            Vector3 replacePosition = objectToReplace.transform.position;
            Quaternion replaceRotation = objectToReplace.transform.rotation;

            Destroy(objectToReplace);
            Instantiate(replacementPrefab, replacePosition, replaceRotation);
        }

        dialogueText.text = "���� �ϼ��Ǿ����ϴ�!";
    }

    public void UpdateText(string newText)
    {
        dialogueText.text = newText;
    }

    public void UpdateBoilingText(string newText)
    {
        dialogueText.text = newText;
    }
}
