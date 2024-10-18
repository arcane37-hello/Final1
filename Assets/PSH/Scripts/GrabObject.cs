using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    private Camera mainCamera;  // ���� ī�޶� ����
    private GameObject selectedObject;  // ���� ���õ� ������Ʈ
    private bool isObjectGrabbed = false; // ������Ʈ�� ���� �������� ����
    private float objectZDistance; // ī�޶�� ������Ʈ ���� �Ÿ�
    private float fixedYPosition;  // ������Ʈ�� Y ��ǥ�� �����ϱ� ���� ����

    void Start()
    {
        // ���� ī�޶� �ڵ� ����
        mainCamera = Camera.main;

        // ���� ī�޶� ���� ��� ��� �޽��� ���
        if (mainCamera == null)
        {
            Debug.LogError("���� ī�޶� ���� �������� �ʽ��ϴ�. ī�޶� 'Main Camera' �±װ� �ִ��� Ȯ���ϼ���.");
        }
    }

    void Update()
    {
        // ���콺 ���� ��ư�� ������ ��
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            // Ray�� ī�޶󿡼� ���콺 ��ġ�� �߻�
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // ������Ʈ�� Ŭ���Ǿ����� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                // �ش� ������Ʈ�� GrabObject ��ũ��Ʈ�� �ִ��� Ȯ��
                GrabObject grabScript = hit.collider.GetComponent<GrabObject>();

                if (grabScript != null && grabScript == this)  // ���� ��ũ��Ʈ�� ���� ������Ʈ�� �̵� ����
                {
                    // ������Ʈ�� �����ϰ�, ī�޶���� Z�� �Ÿ� ���
                    selectedObject = hit.collider.gameObject;
                    objectZDistance = Vector3.Distance(mainCamera.transform.position, selectedObject.transform.position);
                    fixedYPosition = selectedObject.transform.position.y;  // ������ ������Ʈ�� Y ��ǥ ����
                    isObjectGrabbed = true;
                }
            }
        }

        // ���콺 ���� ��ư�� ������ �ִ� ���� ������Ʈ�� �̵�
        if (Input.GetMouseButton(0) && isObjectGrabbed && selectedObject != null)
        {
            // ���콺 ��ġ�� ���� ������Ʈ�� �̵� (ī�޶� �������� ���� �Ÿ� ����)
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = objectZDistance; // ������Ʈ�� ī�޶� ���� �Ÿ� ����

            // ���콺 ��ġ�� �°� ������Ʈ�� �����̵�, Y ��ǥ�� ����
            Vector3 objectPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            objectPosition.y = fixedYPosition;  // Y ��ǥ�� �����Ͽ� �̵����� �ʵ��� ��
            selectedObject.transform.position = objectPosition;
        }

        // ���콺 ���� ��ư�� ���� �� ������Ʈ�� ��������
        if (Input.GetMouseButtonUp(0) && isObjectGrabbed)
        {
            isObjectGrabbed = false;
            selectedObject = null; // ������Ʈ ���� ����
        }
    }
}