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
    private bool isCurrentlyMoving = false;
    private bool isMovingNetwork;        // 네트워크를 통해 수신된 애니메이션 상태

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (!photonView.IsMine)
        {
            rb.isKinematic = true; // 로컬 플레이어가 아닐 경우 물리 효과를 막기 위해 kinematic 설정
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

            // 네트워크에서 받은 애니메이션 상태 적용
            if (animator != null)
            {
                UpdateAnimation(isMovingNetwork);
            }
        }
    }

    void MovePlayer()
    {
        float move = 0f;
        bool isWalking = false;

        if (Input.GetKey(KeyCode.W))
        {
            move = 1f;
            isWalking = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move = -1f;
            isWalking = true;
        }

        UpdateAnimation(isWalking);

        // 회전 입력 (A, D 키로 회전)
        float turn = 0f;
        if (Input.GetKey(KeyCode.A)) turn = -1f;
        if (Input.GetKey(KeyCode.D)) turn = 1f;
        transform.Rotate(0f, turn * turnSpeed * Time.deltaTime, 0f);

        // 이동
        Vector3 moveDirection = transform.forward * move * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    void UpdateAnimation(bool isWalking)
    {
        if (animator != null)
        {
            if (isWalking && !isCurrentlyMoving)
            {
                animator.Play("Korean_Male_Walk");  // 걷기 애니메이션 실행
                isCurrentlyMoving = true;
            }
            else if (!isWalking && isCurrentlyMoving)
            {
                animator.Play("Korean_Male_Stand"); // 서 있는 애니메이션으로 전환
                isCurrentlyMoving = false;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어 데이터 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(isCurrentlyMoving); // 애니메이션 상태 전송
        }
        else
        {
            // 원격 플레이어의 데이터 수신
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            isMovingNetwork = (bool)stream.ReceiveNext(); // 원격 플레이어의 애니메이션 상태 수신
        }
    }
}
