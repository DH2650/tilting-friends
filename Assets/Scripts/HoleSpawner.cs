using UnityEngine;

public class HoleSpawner : MonoBehaviour
{
    [Header("Assign these in Inspector")]
    public GameObject holePrefab;      // Your hole prefab
    public GameObject board;           // The board GameObject
    public GameObject ball;            // Reference to the ball

    [Header("Hole Spawn Area")]
    public Vector2 spawnAreaMin = new Vector2(-4f, -4f); // Min x/z on board
    public Vector2 spawnAreaMax = new Vector2(4f, 4f);   // Max x/z on board
    public float minDistanceFromBall = 2.5f;             // Minimum distance from ball

    void Start()
    {
        SpawnHole();
    }

    void SpawnHole()
    {
        if (holePrefab == null || board == null)
        {
            Debug.LogError("HolePrefab or Board is not assigned!");
            return;
        }

        Vector3 ballPosition = Vector3.zero;
        if (ball != null)
        {
            // Convert ball's world position to board's local space
            ballPosition = board.transform.InverseTransformPoint(ball.transform.position);
        }

        // Try up to 10 times to find a valid position
        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Get random position within spawn area
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomZ = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            spawnPosition = new Vector3(randomX, 0.3f, randomZ);

            // Check distance from ball (in local space)
            float distanceFromBall = Vector2.Distance(
                new Vector2(spawnPosition.x, spawnPosition.z),
                new Vector2(ballPosition.x, ballPosition.z)
            );

            if (distanceFromBall >= minDistanceFromBall)
            {
                validPositionFound = true;
                break;
            }
        }

        // If we couldn't find a valid position, use the last attempted one
        // Spawn the hole and parent it to the board
        GameObject hole = Instantiate(holePrefab, board.transform);
        hole.transform.localPosition = spawnPosition;
        hole.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        Debug.Log("Hole spawned at local position: " + spawnPosition);
    }
}
