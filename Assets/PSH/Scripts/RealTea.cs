using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTea : MonoBehaviour
{
    public float moveSpeed = 3f;  // �̵� �ӵ�
    private Transform targetPoint;  // ��ǥ ���� (TeaTestPoint)
    private Transform cameraTargetPoint;  // ī�޶� ��ǥ ���� (CameraPoint1)
    public RealGame realGame;  // RealGame ��ũ��Ʈ ���� (���� public���� �����Ͽ� Unity �����Ϳ��� �������� �Ҵ� ����)

    private int susemiCount = 0;  // �����̸� ���� Ƚ��
    private float boilingStartTime;  // ���� ���� ������ �ð�
    private float boilingStopTime;  // �� ���̱⸦ ���� �ð�
    private bool isBoiling = false;  // ���� ���� �ִ��� ����

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

        // realGame�� �������� �Ҵ���� �ʾ��� ���, ������ ���
        if (realGame == null)
        {
            Debug.LogError("RealGame ��ũ��Ʈ�� �Ҵ����ּ���.");
        }

        susemiCount = 0;  // ������ Ƚ�� �ʱ�ȭ
    }

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� ����
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray�� RealTea ������Ʈ�� �¾Ҵ��� Ȯ��
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

    // �����̸� Kettle�� ���� �� ȣ��Ǵ� �Լ�
    public void AddSusemi()
    {
        susemiCount++;
        Debug.Log("������ �߰�: " + susemiCount);
    }

    // Kettle ������Ʈ�� ���� ���̱� ������ �� ȣ��Ǵ� �Լ�
    public void StartBoiling()
    {
        boilingStartTime = Time.time;  // �� ���̱� ���� �ð� ���
        isBoiling = true;
        Debug.Log("���� ���� �����߽��ϴ�.");
    }

    // Kettle ������Ʈ�� �ٽ� Ŭ���ؼ� �� ���̱⸦ ���� �� ȣ��Ǵ� �Լ�
    public void StopBoiling()
    {
        if (isBoiling)
        {
            boilingStopTime = Time.time;  // �� ���̱� �ߴ� �ð� ���
            float boilingDuration = boilingStopTime - boilingStartTime;
            isBoiling = false;
            Debug.Log("���� ���� �ð�: " + boilingDuration + "��");
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

        Debug.Log("RealTea ������Ʈ�� TeaTestPoint�� �����߽��ϴ�.");

        // ���Ŀ� �� ���� ����� �߰��� �� ����
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