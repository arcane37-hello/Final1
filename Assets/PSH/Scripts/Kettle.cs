using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kettle : MonoBehaviour
{
    private int teaLeafStack = 0;  // ���� ����

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

            // ���� ������ 5���� �Ǹ� ���� �Ͼ
            if (teaLeafStack >= 5)
            {
                TeaLeafStackFull();
            }
        }
    }

    // ���� ������ 5���� �Ǿ��� �� �߻��ϴ� �Լ� (���߿� ��� �߰� ����)
    void TeaLeafStackFull()
    {
        Debug.Log("���� ������ 5���� �Ǿ����ϴ�! ���� ���𰡰� �Ͼ�ϴ�.");
        // ���߿� �� �κп� ���ϴ� ����� �߰��� �� ����
    }
}