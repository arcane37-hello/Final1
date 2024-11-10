using Photon.Pun;
using UnityEngine;

public class GrabObject : MonoBehaviourPun, IPunObservable
{
    private Camera mainCamera;
    private bool isObjectGrabbed = false;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float objectZDistance;
    private float fixedYPosition;

    void Start()
    {
        mainCamera = Camera.main;
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    void Update()
    {
        if (isObjectGrabbed)
        {
            DragObject();
        }
        else
        {
            // 네트워크 상의 위치와 회전으로 보간, 떨림을 줄이기 위해 보간 속도를 조정
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 5);
        }

        // 마우스 버튼을 눌렀을 때 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        // 마우스 버튼을 놓았을 때 드래그 종료
        if (Input.GetMouseButtonUp(0) && isObjectGrabbed)
        {
            EndDrag();
        }
    }

    private void TryStartDrag()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
        {
            // 드래그 상태로 전환
            isObjectGrabbed = true;
            objectZDistance = Vector3.Distance(mainCamera.transform.position, transform.position);
            fixedYPosition = transform.position.y; // Y 좌표 고정
        }
    }

    private void DragObject()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = objectZDistance;

        Vector3 objectPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        objectPosition.y = fixedYPosition;
        transform.position = objectPosition;

        // 드래그 중 위치와 회전 갱신
        photonView.RPC("UpdatePositionAndRotation", RpcTarget.Others, transform.position, transform.rotation);
    }

    private void EndDrag()
    {
        isObjectGrabbed = false;
    }

    [PunRPC]
    private void UpdatePositionAndRotation(Vector3 position, Quaternion rotation)
    {
        networkPosition = position;
        networkRotation = rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 현재 위치와 회전을 네트워크에 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 네트워크에서 위치와 회전을 수신
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
