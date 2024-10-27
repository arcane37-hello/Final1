using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealGame : MonoBehaviour
{
    public Text dialogueText;
    public GameObject[] grabObjects;
    public Kettle kettle;
    public GameObject boilingEffectPrefab;
    public Transform effectSpawnPoint;
    public GameObject objectToReplace;
    public GameObject replacementPrefab;

    private bool isReadyToBoil = false;
    public bool isBoiling = false;  // 물 끓이기 중 여부
    private bool isReadyToPour = false;
    private int susemiCount = 0;  // 수세미 추가 횟수
    private float boilingStartTime;
    private float boilingStopTime;

    void Start()
    {
        ToggleGrabObjects(false);
        StartCoroutine(StartExperienceMode());
    }

    // 체험 모드 시작 시 순차적으로 동작하는 코루틴
    IEnumerator StartExperienceMode()
    {
        dialogueText.text = "체험 모드를 시작하겠습니다.";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "수세미차를 만들어봅시다.";
        yield return new WaitForSeconds(3f);

        ToggleGrabObjects(true);

        if (kettle != null)
        {
            isReadyToBoil = true;
        }
    }

    // 수세미 추가 시 호출되는 메서드
    public void AddSusemi()
    {
        susemiCount++;
        Debug.Log("수세미가 추가되었습니다. 현재 수세미 개수: " + susemiCount);
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

    void Update()
    {
        if (isReadyToBoil && Input.GetMouseButtonDown(0) && kettle != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == kettle.gameObject)
                {
                    boilingStartTime = Time.time;
                    StartBoilingTimer();
                    isReadyToBoil = false;
                }
            }
        }

        if (isReadyToPour && Input.GetMouseButtonDown(0) && kettle != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == kettle.gameObject)
                {
                    boilingStopTime = Time.time;
                    StopBoiling();
                    StartCoroutine(PourTeaAndReplaceObject());
                    isReadyToPour = false;
                }
            }
        }
    }

    IEnumerator BoilWater()
    {
        isBoiling = true;
        int timer = 0;

        if (boilingEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effectInstance = Instantiate(boilingEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            effectInstance.transform.SetParent(kettle.transform);
        }

        while (isBoiling)
        {
            dialogueText.text = "물이 끓는 중입니다. " + (timer / 60).ToString("D2") + ":" + (timer % 60).ToString("D2");
            yield return new WaitForSeconds(1f);
            timer++;
        }
    }

    public void StopBoiling()
    {
        isBoiling = false;
        dialogueText.text = "차가 완성되었습니다. 평가를 받아봅시다!";
        Debug.Log("물이 끓는 시간이 기록되었습니다: " + (boilingStopTime - boilingStartTime) + "초");
        isReadyToPour = true;

        if (kettle.realTea != null)
        {
            kettle.realTea.RecordBoilingDuration(boilingStopTime - boilingStartTime);
        }
    }

    public void StartBoilingTimer()
    {
        if (!isBoiling)
        {
            StartCoroutine(BoilWater());
        }
    }

    IEnumerator PourTeaAndReplaceObject()
    {
        yield return StartCoroutine(kettle.MoveAndRotateKettle());

        if (objectToReplace != null && replacementPrefab != null)
        {
            Vector3 replacePosition = objectToReplace.transform.position;
            Quaternion replaceRotation = objectToReplace.transform.rotation;

            Destroy(objectToReplace);
            Instantiate(replacementPrefab, replacePosition, replaceRotation);
        }

        dialogueText.text = "차가 완성되었습니다!";
    }

    public void UpdateText(string newText)
    {
        dialogueText.text = newText;
    }

    public void UpdateBoilingText(string newText)
    {
        dialogueText.text = newText;
    }
}
