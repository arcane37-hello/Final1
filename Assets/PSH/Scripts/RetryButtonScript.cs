using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButtonScript : MonoBehaviourPun
{
    public void OnRetryButtonClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DisconnectAllPlayers", RpcTarget.All); // 모든 플레이어에게 서버 연결 해제 명령을 전송
        }
    }

    [PunRPC]
    private void DisconnectAllPlayers()
    {
        PhotonNetwork.Disconnect(); // Photon 서버 연결 해제
    }

    private void Update()
    {
        // 포톤 서버에서 완전히 분리되었을 때 로컬 씬 이동
        if (!PhotonNetwork.IsConnected && SceneManager.GetActiveScene().name != "KMC")
        {
            SceneManager.LoadScene("KMC"); // 로컬 씬으로 이동
        }
    }
}
