using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardsTilting : MonoBehaviour
{
    public float smooth = 5.0f;
    public float rotateAngle = 30.0f;
    public float platformRadius = 10;
    public float tiltCutoffY = -8;
    [SerializeField] DebugOverlay debug;
    [SerializeField] Rigidbody board;

    public bool singlePlayerMode = true;
    private Rigidbody rb1;
    private Rigidbody rb2;
    //private Rigidbody arrow;

    //Quaternion ogArrowRotation;

    void Start()
    {
        // Get players with their rigid bodies
        GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");
        if (player1 != null)
            rb1 = player1.GetComponent<Rigidbody>();
        if (player2 != null)
            rb2 = player2.GetComponent<Rigidbody>();

        //GameObject arrowObj = GameObject.FindGameObjectWithTag("Arrow");
        //if (arrowObj != null)
        //{
        //    arrow = arrowObj.GetComponent<Rigidbody>();
        //    if (arrow != null)
        //        ogArrowRotation = arrow.transform.rotation;
        //}
    }

    void Update()
    {
        // Track active players
        int activePlayers = 0;
        Vector3 norm = Vector3.zero;

        // Check Player 1
        if (rb1 != null && rb1.transform.position.y > tiltCutoffY)
        {
            norm += rb1.transform.position;
            activePlayers++;
        }

        // Check Player 2
        if (rb2 != null && rb2.transform.position.y > tiltCutoffY)
        {
            norm += rb2.transform.position;
            activePlayers++;
        }

        // Don't tilt if no players are active
        if (activePlayers == 0) return;

        // Average the positions if multiple players
        if (activePlayers > 1)
        {
            norm /= activePlayers;
        }

        // Normalize positions relative to platform
        norm.x = Mathf.Clamp(norm.x, -platformRadius, platformRadius) / platformRadius;
        norm.z = Mathf.Clamp(norm.z, -platformRadius, platformRadius) / platformRadius;

        // Calculate rotation direction
        Vector3 direction = norm * rotateAngle;

        // Update debug text
        debug.debugText.text = $"Active Players: {activePlayers}\n" +
                              $"Norm: {norm}\nDir: {direction}";

        // Calculate and apply rotation
        Quaternion bRotation = Quaternion.Euler(direction.z, 0.0f, -direction.x);
        transform.rotation = Quaternion.Slerp(transform.rotation, bRotation, Time.deltaTime * smooth);
    }
    


}