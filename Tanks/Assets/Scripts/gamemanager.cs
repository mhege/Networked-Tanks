using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class gamemanager : MonoBehaviour
{

    public GameObject gameoverScreen;
    private bool gameover = false;

    // Keep checking if master left. If they leave, close the whole game.
    // If a gameover is triggered, show everyone that the game is done.
    void Update()
    {
        if (gameover)
            gameoverScreen.SetActive(true);

        if (PhotonNetwork.MasterClient.ActorNumber != 1)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }

    // Setter for gameover
    public void setGameover(bool game)
    {
        gameover = game;
    }

}
