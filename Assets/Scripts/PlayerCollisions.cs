using UnityEngine;
using System.Collections;

public class PlayerCollisions: MonoBehaviour
{
    public bool touchBoard;
    public bool alive;
    public int timeout;
    public int timeoutMax;

    void Start()
    {
        alive = true;
        timeout = 0;
        timeoutMax = 200;
    }

    private void OnTriggerEnter(Collider other)
    {
        touchBoard = true;
        print("ENTER");
    }

    private void OnTriggerExit(Collider other)
    {
        touchBoard = false;
        print("EXIT");
    }
}
