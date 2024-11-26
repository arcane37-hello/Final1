using UnityEngine;
using Photon.Pun;

public class Knife : MonoBehaviourPun, IPunObservable
{
    private Camera mainCamera;
    private bool isObjectGrabbed = false;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float objectZDistance;
    private float fixedYPosition;

    public AudioClip grabSound; // 클릭 시 재생할 사운드
    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        networkPosition = transform.position;
        networkRotation = transform.rotation;

        // AudioSource 설정
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = grabSound;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isObjectGrabbed)
        {
            DragObject();
        }
        else
        {
            // 네트워크 상의 위치와 회전을 보간하여 동기화
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 5);
        }

        // 마우스 클릭으로 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        // 마우스 버튼을 놓으면 드래그 종료
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
            // 상호작용 권한 요청
            if (!photonView.IsMine)
            {
                photonView.RequestOwnership();
            }

            isObjectGrabbed = true;
            objectZDistance = Vector3.Distance(mainCamera.transform.position, transform.position);
            fixedYPosition = transform.position.y;

            // 사운드 재생
            photonView.RPC("PlayGrabSound", RpcTarget.All);
        }
    }

    private void DragObject()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = objectZDistance;

        Vector3 objectPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        objectPosition.y = fixedYPosition; // Y 축 고정
        transform.position = objectPosition;

        // 위치와 회전을 네트워크로 전송
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

    [PunRPC]
    private void PlayGrabSound()
    {
        if (audioSource != null && grabSound != null)
        {
            audioSource.Play();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어의 위치와 회전을 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 원격 플레이어의 위치와 회전을 수신
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
