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

    // Start is called before the first frame update
    void Start()
    {
        GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
        if (player1 != null)
            rb1 = player1.GetComponent<Rigidbody>();
        if (player2 != null)
            rb2 = player2.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // gets input from user
        float xRotation = Input.GetAxis("Horizontal") * rotateAngle;
        float zRotation = Input.GetAxis("Vertical") * rotateAngle;

        Vector3 norm = rb1.transform.position;
        if (norm.x > max) {
            norm.x = max;
        }
        else if (norm.x < -max) {
            norm.x = -max;
        }
        norm.x /= max;


        if (norm.z > max) {
            norm.z = max;
        }
        else if (norm.z < -max) {
            norm.z = -max;
        }
        norm.z /= max;

        Vector3 direction = norm * rotateAngle;
//         body.AddForceAtPosition(direction.normalized, transform.position);

        debug.debugText.text = $"xRot: {xRotation}\nzRot: {zRotation}\nP1 Pos: {rb1.transform.position} Norm: {norm} Dir: {direction}";

//         // convert the rotation angles into quarternions.
        Quaternion bRotation = Quaternion.Euler(direction.z, 0.0f, -direction.x);
// //         // rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime*smooth);
    }
}