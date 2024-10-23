using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tea1 : MonoBehaviour
{
    public float moveSpeed = 3f;  // 이동 속도
    private Transform targetPoint;  // 목표 지점 (TeaTestPoint)
    private Transform cameraTargetPoint;  // 카메라 목표 지점 (CameraPoint1)
    private PlayMinigame playMinigame; // PlayMinigame 스크립트 참조 (자동으로 찾음)
    private Result result;  // Result 스크립트 참조 (평가 후 호출)

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

        // PlayMinigame 스크립트를 찾아서 설정
        playMinigame = FindObjectOfType<PlayMinigame>();
        if (playMinigame == null)
        {
            Debug.LogError("PlayMinigame 스크립트를 찾을 수 없습니다.");
        }

        // Result 스크립트를 찾아서 설정
        result = FindObjectOfType<Result>();
        if (result == null)
        {
            Debug.LogError("Result 스크립트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray가 Tea1 오브젝트에 맞았는지 확인
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

    // TeaTestPoint로 이동하는 코루틴
    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Tea1 오브젝트가 TeaTestPoint에 도착했습니다.");

        // 이동 완료 후 텍스트 갱신
        if (playMinigame != null)
        {
            playMinigame.UpdateText("평가 중...");

            // 5초 대기 후 텍스트 갱신
            yield return new WaitForSeconds(5f);
            playMinigame.UpdateText("완벽하게 만드셨네요 잘하셨습니다");

            // Result 스크립트의 OnEvaluationComplete() 호출
            if (result != null)
            {
                result.OnEvaluationComplete();  // Result 스크립트로 넘어감
            }
        }
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
}