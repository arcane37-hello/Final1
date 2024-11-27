using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GrabObject : MonoBehaviourPun, IPunObservable
{
    private Camera mainCamera;
    private bool isObjectGrabbed = false;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float objectZDistance;
    private float fixedYPosition;

    public AudioClip grabSound; // 클릭 시 재생할 사운드
    private AudioSource audioSource;

    public bool isInteractable = true;

    // QuestText를 private로 선언
    private Text questText;
    private Text diaText;

    void Start()
    {
        mainCamera = Camera.main;
        networkPosition = transform.position;
        networkRotation = transform.rotation;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = grabSound;
        audioSource.playOnAwake = false;

        // 이름으로 QuestText 오브젝트 찾기
        GameObject questTextObject = GameObject.Find("QuestText");
        if (questTextObject != null)
        {
            questText = questTextObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogWarning("QuestText 오브젝트를 찾을 수 없습니다.");
        }

        GameObject DiaTextObject = GameObject.Find("InfoText");
        if (DiaTextObject != null)
        {
            diaText = DiaTextObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogWarning("InfoText 오브젝트를 찾을 수 없습니다.");
        }

        if (CompareTag("Herb6")) // 오브젝트 태그가 "Herb6"인 경우
        {
            isInteractable = false;
        }
        else
        {
            isInteractable = true;
        }
    }

    void Update()
    {
        if (isObjectGrabbed)
        {
            DragObject();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 5);
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        if (Input.GetMouseButtonUp(0) && isObjectGrabbed)
        {
            EndDrag();
        }
    }

    private void TryStartDrag()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
        {
            // 상호작용 가능한지 확인
            if (!isInteractable)
            {
                if (diaText != null)
                {
                    diaText.text = "생강은 손질을 한 후에 넣을 수 있습니다. 칼을 사용해서 손질해 봅시다.";
                }
                else
                {
                    Debug.LogWarning("diaText가 설정되지 않았습니다.");
                }

                Debug.Log("이 오브젝트는 현재 상호작용할 수 없습니다.");
                return;
            }

            isObjectGrabbed = true;
            objectZDistance = Vector3.Distance(mainCamera.transform.position, transform.position);
            fixedYPosition = transform.position.y;

            photonView.RPC("PlayGrabSound", RpcTarget.All); // 모든 플레이어에게 사운드 재생

            // 태그에 따라 텍스트 갱신
            UpdateQuestText();
        }
    }

    private void DragObject()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = objectZDistance;

        Vector3 objectPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        objectPosition.y = fixedYPosition;
        transform.position = objectPosition;

        photonView.RPC("UpdatePositionAndRotation", RpcTarget.Others, transform.position, transform.rotation);
    }

    private void EndDrag()
    {
        isObjectGrabbed = false;
    }

    [PunRPC]
    private void UpdatePositionAndRotation(Vector3 position, Quaternion rotation)
    {
        networkPosition = position;
        networkRotation = rotation;
    }

    [PunRPC]
    private void PlayGrabSound()
    {
        if (audioSource != null && grabSound != null)
        {
            audioSource.Play();
        }
    }

    // QuestText 갱신 로직 추가
    private void UpdateQuestText()
    {
        if (questText == null)
        {
            Debug.LogWarning("QuestText가 설정되지 않았습니다.");
            return;
        }

        // 태그에 따라 텍스트 갱신
        if (CompareTag("Herb"))
        {
            questText.text = "계피\n몸의 찬 기운을 없애주고\n소화, 혈액 순환에 도움";
        }
        else if (CompareTag("Herb2"))
        {
            questText.text = "감초\n기침 억제, 목 통증 완화";
        }
        else if (CompareTag("Herb3"))
        {
            questText.text = "대추\n간을 보호하고 담즙 분비 유도";
        }
        else if (CompareTag("Herb4"))
        {
            questText.text = "천궁\n혈액 순환에 도움\n진통효과";
        }
        else if (CompareTag("Herb5"))
        {
            questText.text = "작약\n진통, 혈액 순환에 도움";
        }
        else if (CompareTag("Herb6"))
        {
            questText.text = "생강\n혈액을 정화, 혈액 순환에 도움";
        }
        else if (CompareTag("Herb7"))
        {
            questText.text = "황기\n면역력 증진, 피로회복에 효과적";
        }
        else if (CompareTag("Herb8"))
        {
            questText.text = "숙지황\n어지럼증, 변비 등에 효과적";
        }
        else if (CompareTag("Herb9"))
        {
            questText.text = "당귀\n이명현상, 불면증에 효과적";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
