using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTea : MonoBehaviour
{
    public float moveSpeed = 3f;  // 이동 속도
    private Transform targetPoint;  // 목표 지점 (TeaTestPoint)
    private Transform cameraTargetPoint;  // 카메라 목표 지점 (CameraPoint1)
    public RealGame realGame;  // RealGame 스크립트 참조

    private int susemiCount = 0;  // 수세미를 넣은 횟수
    private float boilingStartTime;  // 물이 끓기 시작한 시간
    private float boilingStopTime;  // 물 끓이기를 멈춘 시간
    private bool isBoiling = false;  // 물이 끓고 있는지 여부
    private float boilingDuration;

    void Start()
    {
        GameObject targetObject = GameObject.Find("TeaTestPoint");
        if (targetObject != null)
        {
            targetPoint = targetObject.transform;
        }
        else
        {
            Debug.LogError("TeaTestPoint 오브젝트를 찾을 수 없습니다.");
        }

        GameObject cameraObject = GameObject.Find("CameraPoint1");
        if (cameraObject != null)
        {
            cameraTargetPoint = cameraObject.transform;
        }
        else
        {
            Debug.LogError("CameraPoint1 오브젝트를 찾을 수 없습니다.");
        }

        if (realGame == null)
        {
            Debug.LogError("RealGame 스크립트를 할당해주세요.");
        }

        susemiCount = 0;  // 수세미 횟수 초기화
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    if (targetPoint != null)
                    {
                        StartCoroutine(MoveToTarget());
                    }

                    if (cameraTargetPoint != null)
                    {
                        StartCoroutine(MoveCameraToPoint());
                    }
                }
            }
        }
    }

    public void AddSusemi()
    {
        susemiCount++;
        Debug.Log("수세미 추가: " + susemiCount);
    }

    public void StartBoiling()
    {
        boilingStartTime = Time.time;
        isBoiling = true;
        Debug.Log("물이 끓기 시작했습니다.");
    }

    public void StopBoiling()
    {
        if (isBoiling)
        {
            boilingStopTime = Time.time;
            boilingDuration = boilingStopTime - boilingStartTime;
            isBoiling = false;
            Debug.Log("물이 끓은 시간: " + boilingDuration + "초");
        }
    }

    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("RealTea 오브젝트가 TeaTestPoint에 도착했습니다.");
        StartCoroutine(EvaluateTea());
    }

    IEnumerator MoveCameraToPoint()
    {
        Camera mainCamera = Camera.main;

        while (Vector3.Distance(mainCamera.transform.position, cameraTargetPoint.position) > 0.1f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, cameraTargetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("카메라가 CameraPoint1에 도착했습니다.");
    }

    IEnumerator EvaluateTea()
    {
        if (realGame != null)
        {
            realGame.UpdateText("평가 중...");
            yield return new WaitForSeconds(3f);

            if (susemiCount < 5)
            {
                realGame.UpdateText("재료를 덜 넣으셨네요");
            }
            else if (susemiCount > 5)
            {
                realGame.UpdateText("재료를 너무 많이 넣으셨네요");
            }
            else
            {
                realGame.UpdateText("재료를 적당히 넣으셨네요");
            }

            // EvaluateTea가 끝난 후 3초 대기 후 끓인 시간 평가
            yield return new WaitForSeconds(3f);
            StartCoroutine(DisplayBoilingEvaluation());
        }
        else
        {
            Debug.LogWarning("RealGame 스크립트가 설정되지 않아 텍스트 갱신이 불가능합니다.");
        }
    }

    public void RecordBoilingDuration(float duration)
    {
        boilingDuration = duration;
        Debug.Log("물이 끓인 시간: " + boilingDuration + "초");
    }

    IEnumerator DisplayBoilingEvaluation()
    {
        if (boilingDuration < 15f)
        {
            realGame.UpdateText("차를 덜 우러났네요");
        }
        else if (boilingDuration <= 16f)
        {
            realGame.UpdateText("차를 완벽하게 만드셨네요");
        }
        else
        {
            realGame.UpdateText("차를 너무 오랫동안 끓였네요");
        }
        yield break;
    }
}
