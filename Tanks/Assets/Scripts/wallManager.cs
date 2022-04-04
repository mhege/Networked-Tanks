using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class wallManager : MonoBehaviourPun
{

    private bool delete = false;
    PhotonView view;

    // No need for a network call, just a standard destroy works here.
    [PunRPC]
    private void destroyOnNetwork()
    {
        Destroy(gameObject);
    }

    // Initialize view
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Check ownership before deleting. All walls were made to be under the master, so it's checking for the masterclient
    void Update()
    {
        if (delete && view.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            view.RPC("destroyOnNetwork", RpcTarget.AllBuffered, null);
    }

    // Setter for deletion
    public void setDelete(bool set)
    {
        delete = set;
    }

}
