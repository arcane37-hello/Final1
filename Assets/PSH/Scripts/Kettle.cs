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

            // 찻잎 스택이 5개가 되면 텍스트 갱신 및 물 끓일 준비 완료
            if (teaLeafStack >= 5)
            {
                playMinigame.UpdateText("잘하셨습니다! 이제 차를 만들기 위해 물을 끓여봅시다!");
                isReadyToBoil = true; // 물을 끓일 수 있는 상태로 변경
            }
        }
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 물을 끓이는 기능
        if (isReadyToBoil && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray가 주전자에 맞았는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    Debug.Log("물을 끓이기 시작했습니다.");
                    StartCoroutine(BoilWater());  // 물 끓이기 코루틴 실행
                }
            }
        }

        // 마우스 왼쪽 버튼 클릭 시 Kettle 이동 시작
        if (isReadyToMove && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray가 주전자에 맞았는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    Debug.Log("Kettle 이동 시작");
                    StartCoroutine(MoveAndRotateKettle());  // 이동 및 회전 코루틴 실행
                    isReadyToMove = false;  // 이동 준비 상태 해제
                }
            }
        }
    }

    // 물 끓이기 코루틴
    IEnumerator BoilWater()
    {
        isReadyToBoil = false; // 물 끓이기 동작이 시작되면 다시 끓일 수 없게 설정
        int totalTime = 15;  // 총 15초간 물을 끓임

        // 이펙트 프리팹 생성 (이펙트는 물을 끓이는 동안 계속 유지됨)
        if (boilingEffectPrefab != null)
        {
            // 지정된 위치에 이펙트 생성
            Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            boilingEffectInstance = Instantiate(boilingEffectPrefab, spawnPosition, Quaternion.identity);

            boilingEffectInstance.transform.SetParent(transform);  // Kettle에 이펙트가 따라가도록 설정
        }

        // 타이머가 진행되는 동안 텍스트를 업데이트
        while (totalTime > 0)
        {
            playMinigame.UpdateText($"물을 끓이는 중입니다. 0:{totalTime:D2}");
            yield return new WaitForSeconds(1f);
            totalTime--;
        }

        // 타이머가 끝난 후 텍스트 갱신
        playMinigame.UpdateText("차가 완성된 거 같습니다 이제 컵을 클릭해서 차를 따라 봅시다");

        // 물 끓인 후 이동 준비 상태로 변경 (이펙트는 계속 유지)
        isReadyToMove = true;
    }

    // Kettle 이동 및 회전 코루틴
    IEnumerator MoveAndRotateKettle()
    {
        // Kettle을 지정된 첫 번째 위치로 이동
        yield return StartCoroutine(MoveToPosition(targetPosition.position, 1f));

        // 45도 회전
        yield return StartCoroutine(RotateKettle(45f, 1f));

        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 3초 후 지정된 오브젝트 교체
        ReplaceObject();

        // 텍스트 갱신
        playMinigame.UpdateText("이제 완성된 차를 클릭해서 평가를 받아봅시다");

        // 원래 위치로 이동 및 원래 회전값 복귀를 동시에 진행
        StartCoroutine(MoveToPosition(originalPosition, 1f));
        StartCoroutine(RotateKettle(0f, 1f, true));

        // 두 코루틴이 완료될 때까지 대기
        yield return new WaitForSeconds(1f);
    }

    // 프리팹 교체 함수
    void ReplaceObject()
    {
        // 대상 오브젝트가 존재할 경우에만 교체
        if (objectToReplace != null && replacementPrefab != null)
        {
            Vector3 replacePosition = objectToReplace.transform.position;
            Quaternion replaceRotation = objectToReplace.transform.rotation;

            // 기존 오브젝트 삭제
            Destroy(objectToReplace);

            // 새로운 프리팹을 교체 위치에 생성
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
            // 원래 회전값으로 복귀
            endRotation = originalRotation;
        }
        else
        {
            // X축으로 주어진 각도만큼 회전
            endRotation = startRotation * Quaternion.Euler(angle, 0f, 0f);
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