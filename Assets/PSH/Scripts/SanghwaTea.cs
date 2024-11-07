using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanghwaTea : MonoBehaviour
{
    private Minigame2 minigame2Script;
    private Text dialogueText;
    private Cabinet cabinetScript; // Cabinet 스크립트 참조

    void Start()
    {
        // Minigame2 스크립트를 찾아서 참조
        minigame2Script = FindObjectOfType<Minigame2>();

        // Cabinet 스크립트를 찾아서 참조
        cabinetScript = FindObjectOfType<Cabinet>();
        if (cabinetScript == null)
        {
            Debug.LogError("Cabinet 스크립트를 찾을 수 없습니다.");
        }

        // InfoText 오브젝트를 찾아서 텍스트 참조
        GameObject infoTextObject = GameObject.Find("InfoText");
        if (infoTextObject != null)
        {
            dialogueText = infoTextObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogError("InfoText 오브젝트를 찾을 수 없습니다.");
        }
    }

    void OnMouseDown()
    {
        // 클릭 시 Minigame2의 스크립트 활성화하고 TeaStack 추가 후 오브젝트 제거
        if (minigame2Script != null)
        {
            minigame2Script.cameraMoveScript.enabled = true;
            minigame2Script.playerMoveScript.enabled = true;
        }

        if (cabinetScript != null)
        {
            cabinetScript.AddTeaStack();  // TeaStack 추가 메서드 호출
        }

        // 텍스트 갱신 후 오브젝트 제거
        if (dialogueText != null)
        {
            dialogueText.text = " 오른쪽에 있는 NPC에게 이동하면 평가를 받을 수 있습니다";
        }

        Destroy(gameObject);  // 오브젝트 제거
    }
}
