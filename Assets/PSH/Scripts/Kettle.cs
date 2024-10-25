using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kettle : MonoBehaviour
{
    private int teaLeafStack = 0;  // ���� ����
    public PlayMinigame playMinigame; // PlayMinigame ��ũ��Ʈ ���� (�ؽ�Ʈ ���ſ�)
    public Transform targetPosition;  // Kettle�� �̵��� ù ��° �� ������Ʈ�� ��ġ
    public GameObject objectToReplace;  // Ʃ�丮�� ��忡�� ��ü�� ��� ������Ʈ
    public GameObject replacementPrefab; // Ʃ�丮�� ��忡�� ��ü�� ������
    public GameObject boilingEffectPrefab; // �� ���� �� ����� ����Ʈ ������
    public Transform effectSpawnPoint;  // ����Ʈ�� ������ ��ġ
    private GameObject boilingEffectInstance;  // ������ ����Ʈ �ν��Ͻ�
    private Vector3 originalPosition;  // Kettle�� ���� ��ġ
    private Quaternion originalRotation;  // Kettle�� ���� ȸ����
    private bool isReadyToBoil = false;  // ���� ���� �غ� �Ǿ����� ����
    private bool isReadyToMove = false;  // ���� ���� �� �̵� �غ� ����
    public bool isExperienceMode = false;  // ü�� ��� ���� Ȯ��
    private bool hasBoiledWater = false;  // ���� �������� ���� Ȯ��
    private bool isPouring = false;  // �� ������ ��� ���� Ȯ��
    private bool isBoiling = false;  // �� ���̱� �� ����

    // ü�� ��忡�� ����� ��ü�� ������Ʈ�� ��ü�� �������� ���� ����
    public GameObject experienceObjectToReplace;  // ü�� ��忡�� ��ü�� ������Ʈ
    public GameObject experienceReplacementPrefab;  // ü�� ��忡�� ��ü�� ������ (���̾��Ű�� �ִ� ������Ʈ)

    // �������� �̵��� ��ǥ ���� (public���� �����Ͽ� ����Ƽ���� ���� ���� ����)
    public Transform experienceTargetPoint;  // ü�� ��忡�� ��ü�� �������� �̵��� ��ǥ ����

    void Start()
    {
        // �������� ���� ��ġ�� ȸ������ ����
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        // �±װ� Susemi�� ������Ʈ�� �浹�ߴ��� Ȯ��
        if (collision.gameObject.CompareTag("Susemi"))
        {
            // Susemi ������Ʈ�� �ı�
            Destroy(collision.gameObject);

            // ���� ���� ����
            teaLeafStack++;
            Debug.Log("Susemi ������Ʈ �ı���. ���� ���� ����: " + teaLeafStack);

            // ü�� ��尡 �ƴ� ���� �ؽ�Ʈ ����
            if (!isExperienceMode && teaLeafStack >= 5)
            {
                playMinigame.UpdateText("���ϼ̽��ϴ�! ���� ���� ����� ���� ���� �������ô�!");
                isReadyToBoil = true; // ���� ���� �� �ִ� ���·� ����
            }
        }
    }

    void Update()
    {
        // ���� ���̱� ������ ���� ��ȣ�ۿ�, ���� ���� ���� �� ������ ��ȣ�ۿ�
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray�� �����ڿ� �¾Ҵ��� Ȯ��
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
            {
                if (!isExperienceMode) // Ʃ�丮�� ����� ���
                {
                    if (isReadyToBoil && !isBoiling)  // �� ���� �غ� �Ǿ���, ���� ������ ���� ���
                    {
                        StartCoroutine(BoilWater());
                    }
                    else if (hasBoiledWater && !isPouring)  // ���� ���� �Ŀ� �� ������ ����� ����
                    {
                        StartCoroutine(PourTea());
                    }
                }
                else // ü�� ����� ���
                {
                    if (!isBoiling && !hasBoiledWater)  // ü�� ��忡�� �� ���̱�
                    {
                        StartCoroutine(BoilWater());
                    }
                    else if (hasBoiledWater && !isPouring)  // ü�� ��忡�� �� ������ ���
                    {
                        StartCoroutine(PourTea());
                    }
                }
            }
        }
    }

    // �� ���̱� �ڷ�ƾ
    IEnumerator BoilWater()
    {
        isBoiling = true;
        isReadyToBoil = false;

        int totalTime = isExperienceMode ? 0 : 15;  // ü�� ��忡���� Ÿ�̸� ���� �ٷ� ���̱�

        // ����Ʈ ������ ���� (����Ʈ�� ���� ���̴� ���� ��� ������)
        if (boilingEffectPrefab != null)
        {
            Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            boilingEffectInstance = Instantiate(boilingEffectPrefab, spawnPosition, Quaternion.identity);
            boilingEffectInstance.transform.SetParent(transform);  // Kettle�� ����Ʈ�� ���󰡵��� ����
        }

        // �� ���̴� ���� �ؽ�Ʈ ������Ʈ (ü�� ���� Ÿ�̸� ���� �ٷ� ����)
        while (totalTime > 0)
        {
            if (!isExperienceMode)
            {
                playMinigame.UpdateText($"���� ���̴� ���Դϴ�. 0:{totalTime:D2}");
            }
            yield return new WaitForSeconds(1f);
            totalTime--;
        }

        if (!isExperienceMode)
        {
            playMinigame.UpdateText("���� �ϼ��� �� �����ϴ� ���� ���� Ŭ���ؼ� ���� ���� ���ô�");
        }

        hasBoiledWater = true;
        isBoiling = false;  // �� ���̱� �Ϸ�
    }

    // �� ������ �ڷ�ƾ
    IEnumerator PourTea()
    {
        isPouring = true;  // �� ������ ��

        yield return StartCoroutine(MoveAndRotateKettle());

        if (isExperienceMode)
        {
            ReplaceExperienceObject();  // ü�� ��忡���� ��ü
        }
        else
        {
            ReplaceObject();  // Ʃ�丮�� ��忡���� ��ü
        }

        isPouring = false;
        hasBoiledWater = false;  // �� ������ �Ϸ� �� �ٽ� �� ���̱� ����
    }

    // Kettle �̵� �� ȸ�� �ڷ�ƾ
    public IEnumerator MoveAndRotateKettle()
    {
        yield return StartCoroutine(MoveToPosition(targetPosition.position, 1f));

        // 45�� ȸ��
        yield return StartCoroutine(RotateKettle(45f, 1f));

        // 3�� ���
        yield return new WaitForSeconds(3f);

        if (isExperienceMode)
        {
            ReplaceExperienceObject();  // ü�� ��忡�� ������Ʈ ��ü�� Kettle�� ���ư��� ���� ����
        }

        StartCoroutine(MoveToPosition(originalPosition, 1f));
        StartCoroutine(RotateKettle(0f, 1f, true));

        yield return new WaitForSeconds(1f);
    }

    // Ʃ�丮�� ��忡�� ������Ʈ ��ü �Լ�
    void ReplaceObject()
    {
        if (objectToReplace != null && replacementPrefab != null)
        {
            Vector3 replacePosition = objectToReplace.transform.position;
            Quaternion replaceRotation = objectToReplace.transform.rotation;
            Destroy(objectToReplace);
            Instantiate(replacementPrefab, replacePosition, replaceRotation);
        }
    }

    // ü�� ��忡�� ������Ʈ ��ü �Լ� (��� �ı� �� ��Ÿ��)
    void ReplaceExperienceObject()
    {
        if (experienceObjectToReplace != null && experienceReplacementPrefab != null && experienceTargetPoint != null)
        {
            // ��ü�� ������Ʈ ��� �ı�
            Destroy(experienceObjectToReplace);

            // ���̾��Ű�� �ִ� ��ü�� �������� �ٷ� ������ ��ġ�� �̵�
            experienceReplacementPrefab.transform.position = experienceTargetPoint.position;
        }
    }

    // �ε巴�� ��ġ �̵�
    IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            yield return null;
        }
        transform.position = targetPos;  // ��Ȯ�� ��ġ ����
    }

    // �ε巴�� ȸ���ϴ� �Լ�
    IEnumerator RotateKettle(float angle, float duration, bool toOriginalRotation = false)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (toOriginalRotation)
        {
            endRotation = originalRotation;  // ���� ȸ�������� ����
        }
        else
        {
            endRotation = startRotation * Quaternion.Euler(angle, 0f, 0f);  // X������ �־��� ������ŭ ȸ��
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / duration);
            yield return null;
        }
        transform.rotation = endRotation;  // ��Ȯ�� ȸ���� ����
    }
}