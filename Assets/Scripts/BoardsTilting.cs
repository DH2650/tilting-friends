using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class board: MonoBehaviour
{
    public float smooth = 5.0f;
    public float rotateAngle = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");

//         if (player1 != null)
//             rb1 = player1.GetComponent<Rigidbody>();
//         if (player2 != null)
//             rb2 = player2.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // gets input from user
        float xRotation = Input.GetAxis("Horizontal")*rotateAngle;
        float zRotation = Input.GetAxis("Vertical")*rotateAngle;
        // convert the rotation angles into quarternions.
        Quaternion bRotation = Quaternion.Euler(xRotation, 0.0f, zRotation);
        //rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime*smooth);
    }
}