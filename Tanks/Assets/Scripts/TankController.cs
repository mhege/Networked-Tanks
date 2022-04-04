using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankController : MonoBehaviourPun
{

    [SerializeField] private float speed = 1.0f;

    private CharacterController controller;
    private Vector3 input;
    private Vector3 velocity;
    private bool moveBool = false;

    private Vector3 right = new Vector3(1, 0, 0);
    private Vector3 left = new Vector3(-1, 0, 0);
    private Vector3 up = new Vector3(0, 0, 1);
    private Vector3 down = new Vector3(0, 0, -1);

    private float powerUpHit = 1.5f;
    private bool powerup = false;
    private float powerUpTime = 30.0f;

    private Vector3 rayOffset = new Vector3(0.0f, .25f, 0.0f);

    PhotonView view;
    
    // This RPC call is targeted to master to deal with the deletion.
    [PunRPC]
    private void destroyPowerup(int viewID)
    {
        try
        {
            PhotonView.Find(viewID).gameObject.GetComponent<powerupManager>().setDelete(true);
        }catch(System.NullReferenceException e) { }
        
    }

    // Signal to the zombies all the players in the game and initialize movement controller.
    void Start()
    {
        view = GetComponent<PhotonView>();

        GameObject[] signal = GameObject.FindGameObjectsWithTag("Z1");
        foreach(GameObject GO in signal)
        {
            GO.GetComponent<ZombieController>().playerGOs(view.ViewID);
        }
        
        controller = GetComponent<CharacterController>();
    }

    // If this is the player's view, move your character
    void Update()
    {
        if(view.IsMine)
            Move();

    }

    // The whole game is handled by ray tracing. So fixed update is preferred.
    private void FixedUpdate()
    {
        collide();
    }

    // Simple movement controller with a locked y-axis. The locking prevents gameobject to move when bumping into eachother.
    private void Move()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 playerInput = Vector3.zero;

        //lock y-axis
        controller.enabled = false;
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        controller.enabled = true;

        if (input.x == 1)
        {
            face(right);
            if (transform.forward == right)
                moveBool = true;
            else
                moveBool = false;
        }
        else if (input.x == -1)
        {
            face(left);
            if (transform.forward == left)
                moveBool = true;
            else
                moveBool = false;
        }
        else if (input.z == 1)
        {
            face(up);
            if (transform.forward == up)
                moveBool = true;
            else
                moveBool = false;

        }
        else if (input.z == -1)
        {
            face(down);
            if (transform.forward == down)
                moveBool = true;
            else
                moveBool = false;
        }
        else
            moveBool = false;

        if (moveBool)
            velocity = transform.forward * speed;
        else
            velocity = Vector3.zero;

        controller.Move(velocity * Time.deltaTime);
        
    }

    // Pick up the powerup with a simple ray trace at close range.
    private void collide()
    {
        RaycastHit hit;

        //Debug.DrawRay(transform.position + rayOffset, transform.forward, Color.red, 10.0f);
        
        if (Physics.SphereCast(transform.position + rayOffset, .00001f, transform.forward, out hit, 24.0f) && hit.transform.gameObject.tag == "power" && (transform.position - hit.point).magnitude < powerUpHit)
        {
            if (view.IsMine)
            {
                powerup = true;
                StartCoroutine(powerUpLength());
            }

            this.view.RPC("destroyPowerup", RpcTarget.MasterClient, hit.transform.gameObject.GetPhotonView().ViewID);
            
        }

    }

    // Coroutine that tracks how much time is left to the powerup.
    IEnumerator powerUpLength()
    {
        float normTime = 0.0f;
        while(normTime <= 1.0f)
        {
            normTime += Time.deltaTime/powerUpTime;
            yield return null;
        }

        powerup = false;
        
    }

    // Face in the direction of rotation.
    private void face(Vector3 direction)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        if (Vector3.Angle(direction, transform.forward) < 1.0f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    // Setter for powerup state
    public bool getPowerup()
    {
        return powerup;
    }

}
