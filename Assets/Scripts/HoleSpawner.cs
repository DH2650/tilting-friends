using UnityEngine;

public class HoleSpawner : MonoBehaviour
{
    [Header("Assign these in Inspector")]
    public GameObject holePrefab;      // Your hole prefab
    public GameObject board1;           // The board GameObject

    [Header("Hole Spawn Area")]
    public Vector2 spawnAreaMin = new Vector2(-4f, -4f); // Min x/z on board
    public Vector2 spawnAreaMax = new Vector2(4f, 4f);   // Max x/z on board


    public void SpawnHole(GameObject board)
    {
        if (holePrefab == null || board1 == null)
        {
            Debug.LogError("HolePrefab or Board is not assigned!");
            return;
        }

        // Get random position within spawn area
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomZ = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        Vector3 spawnPosition = new Vector3(randomX, 0.3f, randomZ);

        // Spawn the hole and parent it to the board
        GameObject hole = Instantiate(holePrefab, board.transform);
        hole.transform.localPosition = spawnPosition;
        hole.transform.localRotation = Quaternion.Euler(90f,0f,0f);
        
        Debug.Log("Hole spawned at local position: " + spawnPosition);
    }
}