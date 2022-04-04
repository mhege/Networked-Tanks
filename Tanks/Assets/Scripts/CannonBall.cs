using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CannonBall : MonoBehaviourPun
{
    public GameObject ball;
    private GameObject InstanBall;

    private Vector3 yOffset = new Vector3(0.0f, 0.52f, 0.0f);
    private bool boolBoom = false;
    AudioSource boom;
    PhotonView view;

    // An RPC call is made to play the cannonball hit on all clients
    [PunRPC]
    private void playBoom()
    {
        boom.Play();
    }

    // Initialize audio and photon view
    void Start()
    {
        view = GetComponent<PhotonView>();
        boom = GetComponent<AudioSource>();
    }

    // The cannonball instantiation is based on the one who fired and whether the player has a powerup.
    void Update()
    {
        if(view.IsMine)
            if (Input.GetKeyDown("space") && !exists())
            {
                InstanBall = PhotonNetwork.Instantiate(ball.name, transform.position + (transform.forward*1.2f) + yOffset, Quaternion.identity) as GameObject;
                InstanBall.GetComponent<ball>().setReflect(gameObject.GetComponent<TankController>().getPowerup());
            }

        if (boolBoom)
        {
            view.RPC("playBoom", RpcTarget.All, null);
            boolBoom = false;
        }
    }

    // Can only fire one shot at a time.
    private bool exists()
    {
        if (InstanBall != null)
            return true;
        else
            return false;
    }

    // Switch states to cause a sound
    public void setBoolBoom(bool didBoom)
    {
        boolBoom = didBoom;
    }

}
