using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class connectToServer : MonoBehaviourPunCallbacks
{
    // Vital function to make the initial connection to the photon network.
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // When connection is done, join the lobby system.
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // Switch to lobby scene
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

}
