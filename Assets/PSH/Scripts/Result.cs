using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    private Button retryButton;  // RetryButton 참조
    public Text targetText;  // 갱신할 텍스트 오브젝트
    public string nextText = "튜토리얼이 끝났습니다";  // 갱신될 텍스트

    void Start()
    {
        // RetryButton을 찾아서 비활성화
        retryButton = GameObject.Find("RetryButton")?.GetComponent<Button>();
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(false); // 시작 시 비활성화
            retryButton.onClick.AddListener(ReloadKMCScene);  // 버튼 클릭 시 씬 전환 메서드 연결
        }
        else
        {
            Debug.LogError("RetryButton을 찾을 수 없습니다.");
        }

        // targetText가 할당되지 않았을 경우 오류 로그 출력
        if (targetText == null)
        {
            Debug.LogError("갱신할 텍스트 오브젝트가 설정되지 않았습니다.");
        }
    }

    // "완벽하게 만드셨네요 잘하셨습니다" 텍스트가 출력된 후 실행되는 함수
    public void OnEvaluationComplete()
    {
        if (targetText != null)
        {
            // 5초 대기 후 텍스트 갱신 및 버튼 활성화
            StartCoroutine(UpdateTextAndActivateButton());
        }
    }

    // 텍스트 갱신 및 버튼 활성화 코루틴
    IEnumerator UpdateTextAndActivateButton()
    {
        // 5초 대기
        yield return new WaitForSeconds(5f);

        // 지정된 텍스트로 갱신
        targetText.text = nextText;

        // RetryButton 활성화
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(true);
        }
    }

    // "KMC" 씬으로 전환하는 메서드
    public void ReloadKMCScene()
    {
        SceneManager.LoadScene("KMC");
    }
}