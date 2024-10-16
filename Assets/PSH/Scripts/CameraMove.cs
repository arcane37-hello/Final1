using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;        // Player Transform
    public Transform cameraPoint;   // Player�� CameraPoint Transform
    public LayerMask collisionMask; // �浹�� ���̾ ���� (�� ���� ��ֹ�)

    private Vector3 offset;         // Camera�� Player ������ �ʱ� �Ÿ�

    void Start()
    {
        // Camera�� Player ������ �ʱ� �Ÿ� ���
        offset = cameraPoint.position - player.position;
    }

    void LateUpdate()
    {
        // �⺻ ī�޶� ��ġ ��� (ī�޶� ����Ʈ ���󰡱�)
        Vector3 desiredPosition = cameraPoint.position;

        // �÷��̾�� ī�޶� ���̿� ��ֹ��� �ִ��� ����
        RaycastHit hit;
        if (Physics.Linecast(player.position, desiredPosition, out hit, collisionMask))
        {
            // ��ֹ��� ������ ī�޶� ��ֹ� �������� �̵�
            transform.position = hit.point;
        }
        else
        {
            // ��ֹ��� ������ CameraPoint�� ��ġ�� �̵�
            transform.position = desiredPosition;
        }

        // ī�޶��� ȸ���� �׻� ī�޶� ����Ʈ�� ȸ���� ����
        transform.rotation = cameraPoint.rotation;
    }
}