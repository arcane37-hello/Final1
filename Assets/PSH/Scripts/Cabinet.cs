using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public GameObject prefabToSpawn;          // 소환할 프리팹
    public Transform spawnPoint;              // 프리팹이 소환될 위치
    private bool isInInteractZone = false;    // 상호작용 가능 여부
    private GameObject currentCabinet;        // 현재 상호작용 가능한 Cabinet 오브젝트
    private Herb herbComponent;               // Herb 스크립트 참조

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
        // Table 태그의 오브젝트와 상호작용 시 스택이 있다면 프리팹을 소환하고 스택 감소
        else if (other.CompareTag("Table") && herbComponent != null && herbComponent.HasStack())
        {
            if (spawnPoint != null)
            {
                Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("프리팹이 " + spawnPoint.position + " 위치에 소환되었습니다.");
                herbComponent.ReduceStack();  // 스택 감소
            }
            else
            {
                Debug.LogWarning("spawnPoint가 설정되지 않았습니다.");
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
