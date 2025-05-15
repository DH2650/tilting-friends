using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board: MonoBehaviour
{
    public float smooth = 5.0f;
    public float rotateAngle = 30.0f;
    public float platformRadius = 10;
    public float tiltCutoffY = -7;
    public float secondCutoff = -300;
    public Vector3 respawnPosition = new Vector3(0, 5.0f, 0);

    [SerializeField] DebugOverlay debug;
    [SerializeField] Rigidbody board;


    private Rigidbody rb1;
    private Rigidbody rb2;
    private Rigidbody arrow;

    public GameObject networkManagerObject;
    public Dictionary<string, GameObject> players;

    Quaternion ogArrowRotation;

    void Start()
    {

//         GameObject arrowObj = GameObject.FindGameObjectWithTag("Arrow");
//         if (arrowObj != null)
//             arrow = arrowObj.GetComponent<Rigidbody>();
//             //ogArrowRotation = arrow.transform.rotation;

        networkManagerObject = GameObject.FindGameObjectWithTag("NetworkManager");
        if (networkManagerObject != null)
        {
            NetworkManager networkManager = networkManagerObject.GetComponent<NetworkManager>();
            players = networkManager.players;
        }
    }

    void updatePlayerDebugInfo() {
        string msg = "";

        foreach(var (controllerId, player) in players)
        {
            NetworkPlayerMovement ps = player.GetComponent<NetworkPlayerMovement>();

            msg += $"Name: {ps.name} - Moving: {ps.moving} - Pos: {ps.rb.transform.position}\n";
        }
        debug.debugText.text = msg;
    }

    void Update()
    {
        updatePlayerDebugInfo();
//         // Get positions of both players
        Vector3 norm = Vector3.zero;

        foreach(var (controllerId, player) in players)
        {
            NetworkPlayerMovement ps = player.GetComponent<NetworkPlayerMovement>();
            Rigidbody rb = ps.rb;
            if (rb.transform.position.y > tiltCutoffY){
                norm = rb.transform.position;
//             Debug.Log($"Player transform: {rb.transform.position}");
            }
            if (rb.transform.position.y < secondCutoff){
                Respawn(rb);
            }
                
        }

        // Normalize x according to platformRadius distance from the centre
        if (norm.x > platformRadius) {
            norm.x = platformRadius;
        }
        else if (norm.x < -platformRadius) {
            norm.x = -platformRadius;
        }
        norm.x /= platformRadius;

        // Normalize z according to platformRadius distance from the centre
        if (norm.z > platformRadius) {
            norm.z = platformRadius;
        }
        else if (norm.z < -platformRadius) {
            norm.z = -platformRadius;
        }
        norm.z /= platformRadius;

        // Translate to direction with rotateAngle
        Vector3 direction = norm * rotateAngle;

        // Output debug info
        debug.debugText.text = $"Norm: {norm}\nDir: {direction}";

        // Convert the rotation angles into quaternions
        Quaternion bRotation = Quaternion.Euler(direction.z, 0.0f, -direction.x);

        // Rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime*smooth);
//         arrow.transform.rotation = ogArrowRotation * transform.rotation;
    }

    void Respawn(Rigidbody player){
        player.linearVelocity = Vector3.zero;
        player.rotation = Quaternion.identity;
        player.angularVelocity = Vector3.zero;
        player.transform.position = respawnPosition;
        
    }
}
