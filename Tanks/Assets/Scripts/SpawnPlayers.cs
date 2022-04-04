using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject walls;
    public GameObject powerups;
    private Vector3 position1 = new Vector3(-8.0f, 0.0f, -8.0f);
    private Vector3 position2 = new Vector3(8.0f, 0.0f, -5.0f);
    private int numPlayers = 0;
    
    void Start()
    {
        // Check how many players are in the room so you know where to spawn the players.
        // The master also spawns the walls and powerups
        numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        if (numPlayers == 1)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, position1, Quaternion.identity);
            PhotonNetwork.Instantiate(walls.name, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            PhotonNetwork.Instantiate(powerups.name, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        } 
        else
            PhotonNetwork.Instantiate(playerPrefab.name, position2, Quaternion.identity);
    }

}
