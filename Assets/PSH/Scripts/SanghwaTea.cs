using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SanghwaTea : MonoBehaviourPun
{
    private Minigame2 minigame2Script;
    private Text dialogueText;
    private Cabinet cabinetScript;

    void Start()
    {
        minigame2Script = FindObjectOfType<Minigame2>();
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

    void Update()
    {
        if (cabinetScript == null)
        {
            cabinetScript = FindObjectOfType<Cabinet>();
            if (cabinetScript == null)
            {
                Debug.LogError("Cabinet 스크립트를 찾을 수 없습니다.");
            }
        }
    }

    void OnMouseDown()
    {
        if (!photonView.IsMine) return;

        if (minigame2Script != null)
        {
            minigame2Script.cameraMoveScript.enabled = true;
            minigame2Script.playerMoveScript.enabled = true;
        }

        if (cabinetScript != null)
        {
            cabinetScript.AddTeaStack();
        }

        photonView.RPC("UpdateDialogueText", RpcTarget.All, "오른쪽에 있는 NPC에게 이동하면 평가를 받을 수 있습니다");
        photonView.RPC("RestoreCamera", RpcTarget.All);

        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void RestoreCamera()
    {
        if (minigame2Script != null)
        {
            minigame2Script.cameraMoveScript.enabled = true;
            minigame2Script.playerMoveScript.enabled = true;
        }
    }

    [PunRPC]
    private void UpdateDialogueText(string message)
    {
        if (dialogueText != null)
        {
            dialogueText.text = message;
        }
    }
}
