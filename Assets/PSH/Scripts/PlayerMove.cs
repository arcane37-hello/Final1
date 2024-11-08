using Photon.Pun;
using UnityEngine;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    public float moveSpeed = 5f;         // 이동 속도
    public float turnSpeed = 100f;       // 회전 속도
    private Rigidbody rb;
    private Animator animator;
    public bool isMovementEnabled = true;

    private Vector3 networkPosition;     // 네트워크 상의 위치
    private Quaternion networkRotation;  // 네트워크 상의 회전

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (!photonView.IsMine)
        {
            // 로컬 플레이어가 아닌 경우 입력을 비활성화
            rb.isKinematic = true; // 물리 효과를 막기 위해 kinematic 설정
        }
    }

    void Update()
    {
        if (photonView.IsMine && isMovementEnabled)
        {
            MovePlayer();
        }
        else
        {
            // 네트워크 플레이어의 위치와 회전 동기화
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
        }
    }

    void MovePlayer()
    {
        // 회전 입력 (A, D 키로 회전)
        float turn = 0f;
        if (Input.GetKey(KeyCode.A)) turn = -1f;
        if (Input.GetKey(KeyCode.D)) turn = 1f;
        transform.Rotate(0f, turn * turnSpeed * Time.deltaTime, 0f);

        // 이동 입력 (W, S 키로 이동)
        float move = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            move = 1f;
            if (animator != null) animator.SetBool("isWalking", true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move = -1f;
            if (animator != null) animator.SetBool("isWalking", true);
        }
        else if (animator != null) animator.SetBool("isWalking", false);

        // 이동
        Vector3 moveDirection = transform.forward * move * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어 데이터 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 원격 플레이어의 데이터 수신
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
