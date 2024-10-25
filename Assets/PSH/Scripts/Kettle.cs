using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kettle : MonoBehaviour
{
    private int teaLeafStack = 0;  // 찻잎 스택
    public PlayMinigame playMinigame; // PlayMinigame 스크립트 참조 (텍스트 갱신용)
    public Transform targetPosition;  // Kettle이 이동할 첫 번째 빈 오브젝트의 위치
    public GameObject objectToReplace;  // 교체할 대상 오브젝트
    public GameObject replacementPrefab; // 교체될 프리팹
    public GameObject boilingEffectPrefab; // 물 끓일 때 출력할 이펙트 프리팹
    public Transform effectSpawnPoint;  // 이펙트가 생성될 위치
    private GameObject boilingEffectInstance;  // 생성된 이펙트 인스턴스
    private Vector3 originalPosition;  // Kettle의 원래 위치
    private Quaternion originalRotation;  // Kettle의 원래 회전값
    private bool isReadyToBoil = false;  // 물을 끓일 준비가 되었는지 여부
    private bool isReadyToMove = false;  // 물을 끓인 후 이동 준비 상태
    public bool isExperienceMode = false;  // 체험 모드 여부 확인
    private bool hasBoiledWater = false;  // 물을 끓였는지 여부 확인
    private bool isPouring = false;  // 차 따르기 모션 여부 확인
    private bool isBoiling = false;  // 물 끓이기 중 여부

    void Start()
    {
        // 주전자의 원래 위치와 회전값을 저장
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 태그가 Susemi인 오브젝트와 충돌했는지 확인
        if (collision.gameObject.CompareTag("Susemi"))
        {
            // Susemi 오브젝트를 파괴
            Destroy(collision.gameObject);

            // 찻잎 스택 증가
            teaLeafStack++;
            Debug.Log("Susemi 오브젝트 파괴됨. 현재 찻잎 스택: " + teaLeafStack);

            // 체험 모드가 아닐 때만 텍스트 갱신
            if (!isExperienceMode && teaLeafStack >= 5)
            {
                playMinigame.UpdateText("잘하셨습니다! 이제 차를 만들기 위해 물을 끓여봅시다!");
                isReadyToBoil = true; // 물을 끓일 수 있는 상태로 변경
            }
        }
    }

    void Update()
    {
        // 튜토리얼 모드와 체험 모드에 따라 물 끓이기 및 차 따르기 상호작용 구분
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray가 주전자에 맞았는지 확인
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
            {
                if (!isExperienceMode) // 튜토리얼 모드일 경우
                {
                    if (isReadyToBoil && !isBoiling)  // 물 끓일 준비가 되었고, 물을 끓이지 않은 경우
                    {
                        StartCoroutine(BoilWater());
                    }
                    else if (hasBoiledWater && !isPouring)  // 물을 끓인 후에 차 따르기 모션을 수행
                    {
                        StartCoroutine(PourTea());
                    }
                }
                else // 체험 모드일 경우
                {
                    if (!isBoiling && !hasBoiledWater)  // 체험 모드에서 물 끓이기
                    {
                        StartCoroutine(BoilWater());
                    }
                    else if (hasBoiledWater && !isPouring)  // 체험 모드에서 차 따르기 모션
                    {
                        StartCoroutine(PourTea());
                    }
                }
            }
        }
    }

    // 물 끓이기 코루틴
    IEnumerator BoilWater()
    {
        isBoiling = true;
        isReadyToBoil = false;

        int totalTime = isExperienceMode ? 0 : 15;  // 체험 모드에서는 타이머 없이 바로 끓이기

        // 이펙트 프리팹 생성 (이펙트는 물을 끓이는 동안 계속 유지됨)
        if (boilingEffectPrefab != null)
        {
            Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            boilingEffectInstance = Instantiate(boilingEffectPrefab, spawnPosition, Quaternion.identity);
            boilingEffectInstance.transform.SetParent(transform);  // Kettle에 이펙트가 따라가도록 설정
        }

        // 물 끓이는 동안 텍스트 업데이트 (체험 모드는 타이머 없이 바로 진행)
        while (totalTime > 0)
        {
            if (!isExperienceMode)
            {
                playMinigame.UpdateText($"물을 끓이는 중입니다. 0:{totalTime:D2}");
            }
            yield return new WaitForSeconds(1f);
            totalTime--;
        }

        if (!isExperienceMode)
        {
            playMinigame.UpdateText("차가 완성된 거 같습니다 이제 컵을 클릭해서 차를 따라 봅시다");
        }

        hasBoiledWater = true;
        isBoiling = false;  // 물 끓이기 완료
    }

    // 차 따르기 코루틴
    IEnumerator PourTea()
    {
        isPouring = true;  // 차 따르기 중

        yield return StartCoroutine(MoveAndRotateKettle());

        ReplaceObject();  // 대상 오브젝트 교체

        isPouring = false;
        hasBoiledWater = false;  // 차 따르기 완료 후 다시 물 끓이기 가능
    }

    // Kettle 이동 및 회전 코루틴
    public IEnumerator MoveAndRotateKettle()
    {
        yield return StartCoroutine(MoveToPosition(targetPosition.position, 1f));

        // 45도 회전
        yield return StartCoroutine(RotateKettle(45f, 1f));

        // 3초 대기
        yield return new WaitForSeconds(3f);

        StartCoroutine(MoveToPosition(originalPosition, 1f));
        StartCoroutine(RotateKettle(0f, 1f, true));

        yield return new WaitForSeconds(1f);
    }

    // 프리팹 교체 함수
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

    // 부드럽게 위치 이동
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
        transform.position = targetPos;  // 정확한 위치 보정
    }

    // 부드럽게 회전하는 함수
    IEnumerator RotateKettle(float angle, float duration, bool toOriginalRotation = false)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (toOriginalRotation)
        {
            endRotation = originalRotation;  // 원래 회전값으로 복귀
        }
        else
        {
            endRotation = startRotation * Quaternion.Euler(angle, 0f, 0f);  // X축으로 주어진 각도만큼 회전
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / duration);
            yield return null;
        }
        transform.rotation = endRotation;  // 정확한 회전값 보정
    }
}