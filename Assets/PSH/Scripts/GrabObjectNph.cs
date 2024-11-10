using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjectNph : MonoBehaviour
{
    private Camera mainCamera;  // 메인 카메라 참조
    private GameObject selectedObject;  // 현재 선택된 오브젝트
    private bool isObjectGrabbed = false; // 오브젝트가 잡힌 상태인지 여부
    private float objectZDistance; // 카메라와 오브젝트 간의 거리
    private float fixedYPosition;  // 오브젝트의 Y 좌표를 고정하기 위한 변수

    void Start()
    {
        // 메인 카메라 자동 설정
        mainCamera = Camera.main;

        // 메인 카메라가 없을 경우 경고 메시지 출력
        if (mainCamera == null)
        {
            Debug.LogError("메인 카메라가 씬에 존재하지 않습니다. 카메라에 'Main Camera' 태그가 있는지 확인하세요.");
        }
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            // Ray를 카메라에서 마우스 위치로 발사
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 오브젝트가 클릭되었는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                // 해당 오브젝트에 GrabObject 스크립트가 있는지 확인
                GrabObject grabScript = hit.collider.GetComponent<GrabObject>();

                if (grabScript != null && grabScript == this)  // 현재 스크립트가 붙은 오브젝트만 이동 가능
                {
                    // 오브젝트를 선택하고, 카메라와의 Z축 거리 계산
                    selectedObject = hit.collider.gameObject;
                    objectZDistance = Vector3.Distance(mainCamera.transform.position, selectedObject.transform.position);
                    fixedYPosition = selectedObject.transform.position.y;  // 선택한 오브젝트의 Y 좌표 고정
                    isObjectGrabbed = true;
                }
            }
        }

        // 마우스 왼쪽 버튼을 누르고 있는 동안 오브젝트를 이동
        if (Input.GetMouseButton(0) && isObjectGrabbed && selectedObject != null)
        {
            // 마우스 위치를 따라 오브젝트를 이동 (카메라 앞쪽으로 일정 거리 유지)
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = objectZDistance; // 오브젝트와 카메라 간의 거리 유지

            // 마우스 위치에 맞게 오브젝트를 움직이되, Y 좌표는 고정
            Vector3 objectPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            objectPosition.y = fixedYPosition;  // Y 좌표를 고정하여 이동하지 않도록 함
            selectedObject.transform.position = objectPosition;
        }

        // 마우스 왼쪽 버튼을 뗐을 때 오브젝트를 내려놓음
        if (Input.GetMouseButtonUp(0) && isObjectGrabbed)
        {
            isObjectGrabbed = false;
            selectedObject = null; // 오브젝트 선택 해제
        }
    }
}