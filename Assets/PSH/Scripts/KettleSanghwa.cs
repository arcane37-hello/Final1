using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KettleSanghwa : MonoBehaviourPun
{
    public Transform effectSpawnPoint;
    public GameObject boilingEffectPrefab;
    public Text dialogueText;
    public Transform targetPosition;
    public GameObject objectToReplace;
    public string cupObjectName = "Cup Sanghwa";   // 이동할 오브젝트 이름
    public string spawnPointName = "TeaSpawn";     // 이동할 위치 이름

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private int herbStack = 0;
    private int herb2Stack = 0;
    private int herb3Stack = 0;
    private int herb4Stack = 0;
    private int herb5Stack = 0;
    private bool canInteract = false;
    private bool isBoiling = false;
    private bool isPouring = false;
    private bool boilingComplete = false;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnMouseDown()
    {
        if (!photonView.IsMine) return;

        if (canInteract && !isBoiling && !isPouring)
        {
            if (!boilingComplete)
            {
                photonView.RPC("StartBoilingRPC", RpcTarget.All);
            }
            else
            {
                photonView.RPC("PourTeaRPC", RpcTarget.All);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Herb"))
        {
            herbStack++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Herb2"))
        {
            herb2Stack++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Herb3"))
        {
            herb3Stack++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Herb4"))
        {
            herb4Stack++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Herb5"))
        {
            herb5Stack++;
            Destroy(collision.gameObject);
        }

        if (!canInteract && herbStack >= 2 && herb2Stack >= 2 && herb3Stack >= 2 && herb4Stack >= 2 && herb5Stack >= 2)
        {
            canInteract = true;
            dialogueText.text = "이제 주전자를 클릭해서 물을 끓여봅시다.";
        }
    }

    [PunRPC]
    private void StartBoilingRPC()
    {
        StartCoroutine(StartBoiling());
    }

    private IEnumerator StartBoiling()
    {
        isBoiling = true;
        canInteract = false;
        int timer = 15;

        if (boilingEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effectInstance = Instantiate(boilingEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            effectInstance.transform.SetParent(transform);
        }

        while (timer > 0)
        {
            dialogueText.text = "물이 끓는 중입니다. 0:" + timer.ToString("D2");
            yield return new WaitForSeconds(1f);
            timer--;
        }

        dialogueText.text = "차가 완성된 것 같군요. 이제 주전자를 클릭해서 차를 완성합시다!";
        boilingComplete = true;
        canInteract = true;
        isBoiling = false;
    }

    [PunRPC]
    private void PourTeaRPC()
    {
        StartCoroutine(PourTea());
    }

    private IEnumerator PourTea()
    {
        isPouring = true;
        canInteract = false;

        yield return StartCoroutine(MoveAndRotateKettle(targetPosition.position, targetPosition.rotation));

        yield return new WaitForSeconds(3f);

        ReplaceObject();

        dialogueText.text = "차가 완성됐습니다. 찻잔을 클릭해서 평가를 받아봅시다!";
        canInteract = true;

        yield return StartCoroutine(MoveAndRotateKettle(originalPosition, originalRotation));

        isPouring = false;
    }

    private IEnumerator MoveAndRotateKettle(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPos, elapsedTime / duration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private void ReplaceObject()
    {
        if (objectToReplace != null)
        {
            Destroy(objectToReplace);

            GameObject cupObject = GameObject.Find(cupObjectName);
            Transform spawnPoint = GameObject.Find(spawnPointName)?.transform;

            if (cupObject != null && spawnPoint != null)
            {
                cupObject.transform.position = spawnPoint.position;
                cupObject.transform.rotation = spawnPoint.rotation;
            }
            else
            {
                Debug.LogWarning("Cup Sanghwa 또는 TeaSpawn을 찾을 수 없습니다.");
            }
        }
    }
}
