using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tea1 : MonoBehaviour
{
    public float moveSpeed = 3f;  // 이동 속도
    private Transform targetPoint;  // 목표 지점 (TeaTestPoint)
    private Transform cameraTargetPoint;  // 카메라 목표 지점 (CameraPoint1)

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