using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board: MonoBehaviour
{
    public float smooth = 5.0f;
    public float rotateAngle = 30.0f;
    [SerializeField] DebugOverlay debug;
    [SerializeField] Rigidbody board;
    [SerializeField] PlayerCollisions p1Touch;
    [SerializeField] PlayerCollisions p2Touch;

    private float max = 10;

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
            ogArrowRotation = arrow.transform.rotation;
    }

    void Update()
    {
        if (p1Touch.alive || p2Touch.alive)
        {
            // Get positions of both players
            Vector3 norm = Vector3.zero;
            if (p1Touch.touchBoard)
            {
                p1Touch.alive = true;
                p1Touch.timeout = 0;
                norm = rb1.transform.position;
            } else if (p1Touch.alive)
            {
                norm = rb1.transform.position;
                if (p1Touch.timeout < p1Touch.timeoutMax)
                {
                    p1Touch.timeout++;
                } else
                {
                    p1Touch.alive = false;
                }
            }

            if (p2Touch.touchBoard)
            {
                p2Touch.alive = true;
                p2Touch.timeout = 0;
                norm = norm + rb2.transform.position;
            } else if (p2Touch.alive)
            {
                norm = norm + rb2.transform.position;
                if (p2Touch.timeout < p2Touch.timeoutMax)
                {
                    p2Touch.timeout++;
                } else
                {
                    p2Touch.alive = false;
                }
            }

            // Normalize x according to max distance from the centre
            if (norm.x > max)
            {
                norm.x = max;
            }
            else if (norm.x < -max)
            {
                norm.x = -max;
            }
            norm.x /= max;

            // Normalize z according to max distance from the centre
            if (norm.z > max)
            {
                norm.z = max;
            }
            else if (norm.z < -max)
            {
                norm.z = -max;
            }
            norm.z /= max;

            // Translate to direction with rotateAngle
            Vector3 direction = norm * rotateAngle;

            // Convert the rotation angles into quaternions
            Quaternion bRotation = Quaternion.Euler(direction.z, 0.0f, -direction.x);

            // Rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime*smooth);
            arrow.transform.rotation = ogArrowRotation * transform.rotation;

            // Output debug info
            debug.debugText.text = $@"P1 Pos: {rb1.transform.position}
P2 Pos: {rb2.transform.position}
Norm: {norm}\nDir: {direction}
P1: Touch: {p1Touch.touchBoard} alive: {p1Touch.alive} timeout: {p1Touch.timeout}
P2: Touch: {p2Touch.touchBoard} alive: {p2Touch.alive} timeout: {p2Touch.timeout}";

        } else
        {
            // Convert the rotation angles into quaternions
            Quaternion bRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            // Rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime*smooth);
            arrow.transform.rotation = ogArrowRotation * transform.rotation;
        }
    }
}