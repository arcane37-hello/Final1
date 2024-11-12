using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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
            ToggleRecipeImage();  // Photon RPC 대신 로컬 호출
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
        photonView.RPC("SetRecipeAlpha", RpcTarget.All, 1f);  // 이미지 알파를 1로 설정하여 출력

        dialogueText.text = "레시피를 숙지하셨다면 G 키를 눌러주세요.";

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.G));

        SetRecipeAlpha(0f);  // G 키 입력 후 알파값 0
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

        if (herbCount >= 2 && herb2Count >= 2 && herb3Count >= 2 && herb4Count >= 2 && herb5Count >= 2)
        {
            dialogueText.text = "재료를 전부 가져오셨네요 이제 차를 끓여볼까요?";
            StartCoroutine(WaitAndMoveCamera());
            textUpdateStopped = true;
        }
    }

    IEnumerator WaitAndMoveCamera()
    {
        yield return new WaitForSeconds(2f);

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

        dialogueText.text = "접시 위에 있는 재료들을 주전자로 드래그해봅시다";
    }
}
