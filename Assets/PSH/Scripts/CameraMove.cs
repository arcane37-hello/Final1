using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;        // Player Transform
    public Transform cameraPoint;   // Player의 CameraPoint Transform

    private Vector3 offset;         // Camera와 Player 사이의 초기 거리

    void Start()
    {
        // Camera를 CameraPoint 위치로 설정하고, 회전도 동일하게 맞춤
        transform.position = cameraPoint.position;
        transform.rotation = cameraPoint.rotation;

        // Camera와 Player 사이의 초기 거리 계산
        offset = transform.position - player.position;
    }

    void Update()
    {
        // Camera의 위치를 Player의 위치에 offset을 더한 값으로 업데이트, 회전값은 고정
        transform.position = player.position + offset;
    }
}