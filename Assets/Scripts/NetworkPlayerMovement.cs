using UnityEngine;

public class NetworkPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    public string name;
    public Vector3 moveDir = Vector3.zero;
    public Rigidbody rb;
    public int moving = 0;

    // Optional: Call this after instantiating the player
    public void Initialize(Rigidbody body, string playerName)
    {
        name = playerName;
        rb = body;

        Debug.Log($"Init player");
    }

    public void ProcessInput(string inputType)
    {
        switch (inputType)
        {
            case "up_pressed":
                moveDir.z += 1;
                moving += 1;
                break;
            case "up_released":
                moveDir.z -= 1;
                moving -= 1;
                break;
            case "down_pressed":
                moveDir.z -= 1;
                moving += 1;
                break;
            case "down_released":
                moveDir.z += 1;
                moving -= 1;
                break;
            case "left_pressed":
                moveDir.x -= 1;
                moving += 1;
                break;
            case "left_released":
                moveDir.x += 1;
                moving -= 1;
                break;
            case "right_pressed":
                moveDir.x += 1;
                moving += 1;
                break;
            case "right_released":
                moveDir.x -= 1;
                moving -= 1;
                break;
        }
    }

    void FixedUpdate()
    {
//         if (rb == null) return;

        if (moveDir != Vector3.zero && moving != 0)
        {
            Vector3 moveDirNormalized = moveDir.normalized;

            // Move the character
            rb.MovePosition(rb.position + moveDirNormalized * moveSpeed * Time.fixedDeltaTime);

            // Smooth rotation towards movement direction
            Quaternion toRotation = Quaternion.LookRotation(-moveDirNormalized, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime));
        } else
        {
            moveDir = Vector3.zero;
        }
    }
}