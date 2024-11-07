using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KettleSanghwa : MonoBehaviour
{
    public Transform effectSpawnPoint;       // 이펙트가 생성될 위치
    public GameObject boilingEffectPrefab;   // 물 끓일 때 출력할 이펙트 프리팹
    public Text dialogueText;                // 텍스트 UI
    public Transform targetPosition;         // 주전자가 이동할 위치
    public GameObject objectToReplace;       // 파괴될 오브젝트
    public GameObject replacementPrefab;     // 소환될 프리팹

    private Vector3 originalPosition;        // 주전자의 원래 위치
    private Quaternion originalRotation;     // 주전자의 원래 회전값
    private int herbStack = 0;
    private int herb2Stack = 0;
    private int herb3Stack = 0;
    private int herb4Stack = 0;
    private int herb5Stack = 0;
    private bool canInteract = false;        // 상호작용 가능 여부
    private bool isBoiling = false;          // 물 끓이는 중 여부
    private bool isPouring = false;          // 차 따르기 모션 여부
    private bool boilingComplete = false;    // 물 끓이기 완료 여부

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnMouseDown()
    {
        if (canInteract && !isBoiling && !isPouring)
        {
            if (!boilingComplete)
            {
                StartCoroutine(StartBoiling());
            }
            else
            {
                StartCoroutine(PourTea());
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Herb"))
        {
            herbStack++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Herb2"))
        {
            herb2Stack++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Herb3"))
        {
            herb3Stack++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Herb4"))
        {
            herb4Stack++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Herb5"))
        {
            herb5Stack++;
            Destroy(collision.gameObject);
        }

        // 필요한 모든 재료가 2개 이상일 때만 상호작용 가능 상태로 전환하며 한 번만 갱신
        if (!canInteract && herbStack >= 2 && herb2Stack >= 2 && herb3Stack >= 2 && herb4Stack >= 2 && herb5Stack >= 2)
        {
            canInteract = true;
            dialogueText.text = "이제 주전자를 클릭해서 물을 끓여봅시다.";
        }
    }

    private IEnumerator StartBoiling()
    {
        isBoiling = true;
        canInteract = false;
        int timer = 15;

        if (boilingEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effectInstance = Instantiate(boilingEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            effectInstance.transform.SetParent(transform);
        }

        while (timer > 0)
        {
            dialogueText.text = "물이 끓는 중입니다. 0:" + timer.ToString("D2");
            yield return new WaitForSeconds(1f);
            timer--;
        }

        dialogueText.text = "차가 완성된 것 같군요. 이제 주전자를 클릭해서 차를 완성합시다!";
        boilingComplete = true;
        canInteract = true;
        isBoiling = false;
    }

    private IEnumerator PourTea()
    {
        isPouring = true;
        canInteract = false;

        yield return StartCoroutine(MoveAndRotateKettle(targetPosition.position, targetPosition.rotation));

        yield return new WaitForSeconds(3f);

        ReplaceObject();

        // 오브젝트 교체 후 텍스트 갱신
        dialogueText.text = "차가 완성됐습니다. 찻잔을 클릭해서 평가를 받아봅시다.";
        canInteract = true;

        yield return StartCoroutine(MoveAndRotateKettle(originalPosition, originalRotation));

        isPouring = false;
        
    }

    private IEnumerator MoveAndRotateKettle(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPos, elapsedTime / duration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private void ReplaceObject()
    {
        if (objectToReplace != null && replacementPrefab != null)
        {
            Vector3 replacePosition = objectToReplace.transform.position;
            Quaternion replaceRotation = objectToReplace.transform.rotation;

            Destroy(objectToReplace);
            Instantiate(replacementPrefab, replacePosition, replaceRotation);
        }
    }
}
