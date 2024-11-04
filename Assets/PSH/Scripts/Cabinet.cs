using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    private bool isInInteractZone = false;  // 상호작용 가능 여부
    private GameObject currentCabinet;      // 현재 상호작용 가능한 Cabinet 오브젝트

    void Update()
    {
        // F 키를 누르면 상호작용 시도
        if (isInInteractZone && Input.GetKeyDown(KeyCode.F))
        {
            if (currentCabinet != null)
            {
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
