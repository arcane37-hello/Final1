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

    void Update()
    {
        if (canInteract && Input.GetMouseButtonDown(0) && !isBoiling && !isPouring)
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

        if (herbStack >= 2 && herb2Stack >= 2 && herb3Stack >= 2 && herb4Stack >= 2 && herb5Stack >= 2)
        {
            canInteract = true;
            dialogueText.text = "이제 주전자를 클릭해서 물을 끓여봅시다.";
        }
    }

    // 물 끓이는 상호작용 코루틴
    private IEnumerator StartBoiling()
    {
        isBoiling = true;
        canInteract = false; // 타이머 동안 상호작용 비활성화
        int timer = 15;

        // 이펙트 생성
        if (boilingEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effectInstance = Instantiate(boilingEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            effectInstance.transform.SetParent(transform);
        }

        // 타이머 갱신
        while (timer > 0)
        {
            dialogueText.text = "물이 끓는 중입니다. 0:" + timer.ToString("D2");
            yield return new WaitForSeconds(1f);
            timer--;
        }

        // 물 끓이기 완료 시 텍스트 갱신 및 상호작용 가능
        dialogueText.text = "차가 완성된 것 같군요. 이제 주전자를 클릭해서 차를 완성합시다!";
        boilingComplete = true; // 물 끓이기 완료 상태로 전환
        canInteract = true; // 타이머 종료 후 상호작용 재활성화
        isBoiling = false;
    }

    // 차 따르기 동작 코루틴
    private IEnumerator PourTea()
    {
        isPouring = true;
        canInteract = false;

        // 주전자를 목표 위치로 이동 및 회전
        yield return StartCoroutine(MoveAndRotateKettle(targetPosition.position, targetPosition.rotation));

        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 오브젝트 교체
        ReplaceObject();

        // 원래 위치로 돌아오기
        yield return StartCoroutine(MoveAndRotateKettle(originalPosition, originalRotation));

        isPouring = false;
        canInteract = true; // 동작이 끝나면 상호작용 가능
    }

    // 주전자를 특정 위치로 이동 및 회전시키는 코루틴
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

    // 특정 오브젝트를 파괴하고 동일한 위치에 프리팹 소환
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
