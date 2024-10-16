using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;         // �̵� �ӵ�
    public float turnSpeed = 100f;       // ȸ�� �ӵ�
    private Rigidbody rb;
    private Animator animator;           // Animator ������Ʈ

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // �ڽ� ������Ʈ���� Animator ������Ʈ ã��
        animator = GetComponentInChildren<Animator>();

        // Animator�� ���� ��� ��� �޽��� ���
        if (animator == null)
        {
            Debug.LogWarning("Animator�� Player�� �ڽ� ������Ʈ�� �߰����� �ʾҽ��ϴ�.");
        }
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // ȸ�� �Է� (A, D Ű�� ȸ��)
        float turn = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            turn = -1f; // ���� ȸ��
        }
        if (Input.GetKey(KeyCode.D))
        {
            turn = 1f; // ������ ȸ��
        }

        // �÷��̾� ȸ�� (Y�� ȸ��)
        transform.Rotate(0f, turn * turnSpeed * Time.deltaTime, 0f);

        // �̵� �Է� (W, S Ű�� �̵�)
        float move = 0f;

        // W Ű�� ������ �� ���� �� �ִϸ��̼� ���
        if (Input.GetKey(KeyCode.W))
        {
            move = 1f; // ����
            if (animator != null)
            {
                animator.SetBool("isWalking", true); // Walk �ִϸ��̼� ����
                animator.speed = 1f; // �ִϸ��̼� ������ ���
            }
        }
        // S Ű�� ������ �� ���� �� �ִϸ��̼� ���� ���
        else if (Input.GetKey(KeyCode.S))
        {
            move = -1f; // ����
            if (animator != null)
            {
                animator.SetBool("isWalking", true); // Walk �ִϸ��̼� ����
                animator.speed = 1f; // �ִϸ��̼� ������ ���
            }
        }
        else
        {
            // W�� S Ű�� ���� Idle ���·� ���ư�
            if (animator != null)
            {
                animator.SetBool("isWalking", false); // Walk �ִϸ��̼� ����
                animator.speed = 1f; // �ִϸ��̼� �ӵ� �ʱ�ȭ
            }
        }

        // �÷��̾� ���� �̵� (Z�� �������� �̵�)
        Vector3 moveDirection = transform.forward * move * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }
}