using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public GameObject[] spawnPoints;           // 프리팹이 랜덤으로 소환될 위치들
    private bool isInInteractZone = false;     // 상호작용 가능 여부
    private GameObject currentCabinet;         // 현재 상호작용 가능한 Cabinet 오브젝트
    private Herb herbComponent;                // Herb 스크립트 참조

    void Update()
    {
        // F 키를 누르면 상호작용 시도
        if (isInInteractZone && Input.GetKeyDown(KeyCode.F))
        {
            if (currentCabinet != null)
            {
                herbComponent = currentCabinet.GetComponent<Herb>();
                if (herbComponent != null)
                {
                    herbComponent.Interact();
                }
                Debug.Log(currentCabinet.name + "에서 상호작용을 시작합니다.");
            }
        }
    }

    // Cabinet 태그의 오브젝트와 충돌했을 때 상호작용 가능
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cabinet"))
        {
            isInInteractZone = true;
            currentCabinet = other.gameObject;
            Debug.Log(currentCabinet.name + "와 상호작용 가능");
        }
        // Table 태그의 오브젝트와 상호작용 시 모든 스택을 소모하고 프리팹을 랜덤 위치에 소환
        else if (other.CompareTag("Table") && herbComponent != null && herbComponent.HasStack())
        {
            int stackCount = herbComponent.GetStackCount(); // 스택 개수 확인
            if (spawnPoints.Length > 0 && herbComponent.spawnPrefab != null)
            {
                for (int i = 0; i < stackCount; i++)
                {
                    // 랜덤한 스폰 위치 선택
                    Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
                    Instantiate(herbComponent.spawnPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
                }
                herbComponent.ClearStack();  // 스택 모두 소모
                Debug.Log(stackCount + "개의 프리팹이 랜덤 위치에 소환되었습니다.");
            }
            else if (herbComponent.spawnPrefab == null)
            {
                Debug.LogWarning("소환할 프리팹이 설정되지 않았습니다.");
            }
            else
            {
                Debug.LogWarning("spawnPoints 배열에 위치가 설정되지 않았습니다.");
            }
        }
    }

    // Cabinet 태그의 오브젝트와 충돌이 끝났을 때 상호작용 불가능
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cabinet"))
        {
            isInInteractZone = false;
            currentCabinet = null;
            Debug.Log("상호작용 불가");
        }
    }
}
