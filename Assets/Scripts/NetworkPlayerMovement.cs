using UnityEngine;

public class NetworkPlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private string _playerId;

    // Optional: Call this after instantiating the player
    public void Initialize(string playerId)
    {
        _playerId = playerId;
        gameObject.name = "Player_" + playerId; // For easy identification in Hierarchy
        Debug.Log($"Spawn player");
    }

    public void ProcessInput(string inputType)
    {
        switch (inputType)
        {
            case "up_pressed":
                // Start moving up or set a flag
                transform.Translate(Vector3.up * speed * Time.deltaTime); // Simple example
                break;
            case "up_released":
                // Stop moving up
                break;
            case "down_pressed":
                transform.Translate(Vector3.down * speed * Time.deltaTime);
                break;
            // ... handle other inputs (left, right, actionA_pressed, etc.)
            case "left_pressed":
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                break;
            case "right_pressed":
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                break;
            case "actionA_pressed":
                Debug.Log($"Player {_playerId} pressed Action A!");
                // Perform action
                break;
        }
    }
}