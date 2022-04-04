using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieController : MonoBehaviour
{

    public GameObject node1;
    public GameObject node2;

    private CharacterController controller;

    [SerializeField] private float speed = 1.0f;
    private Vector3 velocity;
    private Vector3 direction;

    private List<Vector3> nodePositions;
    private int index = 0;
    private const float rn = 1.0f;

    private Vector3 rayOffset = new Vector3(0.0f, .25f, 0.0f);
    private const float rp = 5.0f;

    PhotonView view;
    private List<GameObject> players = new List<GameObject>();
    private bool toFire = true;
    private float fireTime = 5.0f;

    // Move ment is based on two nodes. They walk back and forth between. Here they are initialized within a list.
    void Start()
    {
        view = GetComponent<PhotonView>();
        nodePositions = new List<Vector3>();
        nodePositions.Add(node1.transform.position);
        nodePositions.Add(node2.transform.position);
        controller = GetComponent<CharacterController>();
    }

    // Movement is decided on whether the zombies can see the players. If there's a player, they turn to face and shoot.
    void Update()
    {
        
        //lock y-axis
        controller.enabled = false;
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        controller.enabled = true;

        if (view.IsMine)
        {

            if(!seePlayer())
            {
                switchIndex();
                getDirectionNode();
                face();
                Move();
            }
            else{
                face();

                if (Vector3.Angle(direction, transform.forward) < 3.0f && toFire)
                {
                    gameObject.GetComponent<CannonBallZombie>().fire(true);
                    toFire = false;
                    StartCoroutine(fireLength());
                }
                    

            }

        }
      
    }

    // seeing the players is handled by ray tracing. It's preferred on fixedUpdate.
    private void FixedUpdate()
    {
        seePlayer(); 
    }

    // Simple movement constrained by angle.
    private void Move()
    {
        if (Vector3.Angle(transform.forward, direction) < .5f)
        {
            velocity = transform.forward * speed;
            controller.Move(velocity * Time.deltaTime);
        }

    }

    // Pause for at least 5 seconds even if the cannonball hit before 5 seconds are over
    IEnumerator fireLength()
    {
        float normTime = 0.0f;
        while (normTime <= 1.0f)
        {
            normTime += Time.deltaTime / fireTime;
            yield return null;
        }

        toFire = true;

    }

    // Constant ray trace for zombies to be able to detect player movement.
    private bool seePlayer()
    {
        RaycastHit hit;

        float d = 100;
        bool saw = false;
        //Debug.DrawRay(transform.position + rayOffset, transform.forward, Color.red, 10.0f);
        
        if(players.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            foreach(GameObject GO in players)
            {

                //Debug.DrawRay(transform.position + rayOffset, 100*(GO.transform.position - transform.position).normalized, Color.red, 10.0f);
                try
                {
                    if (Physics.SphereCast(transform.position + rayOffset, .00001f, (GO.transform.position + rayOffset - transform.position + rayOffset).normalized, out hit, 24.0f) && (hit.transform.gameObject.transform.parent.gameObject == GO) && (transform.position - hit.point).magnitude < rp)
                    {
                        saw = true;
                        if((transform.position - hit.point).magnitude < d)
                        {
                            d = (transform.position - hit.point).magnitude;
                            direction = ((GO.transform.position - transform.position)).normalized;
                        }
                    
                    }
                }catch(System.NullReferenceException e) { }
            }
        }
        else if (players.Count > PhotonNetwork.CurrentRoom.PlayerCount)
        {
            players.RemoveAt(players.Count - 1);
        }

        return saw;
    }

    // Movement is designated by the node objects
    private void getDirectionNode()
    {
        direction = (nodePositions[index] - gameObject.transform.position).normalized;
    }

    // When a node is reached, switch to the other
    private void switchIndex()
    {
        
        if (index == 0 && (gameObject.transform.position - nodePositions[index]).magnitude <= rn)
            index = 1;
        else if (index == 1 && (gameObject.transform.position - nodePositions[index]).magnitude <= rn)
            index = 0;
    }

    // Face in the direction of rotation
    private void face()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 2f);

        if (Vector3.Angle(direction, transform.forward) < 3.0f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    // Keep player identification
    public void playerGOs(int ID)
    {
        players.Add(PhotonView.Find(ID).gameObject);
    }

}
