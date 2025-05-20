using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VSBoard : MonoBehaviour
{
    public float smooth = 5.0f;
    public float rotateAngle = 60.0f;
    public float platformRadius = 10;
    public float tiltCutoffY = -8;
    public float secondCutoff = -300;
    public Vector3 respawnPosition = new Vector3(0, 5.0f, 0);
    private bool isBoard1 = true;
    private bool isBoard2 = false;
    private bool VSStarted = false;

    [SerializeField] DebugOverlay debug;
    [SerializeField] Rigidbody board;
    [SerializeField] GameObject player;

    public GameObject networkManagerObject;
    public Dictionary<string, GameObject> players;


    private Rigidbody rb1;
    //private Rigidbody rb2;
    private Rigidbody arrow;
    private Vector3 norm = Vector3.zero;

    Quaternion ogArrowRotation;

    void Start()
    {
        networkManagerObject = GameObject.FindGameObjectWithTag("NetworkManager");
        if (networkManagerObject != null)
        {
            NetworkManager networkManager = networkManagerObject.GetComponent<NetworkManager>();
            players = networkManager.players;
            Debug.Log("players:" + players);
        }
    }

    void Update()
    {
        if (VSStarted)
        {
            Debug.Log("board tag: " + board.tag);
            // Get positions of both players
            getPlayers();
            if (norm.x > platformRadius)
            {
                norm.x = platformRadius;
            }
            else if (norm.x < -platformRadius)
            {
                norm.x = -platformRadius;
            }
            norm.x /= platformRadius;

            // Normalize z according to platformRadius distance from the centre
            if (norm.z > platformRadius)
            {
                norm.z = platformRadius;
            }
            else if (norm.z < -platformRadius)
            {
                norm.z = -platformRadius;
            }
            norm.z /= platformRadius;

            // Translate to direction with rotateAngle
            Vector3 direction = norm * rotateAngle;


            // Convert the rotation angles into quaternions
            Quaternion bRotation = Quaternion.Euler(direction.z, 0.0f, -direction.x);

            // Rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime * smooth);
        }
        
    }

    void getPlayers()
    {
        foreach (var (controllerId, player) in players)
        {
            if (board.tag == "Board1")
            {
                if (isBoard1)
                {
                    Debug.Log("board 1");
                    getMovement(player);
                    isBoard1 = false;
                }
                else
                {
                    isBoard1 = true;
                }
            }
            if (board.tag == "Board2")
            {
                if (isBoard2)
                {
                    getMovement(player);
                    isBoard2 = false;
                }
                else
                {
                    isBoard2 = true;
                }
            }
        }
    }

    void getMovement(GameObject player)
    {
        NetworkPlayerMovement ps = player.GetComponent<NetworkPlayerMovement>();
        Rigidbody rb = ps.rb;
        Vector3 localPos = transform.InverseTransformPoint(rb.transform.position);
        if (rb.transform.position.y > tiltCutoffY)
        {
            norm = localPos;
        }
        if (rb.transform.position.y < secondCutoff)
        {
            Respawn(rb, board);
        }
    }

    void Respawn(Rigidbody player, Rigidbody board)
    {
        player.linearVelocity = Vector3.zero;
        player.rotation = Quaternion.identity;
        player.angularVelocity = Vector3.zero;
        player.transform.position = board.transform.position + respawnPosition;
    }

    public void clickStart()
    {
        if (players.Count < 2)
        {
            Debug.Log("Not enough players to launch");
        }
        else
        {
            VSStarted = true;
        }
    }
}
