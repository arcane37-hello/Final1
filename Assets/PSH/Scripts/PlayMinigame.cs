using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMinigame : MonoBehaviour
{
    public Text dialogueText;  // UI 텍스트를 연결
    public GameObject[] grabObjects;  // GrabObject 스크립트가 붙은 오브젝트들

    void Start()
    {
        // 처음에는 GrabObject 스크립트가 비활성화된 상태로 설정
        ToggleGrabObjects(false);

        // 첫 번째 텍스트 출력
        StartCoroutine(DisplayTextSequence());
    }

    // 텍스트 순서대로 출력하는 코루틴
    IEnumerator DisplayTextSequence()
    {
        // 첫 번째 텍스트 출력
        dialogueText.text = "수세미차 만들기 체험에 오신 걸 환영합니다";
        yield return new WaitForSeconds(3f);  // 3초 대기

        // 두 번째 텍스트 출력
        dialogueText.text = "먼저 말린 수세미를 주전자에 넣어 볼까요?";
        yield return new WaitForSeconds(3f);  // 3초 대기

        // GrabObject 스크립트 활성화
        ToggleGrabObjects(true);
    }

    // GrabObject 스크립트가 활성화되거나 비활성화되도록 설정하는 함수
    void ToggleGrabObjects(bool isActive)
    {
        foreach (GameObject obj in grabObjects)
        {
            GrabObject grabScript = obj.GetComponent<GrabObject>();
            if (grabScript != null)
            {
                grabScript.enabled = isActive;
            }
        }
    }
}