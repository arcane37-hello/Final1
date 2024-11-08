using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Photon 서버에 연결
        }
    }

    public override void OnConnectedToMaster()
    {
        // 마스터 서버에 연결되면 로비로 입장
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        // 로비에 입장하면 방에 입장 시도, 방이 없으면 새로 생성
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        // 방에 입장하면 Player 프리팹을 네트워크에 생성
        Vector3 spawnPosition = new Vector3(-2.5f, 1, -1);  // 예시: 스폰 위치
        Quaternion spawnRotation = Quaternion.identity;

        PhotonNetwork.Instantiate("Player", spawnPosition, spawnRotation);
        Debug.Log("Player가 방에 입장했습니다.");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("방 입장 실패: " + message);
    }
}
