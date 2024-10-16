using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;        // Player Transform
    public Transform cameraPoint;   // Player의 CameraPoint Transform
    public LayerMask collisionMask; // 충돌할 레이어를 설정 (벽 같은 장애물)

    private Vector3 offset;         // Camera와 Player 사이의 초기 거리

    void Start()
    {
        // Camera와 Player 사이의 초기 거리 계산
        offset = cameraPoint.position - player.position;
    }

    void LateUpdate()
    {
        // 기본 카메라 위치 계산 (카메라 포인트 따라가기)
        Vector3 desiredPosition = cameraPoint.position;

        // 플레이어와 카메라 사이에 장애물이 있는지 감지
        RaycastHit hit;
        if (Physics.Linecast(player.position, desiredPosition, out hit, collisionMask))
        {
            // 장애물이 있으면 카메라를 장애물 앞쪽으로 이동
            transform.position = hit.point;
        }
        else
        {
            // 장애물이 없으면 CameraPoint의 위치로 이동
            transform.position = desiredPosition;
        }

        // 카메라의 회전은 항상 카메라 포인트의 회전을 따라감
        transform.rotation = cameraPoint.rotation;
    }
}