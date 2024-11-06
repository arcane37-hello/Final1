using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanghwaTea : MonoBehaviour
{
    private Minigame2 minigame2Script;

    void Start()
    {
        // Minigame2 스크립트를 찾아서 참조
        minigame2Script = FindObjectOfType<Minigame2>();
    }

    void OnMouseDown()
    {
        // 클릭 시 Minigame2의 스크립트 활성화하고 오브젝트 제거
        if (minigame2Script != null)
        {
            minigame2Script.cameraMoveScript.enabled = true;
            minigame2Script.playerMoveScript.enabled = true;
        }

        Destroy(gameObject);  // 오브젝트 제거
    }
}
