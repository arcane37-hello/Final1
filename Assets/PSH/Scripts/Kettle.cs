using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kettle : MonoBehaviour
{
    public RealTea realTea;  // RealTea 스크립트 참조
    public RealGame realGame; // RealGame 스크립트 참조 추가
    private int teaLeafStack = 0;  // 찻잎 스택
    public PlayMinigame playMinigame; // PlayMinigame 스크립트 참조 (텍스트 갱신용)
    public Transform targetPosition;  // Kettle이 이동할 첫 번째 빈 오브젝트의 위치
    public GameObject objectToReplace;  // 튜토리얼 모드에서 교체할 대상 오브젝트
    public GameObject replacementPrefab; // 튜토리얼 모드에서 교체될 프리팹
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
    private bool stopBoilingTimer = false;  // 타이머 중지 여부
    private float boilingStartTime; // 체험 모드에서 끓이기 시작 시간
    private float boilingStopTime;  // 체험 모드에서 끓이기 중단 시간

    // 체험 모드에서 사용할 교체할 오브젝트와 교체될 프리팹을 따로 지정
    public GameObject experienceObjectToReplace;  // 체험 모드에서 교체할 오브젝트
    public GameObject experienceReplacementPrefab;  // 체험 모드에서 교체될 프리팹 (하이어라키에 있는 오브젝트)
    public Transform experienceTargetPoint;  // 체험 모드에서 교체될 프리팹이 이동할 목표 지점

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (realTea == null)
        {
            Debug.LogError("RealTea 스크립트가 할당되지 않았습니다.");
        }

        if (realGame == null)
        {
            Debug.LogError("RealGame 스크립트가 할당되지 않았습니다.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Susemi"))
        {
            Destroy(collision.gameObject);
            teaLeafStack++;
            Debug.Log("Susemi 오브젝트 파괴됨. 현재 찻잎 스택: " + teaLeafStack);

            if (isExperienceMode && realTea != null)
            {
                realTea.AddSusemi();
            }

            if (!isExperienceMode && teaLeafStack >= 5)
            {
                playMinigame.UpdateText("잘하셨습니다! 이제 차를 만들기 위해 물을 끓여봅시다!");
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

        if (isExperienceMode)
        {
            boilingStartTime = Time.time; // 체험 모드에서 끓이기 시작 시간 기록
        }

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
                realGame.UpdateBoilingText($"물을 끓이는 중입니다. 0:{totalTime:D2}");
            }
            yield return new WaitForSeconds(1f);
            totalTime--;
        }

        if (!isExperienceMode && !stopBoilingTimer)
        {
            playMinigame.UpdateText("차가 완성된 것 같습니다. 주전자를 클릭해서 차를 따라 봅시다");
        }

        hasBoiledWater = true;
        isBoiling = false;
    }

    IEnumerator PourTea()
    {
        isPouring = true;
        stopBoilingTimer = true;  // 타이머 중지

        if (isExperienceMode)
        {
            boilingStopTime = Time.time;  // 체험 모드에서 끓이기 중단 시간 기록

            if (realTea != null)
            {
                realTea.RecordBoilingDuration(boilingStopTime - boilingStartTime);
            }
        }

        if (realGame != null)
        {
            realGame.StopBoiling();  // BoilWater 코루틴 종료
            realGame.UpdateText("차를 따르는 중...");
        }

        yield return StartCoroutine(MoveAndRotateKettle());

        if (isExperienceMode)
        {
            ReplaceExperienceObject();
        }
        else
        {
            ReplaceObject();
            realGame.UpdateText("차가 완성됐습니다 찻잔을 클릭해서 평가를 받아봅시다");
        }

        if (isExperienceMode && realGame != null)
        {
            realGame.UpdateText("차를 따르는 중...");
            yield return new WaitForSeconds(3f);
            realGame.UpdateText("차가 완성됐습니다 찻잔을 클릭해서 평가를 받아봅시다");
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
        if (!isExperienceMode && objectToReplace != null && replacementPrefab != null)
        {
            Vector3 replacePosition = objectToReplace.transform.position;
            Quaternion replaceRotation = objectToReplace.transform.rotation;
            Destroy(objectToReplace);
            Instantiate(replacementPrefab, replacePosition, replaceRotation);
        }
    }

    void ReplaceExperienceObject()
    {
        if (experienceObjectToReplace != null)
        {
            if (realTea != null && experienceTargetPoint != null)
            {
                realTea.transform.position = experienceTargetPoint.position;  // 체험 모드에서 RealTea 오브젝트 위치 이동
            }

            Destroy(experienceObjectToReplace);  // 특정 오브젝트를 파괴
            Debug.Log("체험 모드에서 RealTea 오브젝트가 목표 지점으로 이동하고 기존 오브젝트가 파괴되었습니다.");
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
        Quaternion endRotation = toOriginalRotation ? originalRotation : startRotation * Quaternion.Euler(angle, 0f, 0f);
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
