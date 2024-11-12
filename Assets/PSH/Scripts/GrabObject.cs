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

    public AudioClip grabSound; // 클릭 시 재생할 사운드
    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        networkPosition = transform.position;
        networkRotation = transform.rotation;

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
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 5);
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

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
            isObjectGrabbed = true;
            objectZDistance = Vector3.Distance(mainCamera.transform.position, transform.position);
            fixedYPosition = transform.position.y;
            photonView.RPC("PlayGrabSound", RpcTarget.All); // 모든 플레이어에게 사운드 재생
        }
    }

    private void DragObject()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = objectZDistance;

        Vector3 objectPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        objectPosition.y = fixedYPosition;
        transform.position = objectPosition;

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
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
