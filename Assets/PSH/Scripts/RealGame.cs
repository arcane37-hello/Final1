using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealGame : MonoBehaviour
{
    public Text dialogueText;  // UI 텍스트 연결
    public GameObject[] grabObjects;  // GrabObject 스크립트가 붙은 오브젝트들
    public Kettle kettle;  // Kettle 스크립트 참조
    public GameObject boilingEffectPrefab;  // 물 끓일 때 출력할 이펙트 프리팹
    public Transform effectSpawnPoint;  // 이펙트가 생성될 위치
    public GameObject objectToReplace;  // 교체할 대상 오브젝트
    public GameObject replacementPrefab; // 교체될 프리팹

    private bool isReadyToBoil = false;  // 물을 끓일 준비가 되었는지 여부
    private bool isBoiling = false;  // 물이 끓고 있는지 여부
    private bool isReadyToPour = false;  // 차를 따를 준비가 되었는지 여부

    void Start()
    {
        // 처음에는 GrabObject 스크립트가 비활성화된 상태로 설정
        ToggleGrabObjects(false);

        // 체험 모드 시작
        StartCoroutine(StartExperienceMode());
    }

    // 체험 모드 시작 시 순차적으로 동작하는 코루틴
    IEnumerator StartExperienceMode()
    {
        dialogueText.text = "체험 모드를 시작하겠습니다.";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "수세미차를 만들어봅시다.";
        yield return new WaitForSeconds(3f);

        // GrabObject 상호작용 활성화
        ToggleGrabObjects(true);

        // 체험 모드에서는 Kettle도 상호작용 가능
        if (kettle != null)
        {
            isReadyToBoil = true;
        }
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
        // 물 끓이기 준비가 되었고, 마우스 왼쪽 버튼 클릭 시 Kettle에서 물 끓이기
        if (isReadyToBoil && Input.GetMouseButtonDown(0) && kettle != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == kettle.gameObject)
                {
                    StartCoroutine(BoilWater());  // 체험 모드에서는 BoilWater 코루틴 사용
                    isReadyToBoil = false;  // 물 끓이기 중에 다시 실행되지 않도록 설정
                }
            }
        }

        // 차를 따를 준비가 되었고, Kettle을 다시 클릭하면 차 따르기 모션 및 오브젝트 교체
        if (isReadyToPour && Input.GetMouseButtonDown(0) && kettle != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == kettle.gameObject)
                {
                    StartCoroutine(PourTeaAndReplaceObject());  // 차 따르기 모션 및 오브젝트 교체
                    isReadyToPour = false;  // 차 따르기 중복 방지
                }
            }
        }
    }

    // 물 끓이기 코루틴 (0부터 증가하는 타이머 + 이펙트)
    IEnumerator BoilWater()
    {
        isBoiling = true;
        int timer = 0;

        // 물 끓일 때 이펙트 생성
        if (boilingEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effectInstance = Instantiate(boilingEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            effectInstance.transform.SetParent(kettle.transform);  // 이펙트가 Kettle을 따라가도록 설정
        }

        while (isBoiling)
        {
            dialogueText.text = "물이 끓는 중입니다. " + (timer / 60).ToString("D2") + ":" + (timer % 60).ToString("D2");
            yield return new WaitForSeconds(1f);
            timer++;
        }

        // 물 끓이기 완료 후 텍스트 갱신
        dialogueText.text = "차가 완성되었습니다. 평가를 받아봅시다!";
        isBoiling = false;
        isReadyToPour = true;  // 차를 따를 준비 완료
    }

    // 차를 따르고 오브젝트를 교체하는 코루틴
    IEnumerator PourTeaAndReplaceObject()
    {
        // 차 따르기 모션 (Kettle 기울이기)
        yield return StartCoroutine(kettle.MoveAndRotateKettle());  // Kettle 스크립트의 MoveAndRotateKettle 사용

        // 대상 오브젝트 교체
        if (objectToReplace != null && replacementPrefab != null)
        {
            Vector3 replacePosition = objectToReplace.transform.position;
            Quaternion replaceRotation = objectToReplace.transform.rotation;

            Destroy(objectToReplace);  // 기존 오브젝트 삭제
            Instantiate(replacementPrefab, replacePosition, replaceRotation);  // 새로운 프리팹 생성
        }

        dialogueText.text = "차가 완성되었습니다!";
    }
}