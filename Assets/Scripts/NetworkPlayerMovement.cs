using UnityEngine;

public class NetworkPlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private string _playerId;
    private string state = "none";

    // Optional: Call this after instantiating the player
    public void Initialize(string playerId)
    {
        _playerId = playerId;
        gameObject.name = "Player_" + playerId; // For easy identification in Hierarchy
        Debug.Log($"Init player");
    }

    public void ProcessInput(string inputType)
    {
        if (inputType.Contains("pressed"))
        {
            state = inputType;
        }
        else
        {
            state = "none";
        }
    }

    void Update()
    {
        if (state != "none") {
            Debug.Log($"Move player");
            switch (state)
            {
                case "up_pressed":
                    // Start moving up or set a flag
                    transform.Translate(Vector3.back * speed * Time.deltaTime); // Simple example
                    break;
                case "down_pressed":
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    break;
                // ... handle other inputs (left, right, actionA_pressed, etc.)
                case "left_pressed":
                    transform.Translate(Vector3.right * speed * Time.deltaTime);
                    break;
                case "right_pressed":
                    transform.Translate(Vector3.left * speed * Time.deltaTime);
                    break;
                case "actionA_pressed":
                    transform.Translate(Vector3.up * speed * Time.deltaTime); // Simple example
                    // Perform action
                    break;
            }
        }
    }
}