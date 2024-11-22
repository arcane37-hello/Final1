using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Minigame2 : MonoBehaviourPunCallbacks
{
    public Text dialogueText;
    public Transform cameraTargetPoint;
    public GameObject player;
    private Camera mainCamera;
    public CameraMove cameraMoveScript;
    public PlayerMove playerMoveScript;
    public CanvasGroup recipeCanvasGroup;
    private float checkInterval = 1f;
    private float timer = 0f;
    private bool textUpdateStopped = false;
    private bool isPlayerCountReady = false;

    void Start()
    {
        mainCamera = Camera.main;
        cameraMoveScript = mainCamera.GetComponent<CameraMove>();
        playerMoveScript = player.GetComponent<PlayerMove>();

        if (recipeCanvasGroup != null)
        {
            recipeCanvasGroup.alpha = 0;
            recipeCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            Debug.LogError("CanvasGroup을 찾을 수 없습니다.");
        }

        dialogueText.text = "플레이어 대기 중...";
        StartCoroutine(CheckPlayerCountAndStart());
    }

    void Update()
    {
        if (!textUpdateStopped && isPlayerCountReady)
        {
            timer += Time.deltaTime;
            if (timer >= checkInterval)
            {
                timer = 0f;
                CheckHerbCounts();
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && recipeCanvasGroup != null)
        {
            ToggleRecipeImage();
        }
    }

    IEnumerator CheckPlayerCountAndStart()
    {
        while (!PhotonNetwork.InRoom)
        {
            yield return null;
        }

        while (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            yield return new WaitForSeconds(1f);
        }

        isPlayerCountReady = true;
        dialogueText.text = "쌍화차 만들기 체험에 오신 걸 환영합니다.";
        StartCoroutine(UpdateDialogueText());
    }

    IEnumerator UpdateDialogueText()
    {
        yield return new WaitForSeconds(3f);
        dialogueText.text = "일단 쌍화차를 만드는 방법을 알려드리겠습니다.";

        yield return new WaitForSeconds(3f);
        photonView.RPC("SetRecipeAlpha", RpcTarget.All, 1f);

        dialogueText.text = "레시피를 숙지하셨다면 G 키를 눌러주세요.";

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.G));

        SetRecipeAlpha(0f);
        dialogueText.text = "레시피를 다시 보고 싶으시다면 G 키를 누르면 언제든지 볼 수 있습니다.";

        yield return new WaitForSeconds(3f);
        dialogueText.text = "그러면 먼저 재료들을 가져옵시다.";
        yield return new WaitForSeconds(3f);
        dialogueText.text = "재료는 왼쪽에 위치한 서랍장에서 얻을 수 있습니다.";
        yield return new WaitForSeconds(3f);
        dialogueText.text = "각각의 서랍장에서 재료를 2개씩 챙깁시다.";
    }

    private void ToggleRecipeImage()
    {
        recipeCanvasGroup.alpha = recipeCanvasGroup.alpha == 1f ? 0f : 1f;
        recipeCanvasGroup.blocksRaycasts = recipeCanvasGroup.alpha == 1f;
    }

    [PunRPC]
    private void SetRecipeAlpha(float alpha)
    {
        recipeCanvasGroup.alpha = alpha;
        recipeCanvasGroup.blocksRaycasts = alpha == 1f;
    }

    void CheckHerbCounts()
    {
        int herbCount = GameObject.FindGameObjectsWithTag("Herb").Length;
        int herb2Count = GameObject.FindGameObjectsWithTag("Herb2").Length;
        int herb3Count = GameObject.FindGameObjectsWithTag("Herb3").Length;
        int herb4Count = GameObject.FindGameObjectsWithTag("Herb4").Length;
        int herb5Count = GameObject.FindGameObjectsWithTag("Herb5").Length;
        int herb6Count = GameObject.FindGameObjectsWithTag("Herb6").Length;
        int herb7Count = GameObject.FindGameObjectsWithTag("Herb7").Length;
        int herb8Count = GameObject.FindGameObjectsWithTag("Herb8").Length;
        int herb9Count = GameObject.FindGameObjectsWithTag("Herb9").Length;

        Debug.Log($"현재 재료 상태 - Herb: {herbCount}, Herb2: {herb2Count}, Herb3: {herb3Count}, Herb4: {herb4Count}");
        Debug.Log($"Herb5: {herb5Count}, Herb6: {herb6Count}, Herb7: {herb7Count}, Herb8: {herb8Count}, Herb9: {herb9Count}");

        if (herbCount >= 1 && herb2Count >= 1 && herb3Count >= 1 && herb4Count >= 1 &&
            herb5Count >= 1 && herb6Count >= 1 && herb7Count >= 1 && herb8Count >= 1 && herb9Count >= 1)
        {
            Debug.Log("모든 재료가 준비되었습니다. 다음 단계로 이동합니다.");
            dialogueText.text = "재료를 전부 가져오셨네요! 이제 차를 끓여볼까요?"; // 텍스트 출력 추가
            StartCoroutine(WaitAndMoveCamera()); // 다음 단계로 이동
            textUpdateStopped = true; // 텍스트 업데이트 멈춤
        }
        else
        {
            Debug.Log("재료가 아직 모두 모이지 않았습니다.");
        }
    }


    IEnumerator WaitAndMoveCamera()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("카메라 이동을 시작합니다.");
        if (cameraMoveScript != null)
        {
            cameraMoveScript.enabled = false;
        }

        mainCamera.transform.position = cameraTargetPoint.position;
        mainCamera.transform.rotation = cameraTargetPoint.rotation;

        if (playerMoveScript != null)
        {
            playerMoveScript.enabled = false;
        }

        Debug.Log("카메라가 이동하였습니다. 다음 텍스트를 표시합니다.");
        dialogueText.text = "접시 위에 있는 재료들을 주전자로 드래그해봅시다";
    }
}
