using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTea : MonoBehaviour
{
    public float moveSpeed = 3f;  // 이동 속도
    private Transform targetPoint;  // 목표 지점 (TeaTestPoint)
    private Transform cameraTargetPoint;  // 카메라 목표 지점 (CameraPoint1)
    public RealGame realGame;  // RealGame 스크립트 참조 (이제 public으로 설정하여 Unity 에디터에서 수동으로 할당 가능)

    private int susemiCount = 0;  // 수세미를 넣은 횟수
    private float boilingStartTime;  // 물이 끓기 시작한 시간
    private float boilingStopTime;  // 물 끓이기를 멈춘 시간
    private bool isBoiling = false;  // 물이 끓고 있는지 여부

    private float boilingDuration;

    void Start()
    {
        // TeaTestPoint 오브젝트를 찾음
        GameObject targetObject = GameObject.Find("TeaTestPoint");
        if (targetObject != null)
        {
            targetPoint = targetObject.transform;
        }
        else
        {
            Debug.LogError("TeaTestPoint 오브젝트를 찾을 수 없습니다.");
        }

        // CameraPoint1 오브젝트를 찾음
        GameObject cameraObject = GameObject.Find("CameraPoint1");
        if (cameraObject != null)
        {
            cameraTargetPoint = cameraObject.transform;
        }
        else
        {
            Debug.LogError("CameraPoint1 오브젝트를 찾을 수 없습니다.");
        }

        // realGame이 수동으로 할당되지 않았을 경우, 오류를 출력
        if (realGame == null)
        {
            Debug.LogError("RealGame 스크립트를 할당해주세요.");
        }

        susemiCount = 0;  // 수세미 횟수 초기화
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray가 RealTea 오브젝트에 맞았는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    // TeaTestPoint로 이동 시작
                    if (targetPoint != null)
                    {
                        StartCoroutine(MoveToTarget());
                    }

                    // 카메라도 CameraPoint1로 이동
                    if (cameraTargetPoint != null)
                    {
                        StartCoroutine(MoveCameraToPoint());
                    }
                }
            }
        }
    }

    // 수세미를 Kettle에 넣을 때 호출되는 함수
    public void AddSusemi()
    {
        susemiCount++;
        Debug.Log("수세미 추가: " + susemiCount);
    }

    // Kettle 오브젝트가 물을 끓이기 시작할 때 호출되는 함수
    public void StartBoiling()
    {
        boilingStartTime = Time.time;  // 물 끓이기 시작 시간 기록
        isBoiling = true;
        Debug.Log("물이 끓기 시작했습니다.");
    }

    // Kettle 오브젝트를 다시 클릭해서 물 끓이기를 멈출 때 호출되는 함수
    public void StopBoiling()
    {
        if (isBoiling)
        {
            boilingStopTime = Time.time;  // 물 끓이기 중단 시간 기록
            float boilingDuration = boilingStopTime - boilingStartTime;
            isBoiling = false;
            Debug.Log("물이 끓은 시간: " + boilingDuration + "초");
        }
    }

    // TeaTestPoint로 이동하는 코루틴
    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("RealTea 오브젝트가 TeaTestPoint에 도착했습니다.");

        // 목표 지점에 도착한 후 평가 실행
        StartCoroutine(EvaluateTea());
    }

    // 카메라를 CameraPoint1로 이동시키는 코루틴
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

    // 수세미 횟수를 평가하는 함수
    IEnumerator EvaluateTea()
    {
        if (realGame != null)
        {
            // "평가 중..." 텍스트를 3초 동안 출력
            realGame.UpdateText("평가 중...");
            yield return new WaitForSeconds(3f);

            // 평가 결과 출력
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
}