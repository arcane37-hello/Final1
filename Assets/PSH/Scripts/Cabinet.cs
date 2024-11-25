using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Cabinet : MonoBehaviourPunCallbacks
{
    public GameObject teaStackPrefab;
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

    // FIcon 이미지 관련 변수
    private CanvasGroup iconCanvasGroup;

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

        // FIcon 이미지 CanvasGroup 초기화
        GameObject iconObject = GameObject.Find("FIcon");
        if (iconObject != null)
        {
            iconCanvasGroup = iconObject.GetComponent<CanvasGroup>();
            if (iconCanvasGroup != null)
            {
                iconCanvasGroup.alpha = 0f; // 초기 알파값 0
            }
            else
            {
                Debug.LogWarning("FIcon에 CanvasGroup 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("FIcon 오브젝트를 찾을 수 없습니다.");
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

            if (currentTable != null && herbComponent != null && herbComponent.HasStack())
            {
                if (herbComponent.stackName == "Herb6")
                {
                    SpawnHerb6Directly();
                    herbComponent.ClearStack();
                }
                else
                {
                    SpawnOtherHerbs();
                }
            }
        }

        // FIcon 이미지 알파값 업데이트
        if (iconCanvasGroup != null)
        {
            iconCanvasGroup.alpha = isInInteractZone ? 1f : 0f;
        }
    }

    private void SpawnHerb6Directly()
    {
        GameObject herb6Prefab = Resources.Load<GameObject>("Prefabs/Herb6");

        if (herb6Prefab == null)
        {
            Debug.LogError("Herb6 프리팹을 Resources 폴더에서 찾을 수 없습니다.");
            return;
        }

        GameObject herbCutObject = GameObject.FindWithTag("HerbCut");

        if (herbCutObject != null)
        {
            GameObject spawnedHerb6 = Instantiate(herb6Prefab, herbCutObject.transform.position, herbCutObject.transform.rotation);

            spawnedHerb6.tag = "Herb6";
            Debug.Log($"소환된 Herb6의 최종 태그: {spawnedHerb6.tag}, 위치: {spawnedHerb6.transform.position}");
        }
        else
        {
            Debug.LogWarning("태그가 HerbCut인 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void SpawnOtherHerbs()
    {
        Debug.Log("SpawnOtherHerbs 호출됨");
        int stackCount = herbComponent.GetStackCount();
        if (spawnPoints.Count > 0 && herbComponent.spawnPrefab != null)
        {
            for (int i = 0; i < stackCount; i++)
            {
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                PhotonNetwork.Instantiate(herbComponent.spawnPrefab.name, randomSpawnPoint.position, randomSpawnPoint.rotation);
                Debug.Log($"Herb6이 아닌 재료가 {randomSpawnPoint.position} 위치에서 소환됨");
            }
            herbComponent.ClearStack();
        }
        else if (herbComponent.spawnPrefab == null)
        {
            Debug.LogWarning("소환할 프리팹이 설정되지 않았습니다.");
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
            photonView.RPC("ActivateRetryButton", RpcTarget.All);
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
