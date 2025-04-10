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

    private float max = 10;

    private Rigidbody rb1;
    private Rigidbody rb2;

    void Start()
    {
        // Get players with their rigid bodies
        GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
        if (player1 != null)
            rb1 = player1.GetComponent<Rigidbody>();
        if (player2 != null)
            rb2 = player2.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get positions of both players
        Vector3 norm = rb1.transform.position + rb2.transform.position;

        // Normalize x according to max distance from the centre
        if (norm.x > max) {
            norm.x = max;
        }
        else if (norm.x < -max) {
            norm.x = -max;
        }
        norm.x /= max;

        // Normalize z according to max distance from the centre
        if (norm.z > max) {
            norm.z = max;
        }
        else if (norm.z < -max) {
            norm.z = -max;
        }
        norm.z /= max;

        // Translate to direction with rotateAngle
        Vector3 direction = norm * rotateAngle;

        // Output debug info
        debug.debugText.text = $"P1 Pos: {rb1.transform.position}\nP2 Pos: {rb2.transform.position}\nNorm: {norm}\nDir: {direction}";

        // Convert the rotation angles into quaternions
        Quaternion bRotation = Quaternion.Euler(direction.z, 0.0f, -direction.x);

        // Rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime*smooth);
    }
}