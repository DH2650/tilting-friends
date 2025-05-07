using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board: MonoBehaviour
{
    public float smooth = 5.0f;
    public float rotateAngle = 30.0f;
    public float platformRadius = 10;
    public float tiltCutoffY = -8;

    [SerializeField] DebugOverlay debug;
    [SerializeField] Rigidbody board;


    private Rigidbody rb1;
    private Rigidbody rb2;
    private Rigidbody arrow;

    Quaternion ogArrowRotation;

    void Start()
    {
        // Get players with their rigid bodies
        GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
        if (player1 != null)
            rb1 = player1.GetComponent<Rigidbody>();
        if (player2 != null)
            rb2 = player2.GetComponent<Rigidbody>();

        GameObject arrowObj = GameObject.FindGameObjectWithTag("Arrow");
        if (arrowObj != null)
            arrow = arrowObj.GetComponent<Rigidbody>();
            //ogArrowRotation = arrow.transform.rotation;
    }

    void Update()
    {
        // Get positions of both players
        Vector3 norm = Vector3.zero;
        if (rb1.transform.position.y > tiltCutoffY)
            norm = rb1.transform.position;

        if (rb2.transform.position.y > tiltCutoffY)
            norm = norm + rb2.transform.position;

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
        debug.debugText.text = $"P1 Pos: {rb1.transform.position}\nP2 Pos: {rb2.transform.position}\nNorm: {norm}\nDir: {direction}";

        // Convert the rotation angles into quaternions
        Quaternion bRotation = Quaternion.Euler(direction.z, 0.0f, -direction.x);

        // Rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime*smooth);
        //arrow.transform.rotation = ogArrowRotation * transform.rotation;
    }
}
