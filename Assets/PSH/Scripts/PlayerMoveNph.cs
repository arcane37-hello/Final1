using UnityEngine;

public class PlayerMoveNph : MonoBehaviour
{
    public float moveSpeed = 5f;         // 이동 속도
    public float turnSpeed = 100f;       // 회전 속도
    private Rigidbody rb;
    private Animator animator;
    public bool isMovementEnabled = true;
    private bool isCurrentlyMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isMovementEnabled)
        {
            MovePlayer();
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
}
