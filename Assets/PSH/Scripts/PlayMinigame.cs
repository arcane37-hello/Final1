using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMinigame : MonoBehaviour
{
    public Text dialogueText;  // UI 텍스트 연결
    public GameObject[] grabObjects;  // GrabObject 스크립트가 붙은 오브젝트들
    public Button yesButton;   // YesButton 참조
    public Button noButton;    // NoButton 참조
    public GameObject realGame;  // 체험 모드를 위한 RealGame 스크립트가 붙은 오브젝트
    public Kettle kettle;
    void Start()
    {
        // 처음에는 GrabObject 스크립트가 비활성화된 상태로 설정
        ToggleGrabObjects(false);

        // 처음에는 버튼도 비활성화
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // 첫 번째 텍스트 출력
        StartCoroutine(DisplayTextSequence());
    }

    // 텍스트 순서대로 출력하는 코루틴
    IEnumerator DisplayTextSequence()
    {
        // 첫 번째 텍스트 출력
        dialogueText.text = "수세미차 만들기 체험에 오신 걸 환영합니다";
        yield return new WaitForSeconds(3f);  // 3초 대기

        // 두 번째 질문 텍스트 출력
        dialogueText.text = "수세미차 만드시는 건 처음이신가요?";

        // YesButton과 NoButton 활성화
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        // YesButton 클릭 시 튜토리얼 시작
        yesButton.onClick.AddListener(StartTutorial);
        noButton.onClick.AddListener(StartExperienceMode);
    }

    // YesButton 클릭 시 튜토리얼 시작
    void StartTutorial()
    {
        // 버튼 비활성화
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // 두 번째 텍스트 출력 변경
        dialogueText.text = "알겠습니다 그러면 튜토리얼을 시작하겠습니다";

        // 튜토리얼 시작
        StartCoroutine(StartTutorialSequence());
    }

    // 튜토리얼 모드의 순차적인 동작 코루틴
    IEnumerator StartTutorialSequence()
    {
        // 세 번째 텍스트 출력
        yield return new WaitForSeconds(3f);
        dialogueText.text = "먼저 말린 수세미를 주전자에 넣어 볼까요?";
        yield return new WaitForSeconds(3f);

        // 네 번째 텍스트 출력
        dialogueText.text = "수세미 조각 한 개 당 3g 입니다 5개를 넣어야 합니다";
        yield return new WaitForSeconds(3f);

        // GrabObject 스크립트 활성화
        ToggleGrabObjects(true);
    }

    // NoButton 클릭 시 체험 모드 시작
    public void StartExperienceMode()
    {
        // 버튼 비활성화
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        kettle.isExperienceMode = true;

        // 체험 모드 시작 텍스트 출력
        dialogueText.text = "체험 모드를 시작하겠습니다.";
        StartCoroutine(StartExperienceSequence());
    }

    // 체험 모드로 전환하는 코루틴
    IEnumerator StartExperienceSequence()
    {
        yield return new WaitForSeconds(3f);
        dialogueText.text = "수세미차를 만들어봅시다";

        // PlayMinigame 비활성화, RealGame 활성화
        this.gameObject.SetActive(false);  // PlayMinigame 오브젝트 비활성화
        realGame.SetActive(true);  // RealGame 오브젝트 활성화
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

    // 외부에서 텍스트를 갱신할 수 있는 함수
    public void UpdateText(string newText)
    {
        dialogueText.text = newText;  // 텍스트 갱신
    }
}