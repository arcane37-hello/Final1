using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kettle : MonoBehaviour
{
    private int teaLeafStack = 0;  // 찻잎 스택
    public PlayMinigame playMinigame; // PlayMinigame 스크립트 참조 (텍스트 갱신용)
    private bool isReadyToBoil = false; // 물을 끓일 준비가 되었는지 여부

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

            // 찻잎 스택이 5개가 되면 텍스트 갱신 및 물 끓일 준비 완료
            if (teaLeafStack >= 5)
            {
                playMinigame.UpdateText("잘하셨습니다! 이제 차를 만들기 위해 물을 끓여봅시다!");
                isReadyToBoil = true; // 물을 끓일 수 있는 상태로 변경
            }
        }
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 물을 끓이는 기능
        if (isReadyToBoil && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray가 주전자에 맞았는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    Debug.Log("물을 끓이기 시작했습니다.");
                    StartCoroutine(BoilWater());  // 물 끓이기 코루틴 실행
                }
            }
        }
    }

    // 물 끓이기 코루틴
    IEnumerator BoilWater()
    {
        isReadyToBoil = false; // 물 끓이기 동작이 시작되면 다시 끓일 수 없게 설정
        yield return new WaitForSeconds(15f);  // 15초 대기
        playMinigame.UpdateText("차가 완성된 거 같습니다 이제 컵을 클릭해서 차를 따라 봅시다");
    }
}