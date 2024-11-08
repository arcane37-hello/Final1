using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Cabinet : MonoBehaviourPunCallbacks
{
    public GameObject teaStackPrefab;          // TeaStack 소모 시 소환될 프리팹
    private Text dialogueText;                 // 텍스트 UI (코드로 지정)
    private bool isInInteractZone = false;     // 상호작용 가능 여부
    private GameObject currentCabinet;         // 현재 상호작용 가능한 Cabinet 오브젝트
    private GameObject currentTeaTest;         // TeaTest 상호작용 오브젝트
    private GameObject currentTable;           // Table 상호작용 오브젝트
    private Herb herbComponent;                // Herb 스크립트 참조
    private int teaStack = 0;                  // TeaStack을 관리할 변수
    private List<Transform> spawnPoints = new List<Transform>();  // 스폰 포인트 목록
    private Transform teaStackSpawnPoint;      // TeaStack 소환 위치

    void Start()
    {
        // TeaTestPoint 오브젝트를 찾아 TeaStackSpawnPoint로 지정
        GameObject teaTestPoint = GameObject.Find("TeaTestPoint");
        if (teaTestPoint != null)
        {
            teaStackSpawnPoint = teaTestPoint.transform;
        }
        else
        {
            Debug.LogWarning("TeaTestPoint를 찾을 수 없습니다. TeaStackPrefab의 소환 위치를 설정하지 못했습니다.");
        }

        // InfoText 오브젝트를 찾아 dialogueText로 지정
        GameObject infoTextObject = GameObject.Find("InfoText");
        if (infoTextObject != null)
        {
            dialogueText = infoTextObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogWarning("InfoText 오브젝트를 찾을 수 없습니다. 텍스트 UI를 설정하지 못했습니다.");
        }

        // 태그가 "SpawnPoint"인 모든 오브젝트를 찾아서 spawnPoints 리스트에 추가
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            spawnPoints.Add(point.transform);
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("SpawnPoint 태그를 가진 오브젝트가 없습니다.");
        }
    }

    void Update()
    {
        if (photonView.IsMine && isInInteractZone && Input.GetKeyDown(KeyCode.F))
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

            if (currentTeaTest != null && teaStack > 0)
            {
                ConsumeTeaStack();
            }

            if (currentTable != null && herbComponent != null && herbComponent.HasStack())
            {
                int stackCount = herbComponent.GetStackCount(); // 스택 개수 확인
                if (spawnPoints.Count > 0 && herbComponent.spawnPrefab != null)
                {
                    for (int i = 0; i < stackCount; i++)
                    {
                        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                        PhotonNetwork.Instantiate(herbComponent.spawnPrefab.name, randomSpawnPoint.position, randomSpawnPoint.rotation);
                    }
                    herbComponent.ClearStack();  // 스택 모두 소모
                    Debug.Log(stackCount + "개의 프리팹이 랜덤 위치에 소환되었습니다.");
                }
                else if (herbComponent.spawnPrefab == null)
                {
                    Debug.LogWarning("소환할 프리팹이 설정되지 않았습니다.");
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cabinet"))
        {
            isInInteractZone = true;
            currentCabinet = other.gameObject;
            Debug.Log(currentCabinet.name + "와 상호작용 가능");
        }
        else if (other.CompareTag("TeaTest"))
        {
            isInInteractZone = true;
            currentTeaTest = other.gameObject;
            Debug.Log("TeaTest 오브젝트와 상호작용 가능");
        }
        else if (other.CompareTag("Table"))
        {
            isInInteractZone = true;
            currentTable = other.gameObject;
            Debug.Log("Table 오브젝트와 상호작용 가능");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cabinet"))
        {
            isInInteractZone = false;
            currentCabinet = null;
            Debug.Log("상호작용 불가");
        }
        else if (other.CompareTag("TeaTest"))
        {
            isInInteractZone = false;
            currentTeaTest = null;
            Debug.Log("TeaTest 오브젝트와의 상호작용 불가");
        }
        else if (other.CompareTag("Table"))
        {
            isInInteractZone = false;
            currentTable = null;
            Debug.Log("Table 오브젝트와의 상호작용 불가");
        }
    }

    public void AddTeaStack()
    {
        teaStack++;
        Debug.Log("현재 TeaStack 개수: " + teaStack);
    }

    private void ConsumeTeaStack()
    {
        // teaStackSpawnPoint와 teaStackPrefab이 Null이 아닐 때만 실행
        if (teaStackSpawnPoint != null && teaStackPrefab != null)
        {
            PhotonNetwork.Instantiate(teaStackPrefab.name, teaStackSpawnPoint.position, teaStackSpawnPoint.rotation);
            teaStack--;  // TeaStack 소모
            Debug.Log("TeaStack 소모됨. 현재 TeaStack: " + teaStack);

            dialogueText.text = " ";  // 텍스트 초기화
        }
        else
        {
            Debug.LogWarning("TeaStack 소모를 위한 프리팹 또는 소환 위치가 설정되지 않았습니다.");
        }
    }

    public int GetTeaStack()
    {
        return teaStack;
    }
}
