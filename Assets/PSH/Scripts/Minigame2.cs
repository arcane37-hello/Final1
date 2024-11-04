using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Minigame2 : MonoBehaviour
{
    public Text dialogueText;  // UI 텍스트를 지정할 수 있는 public 변수

    void Start()
    {
        // 코루틴을 시작하여 텍스트를 순차적으로 갱신
        StartCoroutine(DisplayTextSequence());
    }

    IEnumerator DisplayTextSequence()
    {
        // 첫 번째 텍스트 출력
        dialogueText.text = "쌍화차 만들기 체험에 오신 걸 환영합니다.";
        yield return new WaitForSeconds(3f);  // 3초 대기

        // 두 번째 텍스트 출력
        dialogueText.text = "우선 왼쪽에 있는 서랍장에서 재료들을 가져와봅시다.";
        yield return new WaitForSeconds(3f);  // 3초 대기

        // 세 번째 텍스트 출력
        dialogueText.text = "필요한 재료는 총 10개입니다.";
    }
}
