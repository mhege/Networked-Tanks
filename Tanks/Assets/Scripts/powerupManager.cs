using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class powerupManager : MonoBehaviour
{
    private bool delete = false;
    PhotonView view;

    // No need for a network call here, just a standard destroy works fine.
    [PunRPC]
    private void destroyOnNetwork()
    {
        Destroy(gameObject);
    }

    // Initialize the view
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Check ownership before deletion. The powerups were made to be owned by master, so it's checking if this is master.
    void Update()
    {
        if (delete && view.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            view.RPC("destroyOnNetwork", RpcTarget.AllBuffered, null);
            delete = false;
        }
            
    }

    // Setter for deletion
    public void setDelete(bool set)
    {
        delete = set;
    }
}
