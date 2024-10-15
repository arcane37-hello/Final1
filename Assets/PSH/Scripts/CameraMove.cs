using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;        // Player Transform
    public Transform cameraPoint;   // Player�� CameraPoint Transform

    private Vector3 offset;         // Camera�� Player ������ �ʱ� �Ÿ�

    void Start()
    {
        // Camera�� CameraPoint ��ġ�� �����ϰ�, ȸ���� �����ϰ� ����
        transform.position = cameraPoint.position;
        transform.rotation = cameraPoint.rotation;

        // Camera�� Player ������ �ʱ� �Ÿ� ���
        offset = transform.position - player.position;
    }

    void Update()
    {
        // Camera�� ��ġ�� Player�� ��ġ�� offset�� ���� ������ ������Ʈ, ȸ������ ����
        transform.position = player.position + offset;
    }
}