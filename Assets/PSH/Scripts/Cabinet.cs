using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Cabinet : MonoBehaviourPunCallbacks
{
    public GameObject teaStackPrefab;
    public GameObject herb6Prefab; // Herb6 프리팹 참조
    private Text dialogueText;
    private bool isInInteractZone = false;
    private GameObject currentCabinet;
    private GameObject currentTeaTest;
    private GameObject currentTable;
    private Herb herbComponent;
    private int teaStack = 0;
    private List<Transform> spawnPoints = new List<Transform>();
    private Transform teaStackSpawnPoint;
    private CanvasGroup retryButtonCanvasGroup;

    void Start()
    {
        GameObject teaTestPoint = GameObject.Find("TeaTestPoint");
        if (teaTestPoint != null)
        {
            teaStackSpawnPoint = teaTestPoint.transform;
        }
        else
        {
            Debug.LogWarning("TeaTestPoint를 찾을 수 없습니다.");
        }

        GameObject infoTextObject = GameObject.Find("InfoText");
        if (infoTextObject != null)
        {
            dialogueText = infoTextObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogWarning("InfoText 오브젝트를 찾을 수 없습니다.");
        }

        foreach (GameObject point in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            spawnPoints.Add(point.transform);
        }

        GameObject retryButton = GameObject.Find("RetryButton");
        if (retryButton != null)
        {
            retryButtonCanvasGroup = retryButton.GetComponent<CanvasGroup>();
            if (retryButtonCanvasGroup != null)
            {
                retryButtonCanvasGroup.alpha = 0f;
                retryButtonCanvasGroup.interactable = false;
                retryButtonCanvasGroup.blocksRaycasts = false;
            }
            else
            {
                Debug.LogWarning("RetryButton에 CanvasGroup 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("RetryButton 오브젝트를 찾을 수 없습니다.");
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
                    Debug.Log($"{currentCabinet.name}에서 상호작용을 시작합니다.");
                }
            }

            if (currentTeaTest != null && teaStack > 0)
            {
                photonView.RPC("ConsumeTeaStack", RpcTarget.All);
            }

            // Herb6에 대한 독립 처리
            if (currentTable != null && herbComponent != null && herbComponent.HasStack())
            {
                if (herbComponent.stackName == "Herb6")
                {
                    photonView.RPC("SpawnHerb6RPC", RpcTarget.All); // 네트워크 소환 호출
                    herbComponent.ClearStack(); // 중복 소환 방지
                }
                else
                {
                    // 기존 spawnPoints 기반 소환 처리
                    SpawnOtherHerbs();
                }
            }
        }
    }

    private void SpawnOtherHerbs()
    {
        int stackCount = herbComponent.GetStackCount();
        if (spawnPoints.Count > 0 && herbComponent.spawnPrefab != null)
        {
            for (int i = 0; i < stackCount; i++)
            {
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                PhotonNetwork.Instantiate(herbComponent.spawnPrefab.name, randomSpawnPoint.position, randomSpawnPoint.rotation);
            }
            herbComponent.ClearStack();
            Debug.Log($"{stackCount}개의 프리팹이 랜덤 위치에 소환되었습니다.");
        }
        else if (herbComponent.spawnPrefab == null)
        {
            Debug.LogWarning("소환할 프리팹이 설정되지 않았습니다.");
        }
    }

    [PunRPC]
    private void SpawnHerb6RPC()
    {
        GameObject herbCutObject = GameObject.FindWithTag("HerbCut");

        if (herbCutObject != null)
        {
            if (herb6Prefab != null)
            {
                // Herb6 프리팹을 HerbCut 위치에 소환
                Instantiate(herb6Prefab, herbCutObject.transform.position, herbCutObject.transform.rotation);
                Debug.Log($"Herb6 프리팹이 {herbCutObject.name} 위치에 소환되었습니다.");
            }
            else
            {
                Debug.LogWarning("Herb6 프리팹이 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning("태그가 HerbCut인 오브젝트를 찾을 수 없습니다.");
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

    [PunRPC]
    private void ConsumeTeaStack()
    {
        if (teaStackSpawnPoint != null && teaStackPrefab != null)
        {
            PhotonNetwork.Instantiate(teaStackPrefab.name, teaStackSpawnPoint.position, teaStackSpawnPoint.rotation);
            teaStack--;
            Debug.Log("TeaStack 소모됨. 현재 TeaStack: " + teaStack);
            photonView.RPC("UpdateDialogueText", RpcTarget.All, "쌍화차 제작 체험을 완료하셨습니다. 수고하셨습니다.");
            photonView.RPC("ActivateRetryButton", RpcTarget.All); // RetryButton 활성화
        }
        else
        {
            Debug.LogWarning("TeaStack 소모를 위한 프리팹 또는 소환 위치가 설정되지 않았습니다.");
        }
    }

    [PunRPC]
    private void UpdateDialogueText(string message)
    {
        if (dialogueText != null)
        {
            dialogueText.text = message;
        }
    }

    [PunRPC]
    private void ActivateRetryButton()
    {
        if (retryButtonCanvasGroup != null)
        {
            retryButtonCanvasGroup.alpha = 1f;
            retryButtonCanvasGroup.interactable = true;
            retryButtonCanvasGroup.blocksRaycasts = true;
        }
    }

    public int GetTeaStack()
    {
        return teaStack;
    }
}
