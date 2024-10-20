using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kettle : MonoBehaviour
{
    private int teaLeafStack = 0;  // 찻잎 스택

    void OnCollisionEnter(Collision collision)
    {
        // 태그가 Susemi인 오브젝트와 충돌했는지 확인
        if (collision.gameObject.CompareTag("Susemi"))
        {
            // Susemi 오브젝트를 파괴
            Destroy(collision.gameObject);

            // 찻잎 스택 증가
            teaLeafStack++;
            Debug.Log("Susemi 오브젝트 파괴됨. 현재 찻잎 스택: " + teaLeafStack);

            // 찻잎 스택이 5개가 되면 무언가 일어남
            if (teaLeafStack >= 5)
            {
                TeaLeafStackFull();
            }
        }
    }

    // 찻잎 스택이 5개가 되었을 때 발생하는 함수 (나중에 기능 추가 예정)
    void TeaLeafStackFull()
    {
        Debug.Log("찻잎 스택이 5개가 되었습니다! 이제 무언가가 일어납니다.");
        // 나중에 이 부분에 원하는 기능을 추가할 수 있음
    }
}