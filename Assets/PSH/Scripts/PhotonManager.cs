using Photon.Pun;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // 포톤 서버에 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    // 서버 연결 성공 시 호출되는 콜백
    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤 서버에 연결되었습니다.");
    }

    // 서버 연결 실패 시 호출되는 콜백
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogWarning("서버 연결이 실패했습니다: " + cause);
    }
}
