using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame2 : MonoBehaviour
{
    public Text dialogueText;  // 텍스트 UI 참조
    private float checkInterval = 1f;  // Herb 개수를 확인하는 간격
    private float timer = 0f;

    void Start()
    {
        // 처음 텍스트 출력
        dialogueText.text = "쌍화차 만들기 체험에 오신 걸 환영합니다.";
        StartCoroutine(UpdateDialogueText());
    }

    void Update()
    {
        // 일정 간격으로 Herb 오브젝트 개수 체크
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
        }
    }
}
