using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kettle : MonoBehaviour
{
    private int teaLeafStack = 0;  // ���� ����
    public PlayMinigame playMinigame; // PlayMinigame ��ũ��Ʈ ���� (�ؽ�Ʈ ���ſ�)
    private bool isReadyToBoil = false; // ���� ���� �غ� �Ǿ����� ����

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
    }

    // �� ���̱� �ڷ�ƾ
    IEnumerator BoilWater()
    {
        isReadyToBoil = false; // �� ���̱� ������ ���۵Ǹ� �ٽ� ���� �� ���� ����
        yield return new WaitForSeconds(15f);  // 15�� ���
        playMinigame.UpdateText("���� �ϼ��� �� �����ϴ� ���� ���� Ŭ���ؼ� ���� ���� ���ô�");
    }
}