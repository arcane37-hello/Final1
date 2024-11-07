using Photon.Pun;
using UnityEngine;

public class CameraMove : MonoBehaviourPun
{
    private Transform player;            // 로컬 플레이어의 Transform
    public Transform cameraPoint;        // 로컬 플레이어의 카메라 포인트 Transform
    public LayerMask collisionMask;      // 충돌할 레이어 (장애물 등)
    private bool isInitialized = false;  // 카메라 초기화 상태 확인

    private Vector3 offset;              // 카메라와 플레이어 사이의 초기 거리

    void Update()
    {
        if (!isInitialized)
        {
            InitializeCamera();
        }
    }

    private void InitializeCamera()
    {
        // 로컬 플레이어 확인 및 카메라 할당
        var localPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var localPlayer in localPlayers)
        {
            var photonView = localPlayer.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                player = localPlayer.transform;
                cameraPoint = player.Find("CameraPoint");  // CameraPoint가 플레이어 하위에 있다고 가정

                // 카메라 설정
                Camera.main.transform.SetParent(cameraPoint);
                Camera.main.transform.localPosition = Vector3.zero;
                Camera.main.transform.localRotation = Quaternion.identity;

                isInitialized = true;  // 초기화 완료
                break;
            }
        }
    }

    void LateUpdate()
    {
        if (!isInitialized) return;

        // 카메라 위치 계산
        Vector3 desiredPosition = cameraPoint.position;

        // 플레이어와 카메라 사이에 장애물이 있는지 감지
        RaycastHit hit;
        if (Physics.Linecast(player.position, desiredPosition, out hit, collisionMask))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = desiredPosition;
        }

        // 카메라의 회전은 카메라 포인트의 회전을 따름
        transform.rotation = cameraPoint.rotation;
    }
}
