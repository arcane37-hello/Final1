using UnityEngine;

public class PlayerMoveNph : MonoBehaviour
{
    public float moveSpeed = 5f;         // 이동 속도
    public float turnSpeed = 100f;       // 회전 속도
    private Rigidbody rb;
    private Animator animator;
    public bool isMovementEnabled = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // Animator가 없을 경우 경고 메시지 출력
        if (animator == null)
        {
            Debug.LogWarning("Animator가 Player의 자식 오브젝트에 추가되지 않았습니다.");
        }
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
}
