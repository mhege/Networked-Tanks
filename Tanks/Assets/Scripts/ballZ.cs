using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ballZ : MonoBehaviourPun
{

    public string player = "P1";

    private GameObject tankBody;
    private Vector3 direction;
    public float speed = 4.0f;
    private const float tankHit = .5f;
    private const float partHit = .5f;
    private const float borderHit = .5f;
    private bool reflectBool = false;
    private Vector3 yOffset = new Vector3(0.0f, -.25f, 0.0f);

    private GameObject gamemanager;

    PhotonView view;

    AudioSource Shot;

    private void Awake()
    {
        //Play Cannon shot upon awaking. This plays on both clients.
        Shot = GetComponent<AudioSource>();
        Shot.Play();
    }

    //Call an RPC to signal the destruction of the wall on all clients. The walls are networked.
    [PunRPC]
    private void destroyWall(int viewID)
    {
        PhotonView.Find(viewID).gameObject.GetComponent<wallManager>().setDelete(true);
    }

    //Initialize the cannonball with proper coordinates. Attach the game manager to end game if a player is hit.
    void Start()
    {
        view = GetComponent<PhotonView>();
        gamemanager = GameObject.FindGameObjectWithTag("GM");
        tankBody = GameObject.FindGameObjectWithTag(player);
        direction = tankBody.transform.forward;
        transform.rotation = Quaternion.LookRotation(tankBody.transform.forward);
    }

    // Movement of the cannonball is based on who fired.
    void Update()
    {
        if (view.IsMine)
            move();
    }

    // Collisions are all handled with ray tracing, so a FixedUpdate is preferred.
    private void FixedUpdate()
    {
        try {
            collide();
        }catch(System.NullReferenceException e) { }
    }

    // Basic movement with a fixed speed.
    private void move()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void collide()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position + yOffset, transform.forward, Color.red, 10.0f);

        // When cannonball interacts with wall, it must be destroyed on all clients and network.
        if (Physics.SphereCast(transform.position + yOffset, .00001f, transform.forward, out hit, 24.0f) && hit.transform.gameObject.tag == "partition" && (transform.position - hit.point).magnitude < partHit)
        {

            tankBody.GetComponentInParent<CannonBallZombie>().setBoolBoom(true);

            this.view.RPC("destroyWall", RpcTarget.MasterClient, hit.transform.gameObject.GetPhotonView().ViewID);

            if (reflectBool)
            {
                direction = reflect(transform.position - hit.point + yOffset, hit.transform.forward);
                transform.rotation = Quaternion.LookRotation(direction);

            }
            else
            {
                if (view.IsMine)
                    PhotonNetwork.Destroy(gameObject);
            }

        }// If a player is hit, a gameover is issued.
        else if (Physics.SphereCast(transform.position + yOffset, .00001f, transform.forward, out hit, 24.0f) && (hit.transform.gameObject.tag == "P1" || hit.transform.gameObject.tag == "P2") && (transform.position - hit.point).magnitude < tankHit)
        {
            Time.timeScale = 0.0f;
            gamemanager.GetComponent<gamemanager>().setGameover(true);
        }// Else if it hits anything else, destroy the cannonball and make a sound.
        else if (Physics.SphereCast(transform.position + yOffset, .00001f, transform.forward, out hit, 24.0f) && (hit.transform.gameObject.tag == "ballP1" || hit.transform.gameObject.tag == "border" || hit.transform.gameObject.tag == "Z2" || hit.transform.gameObject.tag == "Z3" || hit.transform.gameObject.tag == "power") && (transform.position - hit.point).magnitude < borderHit)
        {
            tankBody.GetComponentInParent<CannonBallZombie>().setBoolBoom(true);
            if (view.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }

    }

    // Calculate the reflection vector.
    private Vector3 reflect(Vector3 incomingDirection, Vector3 norm)
    {
        return Vector3.Reflect(incomingDirection, norm).normalized;
    }

    // Switch to reflect state.
    public void setReflect(bool refState)
    {
        reflectBool = refState;
    }
}
