using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Vector3 startPosition; // Stores the initial position of the ball
    private Transform parentBoard; 
    [SerializeField] HoleSpawner holeSpawner;
    [SerializeField] VSGameSystem gameSystem;

    void Start()
    {
        startPosition = transform.position; // Save the starting position
        parentBoard = transform.parent;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hole")) // Check if the ball hits a hole
        {
            Debug.Log("Ball fell into the hole!");
            Destroy(other.gameObject);
            ResetBall(); // Reset ball position
        }
    }

    void ResetBall()
    {
        transform.position = startPosition; // Move the ball back to start position
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        Transform woodboard = parentBoard.Find("woodboard");
        Transform cubeObj = woodboard.Find("Cube");
        holeSpawner.SpawnHole(cubeObj.gameObject);
    }

}
