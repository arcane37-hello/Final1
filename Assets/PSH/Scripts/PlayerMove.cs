using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;         // 이동 속도
    public float turnSpeed = 100f;       // 회전 속도
    private Rigidbody rb;
    private Animator animator;           // Animator 컴포넌트
    public bool isMovementEnabled = true; // 이동 가능 여부

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 자식 오브젝트에서 Animator 컴포넌트 찾기
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
        if (Input.GetKey(KeyCode.A))
        {
            turn = -1f; // 왼쪽 회전
        }
        if (Input.GetKey(KeyCode.D))
        {
            turn = 1f; // 오른쪽 회전
        }

        // 플레이어 회전 (Y축 회전)
        transform.Rotate(0f, turn * turnSpeed * Time.deltaTime, 0f);

        // 이동 입력 (W, S 키로 이동)
        float move = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            move = 1f; // 전진
            if (animator != null)
            {
                animator.SetBool("isWalking", true); // Walk 애니메이션 시작
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move = -1f; // 후진
            if (animator != null)
            {
                animator.SetBool("isWalking", true); // Walk 애니메이션 시작
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("isWalking", false); // Walk 애니메이션 중지
            }
        }

        // 플레이어 전후 이동 (Z축 방향으로 이동)
        Vector3 moveDirection = transform.forward * move * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }
}