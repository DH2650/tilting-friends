using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Vector3 startPosition; // Stores the initial position of the ball

    void Start()
    {
        startPosition = transform.position; // Save the starting position
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hole")) // Check if the ball hits a hole
        {
            Debug.Log("Ball fell into the hole!");
            ResetBall(); // Reset ball position
        }
    }

    void ResetBall()
    {
        transform.position = startPosition; // Move the ball back to start position
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

    }
}
