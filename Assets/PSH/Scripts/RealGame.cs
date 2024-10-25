using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealGame : MonoBehaviour
{
    public Text dialogueText;  // UI �ؽ�Ʈ ����
    public GameObject[] grabObjects;  // GrabObject ��ũ��Ʈ�� ���� ������Ʈ��
    public Kettle kettle;  // Kettle ��ũ��Ʈ ����
    public GameObject boilingEffectPrefab;  // �� ���� �� ����� ����Ʈ ������
    public Transform effectSpawnPoint;  // ����Ʈ�� ������ ��ġ
    public GameObject objectToReplace;  // ��ü�� ��� ������Ʈ
    public GameObject replacementPrefab; // ��ü�� ������

    private bool isReadyToBoil = false;  // ���� ���� �غ� �Ǿ����� ����
    private bool isBoiling = false;  // ���� ���� �ִ��� ����
    private bool isReadyToPour = false;  // ���� ���� �غ� �Ǿ����� ����

    void Start()
    {
        // ó������ GrabObject ��ũ��Ʈ�� ��Ȱ��ȭ�� ���·� ����
        ToggleGrabObjects(false);

        // ü�� ��� ����
        StartCoroutine(StartExperienceMode());
    }

    // ü�� ��� ���� �� ���������� �����ϴ� �ڷ�ƾ
    IEnumerator StartExperienceMode()
    {
        dialogueText.text = "ü�� ��带 �����ϰڽ��ϴ�.";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "���������� �����ô�.";
        yield return new WaitForSeconds(3f);

        // GrabObject ��ȣ�ۿ� Ȱ��ȭ
        ToggleGrabObjects(true);

        // ü�� ��忡���� Kettle�� ��ȣ�ۿ� ����
        if (kettle != null)
        {
            isReadyToBoil = true;
        }
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
        // �� ���̱� �غ� �Ǿ���, ���콺 ���� ��ư Ŭ�� �� Kettle���� �� ���̱�
        if (isReadyToBoil && Input.GetMouseButtonDown(0) && kettle != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == kettle.gameObject)
                {
                    StartCoroutine(BoilWater());  // ü�� ��忡���� BoilWater �ڷ�ƾ ���
                    isReadyToBoil = false;  // �� ���̱� �߿� �ٽ� ������� �ʵ��� ����
                }
            }
        }

        // ���� ���� �غ� �Ǿ���, Kettle�� �ٽ� Ŭ���ϸ� �� ������ ��� �� ������Ʈ ��ü
        if (isReadyToPour && Input.GetMouseButtonDown(0) && kettle != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == kettle.gameObject)
                {
                    StartCoroutine(PourTeaAndReplaceObject());  // �� ������ ��� �� ������Ʈ ��ü
                    isReadyToPour = false;  // �� ������ �ߺ� ����
                }
            }
        }
    }

    // �� ���̱� �ڷ�ƾ (0���� �����ϴ� Ÿ�̸� + ����Ʈ)
    IEnumerator BoilWater()
    {
        isBoiling = true;
        int timer = 0;

        // �� ���� �� ����Ʈ ����
        if (boilingEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effectInstance = Instantiate(boilingEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            effectInstance.transform.SetParent(kettle.transform);  // ����Ʈ�� Kettle�� ���󰡵��� ����
        }

        while (isBoiling)
        {
            dialogueText.text = "���� ���� ���Դϴ�. " + (timer / 60).ToString("D2") + ":" + (timer % 60).ToString("D2");
            yield return new WaitForSeconds(1f);
            timer++;
        }

        // �� ���̱� �Ϸ� �� �ؽ�Ʈ ����
        dialogueText.text = "���� �ϼ��Ǿ����ϴ�. �򰡸� �޾ƺ��ô�!";
        isBoiling = false;
        isReadyToPour = true;  // ���� ���� �غ� �Ϸ�
    }

    // ���� ������ ������Ʈ�� ��ü�ϴ� �ڷ�ƾ
    IEnumerator PourTeaAndReplaceObject()
    {
        // �� ������ ��� (Kettle ����̱�)
        yield return StartCoroutine(kettle.MoveAndRotateKettle());  // Kettle ��ũ��Ʈ�� MoveAndRotateKettle ���

        // ��� ������Ʈ ��ü
        if (objectToReplace != null && replacementPrefab != null)
        {
            Vector3 replacePosition = objectToReplace.transform.position;
            Quaternion replaceRotation = objectToReplace.transform.rotation;

            Destroy(objectToReplace);  // ���� ������Ʈ ����
            Instantiate(replacementPrefab, replacePosition, replaceRotation);  // ���ο� ������ ����
        }

        dialogueText.text = "���� �ϼ��Ǿ����ϴ�!";
    }
}