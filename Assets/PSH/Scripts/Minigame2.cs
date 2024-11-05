using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame2 : MonoBehaviour
{
    public Text dialogueText;                // 텍스트 UI 참조
    public Transform cameraTargetPoint;      // 카메라가 이동할 목표 지점
    public GameObject player;                // 플레이어 오브젝트
    private Camera mainCamera;               // 메인 카메라
    private CameraMove cameraMoveScript;     // CameraMove 스크립트 참조
    private float checkInterval = 1f;        // Herb 개수 확인 간격
    private float timer = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        cameraMoveScript = mainCamera.GetComponent<CameraMove>();

        dialogueText.text = "쌍화차 만들기 체험에 오신 걸 환영합니다.";
        StartCoroutine(UpdateDialogueText());
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckHerbCount();
        }
    }

    IEnumerator UpdateDialogueText()
    {
        yield return new WaitForSeconds(3f);
        dialogueText.text = "우선 왼쪽에 있는 서랍장에서 재료들을 가져와봅시다.";
        yield return new WaitForSeconds(3f);
        dialogueText.text = "필요한 재료는 총 10개입니다.";
    }

    // Herb 태그를 가진 오브젝트가 10개 이상인지 확인하는 메서드
    void CheckHerbCount()
    {
        GameObject[] herbObjects = GameObject.FindGameObjectsWithTag("Herb");
        if (herbObjects.Length >= 10)
        {
            dialogueText.text = "재료를 전부 가져오셨네요 이제 차를 끓여볼까요?";
            StartCoroutine(MoveCameraAndDeactivatePlayer());
        }
    }

    // 카메라를 목표 위치로 부드럽게 이동시키고 플레이어를 비활성화하는 코루틴
    IEnumerator MoveCameraAndDeactivatePlayer()
    {
        yield return new WaitForSeconds(3f);

        if (cameraMoveScript != null)
        {
            cameraMoveScript.enabled = false;
        }

        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        Vector3 targetPosition = cameraTargetPoint.position;
        Quaternion targetRotation = cameraTargetPoint.rotation;

        float elapsedTime = 0f;
        float duration = 2f;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;

        if (player != null)
        {
            player.SetActive(false);
        }
    }
}
