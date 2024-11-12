using System.Collections;
using UnityEngine;
using Photon.Pun;

public class SanghwaBGM : MonoBehaviourPunCallbacks
{
    public AudioClip backgroundMusic;
    private AudioSource audioSource;
    private bool isPlaying = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 추가
        audioSource.clip = backgroundMusic; // 배경음악 지정
        audioSource.loop = true; // 반복재생 설정
        audioSource.playOnAwake = false; // 자동 재생 해제
        audioSource.volume = 0.2f; // 볼륨 조절
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        CheckAndPlayBGM();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        CheckAndPlayBGM();
    }

    private void CheckAndPlayBGM()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && !isPlaying)
        {
            photonView.RPC("PlayBGM", RpcTarget.All); // 모든 클라이언트에서 BGM 재생
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount < 2 && isPlaying)
        {
            photonView.RPC("StopBGM", RpcTarget.All); // 모든 클라이언트에서 BGM 중지
        }
    }

    [PunRPC]
    private void PlayBGM()
    {
        if (!isPlaying)
        {
            audioSource.Play();
            isPlaying = true;
        }
    }

    [PunRPC]
    private void StopBGM()
    {
        if (isPlaying)
        {
            audioSource.Stop();
            isPlaying = false;
        }
    }
}
