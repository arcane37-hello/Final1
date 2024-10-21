using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kettle : MonoBehaviour
{
    private int teaLeafStack = 0;  // ���� ����
    public PlayMinigame playMinigame; // PlayMinigame ��ũ��Ʈ ���� (�ؽ�Ʈ ���ſ�)
    public Transform targetPosition;  // Kettle�� �̵��� ù ��° �� ������Ʈ�� ��ġ
    private Vector3 originalPosition;  // Kettle�� ���� ��ġ
    private Quaternion originalRotation;  // Kettle�� ���� ȸ����
    private bool isReadyToBoil = false;  // ���� ���� �غ� �Ǿ����� ����
    private bool isReadyToMove = false;  // ���� ���� �� �̵� �غ� ����

    void Start()
    {
        // �������� ���� ��ġ�� ȸ������ ����
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        // �±װ� Susemi�� ������Ʈ�� �浹�ߴ��� Ȯ��
        if (collision.gameObject.CompareTag("Susemi"))
        {
            // Susemi ������Ʈ�� �ı�
            Destroy(collision.gameObject);

            // ���� ���� ����
            teaLeafStack++;
            Debug.Log("Susemi ������Ʈ �ı���. ���� ���� ����: " + teaLeafStack);

            // ���� ������ 5���� �Ǹ� �ؽ�Ʈ ���� �� �� ���� �غ� �Ϸ�
            if (teaLeafStack >= 5)
            {
                playMinigame.UpdateText("���ϼ̽��ϴ�! ���� ���� ����� ���� ���� �������ô�!");
                isReadyToBoil = true; // ���� ���� �� �ִ� ���·� ����
            }
        }
    }

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� �� ���� ���̴� ���
        if (isReadyToBoil && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray�� �����ڿ� �¾Ҵ��� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    Debug.Log("���� ���̱� �����߽��ϴ�.");
                    StartCoroutine(BoilWater());  // �� ���̱� �ڷ�ƾ ����
                }
            }
        }

        // ���콺 ���� ��ư Ŭ�� �� Kettle �̵� ����
        if (isReadyToMove && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray�� �����ڿ� �¾Ҵ��� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    Debug.Log("Kettle �̵� ����");
                    StartCoroutine(MoveAndRotateKettle());  // �̵� �� ȸ�� �ڷ�ƾ ����
                    isReadyToMove = false;  // �̵� �غ� ���� ����
                }
            }
        }
    }

    // �� ���̱� �ڷ�ƾ
    IEnumerator BoilWater()
    {
        isReadyToBoil = false; // �� ���̱� ������ ���۵Ǹ� �ٽ� ���� �� ���� ����
        yield return new WaitForSeconds(15f);  // 15�� ���
        playMinigame.UpdateText("���� �ϼ��� �� �����ϴ� ���� �����ڸ� Ŭ���ؼ� ���� ���� ���ô�");

        // �� ���� �� �̵� �غ� ���·� ����
        isReadyToMove = true;
    }

    // Kettle �̵� �� ȸ�� �ڷ�ƾ
    IEnumerator MoveAndRotateKettle()
    {
        // Kettle�� ������ ù ��° ��ġ�� �̵�
        yield return StartCoroutine(MoveToPosition(targetPosition.position, 1f));

        // 45�� ȸ��
        yield return StartCoroutine(RotateKettle(45f, 1f));

        // 3�� ���
        yield return new WaitForSeconds(3f);

        // ���� ��ġ�� �̵� �� ���� ȸ���� ���͸� ���ÿ� ����
        StartCoroutine(MoveToPosition(originalPosition, 1f));
        StartCoroutine(RotateKettle(0f, 1f, true));

        // �� �ڷ�ƾ�� �Ϸ�� ������ ���
        yield return new WaitForSeconds(1f);
    }

    // �ε巴�� ��ġ �̵�
    IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            yield return null;
        }
        transform.position = targetPos;  // ��Ȯ�� ��ġ ����
    }

    // �ε巴�� ȸ���ϴ� �Լ�
    IEnumerator RotateKettle(float angle, float duration, bool toOriginalRotation = false)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (toOriginalRotation)
        {
            // ���� ȸ�������� ����
            endRotation = originalRotation;
        }
        else
        {
            // X������ �־��� ������ŭ ȸ��
            endRotation = startRotation * Quaternion.Euler(angle, 0f, 0f);
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / duration);
            yield return null;
        }
        transform.rotation = endRotation;  // ��Ȯ�� ȸ���� ����
    }
}