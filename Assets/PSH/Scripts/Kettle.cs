using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kettle : MonoBehaviour
{
    public RealTea realTea;  // RealTea ��ũ��Ʈ ����
    public RealGame realGame; // RealGame ��ũ��Ʈ ���� �߰�
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
    private bool stopBoilingTimer = false;  // Ÿ�̸� ���� ����

    // ü�� ��忡�� ����� ��ü�� ������Ʈ�� ��ü�� �������� ���� ����
    public GameObject experienceObjectToReplace;  // ü�� ��忡�� ��ü�� ������Ʈ
    public GameObject experienceReplacementPrefab;  // ü�� ��忡�� ��ü�� ������ (���̾��Ű�� �ִ� ������Ʈ)
    public Transform experienceTargetPoint;  // ü�� ��忡�� ��ü�� �������� �̵��� ��ǥ ����

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (realTea == null)
        {
            Debug.LogError("RealTea ��ũ��Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
        }

        if (realGame == null)
        {
            Debug.LogError("RealGame ��ũ��Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Susemi"))
        {
            Destroy(collision.gameObject);
            teaLeafStack++;
            Debug.Log("Susemi ������Ʈ �ı���. ���� ���� ����: " + teaLeafStack);

            if (isExperienceMode && realTea != null)
            {
                realTea.AddSusemi();
            }

            if (!isExperienceMode && teaLeafStack >= 5)
            {
                playMinigame.UpdateText("���ϼ̽��ϴ�! ���� ���� ����� ���� ���� �������ô�!");
                isReadyToBoil = true;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
            {
                if (!isExperienceMode)
                {
                    if (isReadyToBoil && !isBoiling)
                    {
                        StartCoroutine(BoilWater());
                    }
                    else if (hasBoiledWater && !isPouring)
                    {
                        StartCoroutine(PourTea());
                    }
                }
                else
                {
                    if (!isBoiling && !hasBoiledWater)
                    {
                        StartCoroutine(BoilWater());
                    }
                    else if (hasBoiledWater && !isPouring)
                    {
                        StartCoroutine(PourTea());
                    }
                }
            }
        }
    }

    IEnumerator BoilWater()
    {
        isBoiling = true;
        isReadyToBoil = false;
        stopBoilingTimer = false;

        int totalTime = isExperienceMode ? 0 : 15;

        if (boilingEffectPrefab != null)
        {
            Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            boilingEffectInstance = Instantiate(boilingEffectPrefab, spawnPosition, Quaternion.identity);
            boilingEffectInstance.transform.SetParent(transform);
        }

        while (totalTime > 0 && !stopBoilingTimer)
        {
            if (!isExperienceMode && realGame != null)
            {
                realGame.UpdateBoilingText($"���� ���̴� ���Դϴ�. 0:{totalTime:D2}");
            }
            yield return new WaitForSeconds(1f);
            totalTime--;
        }

        if (!isExperienceMode && !stopBoilingTimer)
        {
            playMinigame.UpdateText("���� �ϼ��� �� �����ϴ�. �����ڸ� Ŭ���ؼ� ���� ���� ���ô�");
        }

        hasBoiledWater = true;
        isBoiling = false;
    }

    IEnumerator PourTea()
    {
        isPouring = true;
        stopBoilingTimer = true;  // Ÿ�̸� ����

        if (!isExperienceMode)
        {
            realGame.UpdateText("���� ������ ��...");
        }

        yield return StartCoroutine(MoveAndRotateKettle());

        if (isExperienceMode)
        {
            ReplaceExperienceObject();
        }
        else
        {
            ReplaceObject();
            realGame.UpdateText("���� �ϼ��ƽ��ϴ� ������ Ŭ���ؼ� �򰡸� �޾ƺ��ô�");
        }

        if (isExperienceMode && realGame != null)
        {
            realGame.UpdateText("���� ������ ��...");
            yield return new WaitForSeconds(3f);
            realGame.UpdateText("���� �ϼ��ƽ��ϴ� ������ Ŭ���ؼ� �򰡸� �޾ƺ��ô�");
        }

        isPouring = false;
        hasBoiledWater = false;
    }

    public IEnumerator MoveAndRotateKettle()
    {
        yield return StartCoroutine(MoveToPosition(targetPosition.position, 1f));
        yield return StartCoroutine(RotateKettle(45f, 1f));
        yield return new WaitForSeconds(3f);

        if (isExperienceMode)
        {
            ReplaceExperienceObject();
        }

        StartCoroutine(MoveToPosition(originalPosition, 1f));
        StartCoroutine(RotateKettle(0f, 1f, true));
    }

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

    void ReplaceExperienceObject()
    {
        if (experienceObjectToReplace != null && experienceReplacementPrefab != null && experienceTargetPoint != null)
        {
            Destroy(experienceObjectToReplace);
            experienceReplacementPrefab.transform.position = experienceTargetPoint.position;
        }
    }

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
        transform.position = targetPos;
    }

    IEnumerator RotateKettle(float angle, float duration, bool toOriginalRotation = false)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (toOriginalRotation)
        {
            endRotation = originalRotation;
        }
        else
        {
            endRotation = startRotation * Quaternion.Euler(angle, 0f, 0f);
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / duration);
            yield return null;
        }
        transform.rotation = endRotation;
    }
}
