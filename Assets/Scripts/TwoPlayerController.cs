using UnityEngine;

public class TwoPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb1;
    private Rigidbody rb2;

    void Start()
    {
        GameObject player1 = GameObject.FindGameObjectWithTag("Player1");
        GameObject player2 = GameObject.FindGameObjectWithTag("Player2");

        if (player1 != null)
            rb1 = player1.GetComponent<Rigidbody>();
        if (player2 != null)
            rb2 = player2.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandlePlayerMovement(rb1, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D);
        HandlePlayerMovement(rb2, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
    }

    void HandlePlayerMovement(Rigidbody rb, KeyCode up, KeyCode down, KeyCode left, KeyCode right)
    {
        if (rb == null) return;

        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(up)) moveDir.z += 1;
        if (Input.GetKey(down)) moveDir.z -= 1;
        if (Input.GetKey(left)) moveDir.x -= 1;
        if (Input.GetKey(right)) moveDir.x += 1;

        moveDir = moveDir.normalized;

        if (moveDir != Vector3.zero)
        {
            // Move the character
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            // Smooth rotation towards movement direction
            Quaternion toRotation = Quaternion.LookRotation(-moveDir, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }
} 