using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tea1 : MonoBehaviour
{
    public float moveSpeed = 3f;  // �̵� �ӵ�
    private Transform targetPoint;  // ��ǥ ���� (TeaTestPoint)
    private Transform cameraTargetPoint;  // ī�޶� ��ǥ ���� (CameraPoint1)
    private PlayMinigame playMinigame; // PlayMinigame ��ũ��Ʈ ���� (�ڵ����� ã��)
    private Result result;  // Result ��ũ��Ʈ ���� (�� �� ȣ��)

    void Start()
    {
        // TeaTestPoint ������Ʈ�� ã��
        GameObject targetObject = GameObject.Find("TeaTestPoint");
        if (targetObject != null)
        {
            targetPoint = targetObject.transform;
        }
        else
        {
            Debug.LogError("TeaTestPoint ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // CameraPoint1 ������Ʈ�� ã��
        GameObject cameraObject = GameObject.Find("CameraPoint1");
        if (cameraObject != null)
        {
            cameraTargetPoint = cameraObject.transform;
        }
        else
        {
            Debug.LogError("CameraPoint1 ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // PlayMinigame ��ũ��Ʈ�� ã�Ƽ� ����
        playMinigame = FindObjectOfType<PlayMinigame>();
        if (playMinigame == null)
        {
            Debug.LogError("PlayMinigame ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }

        // Result ��ũ��Ʈ�� ã�Ƽ� ����
        result = FindObjectOfType<Result>();
        if (result == null)
        {
            Debug.LogError("Result ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� ����
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray�� Tea1 ������Ʈ�� �¾Ҵ��� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    // TeaTestPoint�� �̵� ����
                    if (targetPoint != null)
                    {
                        StartCoroutine(MoveToTarget());
                    }

                    // ī�޶� CameraPoint1�� �̵�
                    if (cameraTargetPoint != null)
                    {
                        StartCoroutine(MoveCameraToPoint());
                    }
                }
            }
        }
    }

    // TeaTestPoint�� �̵��ϴ� �ڷ�ƾ
    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Tea1 ������Ʈ�� TeaTestPoint�� �����߽��ϴ�.");

        // �̵� �Ϸ� �� �ؽ�Ʈ ����
        if (playMinigame != null)
        {
            playMinigame.UpdateText("�� ��...");

            // 5�� ��� �� �ؽ�Ʈ ����
            yield return new WaitForSeconds(5f);
            playMinigame.UpdateText("�Ϻ��ϰ� ����̳׿� ���ϼ̽��ϴ�");

            // Result ��ũ��Ʈ�� OnEvaluationComplete() ȣ��
            if (result != null)
            {
                result.OnEvaluationComplete();  // Result ��ũ��Ʈ�� �Ѿ
            }
        }
    }

    // ī�޶� CameraPoint1�� �̵���Ű�� �ڷ�ƾ
    IEnumerator MoveCameraToPoint()
    {
        Camera mainCamera = Camera.main;

        while (Vector3.Distance(mainCamera.transform.position, cameraTargetPoint.position) > 0.1f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, cameraTargetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("ī�޶� CameraPoint1�� �����߽��ϴ�.");
    }
}