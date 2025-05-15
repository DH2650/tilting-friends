using TMPro;
using UnityEngine;

public class TwoPlayerController : MonoBehaviour
{
    public static TwoPlayerController Instance { get; private set; }
    //void Awake() => Instance = this;

    public float moveSpeed = 10f; // Increased for better responsiveness
    public float rotationSpeed = 10f;
    private Rigidbody[] bodies = new Rigidbody[2];
    private Vector3[] inputs = new Vector3[2];  // latest tilt per player
    private bool[] activePlayers = new bool[2];
    // Debug visualization
    public bool showDebugInfo = true;
    public TextMeshProUGUI debugText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("TwoPlayerController singleton initialized");
    }

    void Start()
    {
        // Find player objects
        bodies[0] = GameObject.FindWithTag("Player1")?.GetComponent<Rigidbody>();
        bodies[1] = GameObject.FindWithTag("Player2")?.GetComponent<Rigidbody>();

        // Initialize all as inactive
        activePlayers[0] = false;
        activePlayers[1] = false;

        // Mark as active ONLY if both Rigidbody exists AND a client is connected for that index
        if (JoinService.ConnectedClients != null)
        {
            foreach (var client in JoinService.ConnectedClients)
            {
                int idx = client.PlayerIndex;
                if (idx >= 0 && idx < activePlayers.Length && bodies[idx] != null)
                {
                    activePlayers[idx] = true;
                }
            }
        }

        // Disable inactive player objects if needed
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] != null && !activePlayers[i])
            {
                bodies[i].gameObject.SetActive(false);
            }
        }

        Debug.Log($"Game started with {JoinService.ConnectedClients.Count} player(s)");
        Debug.Log($"Player 1 active: {activePlayers[0]}, Player 2 active: {activePlayers[1]}");

        // Start the movement test
        StartCoroutine(TestMovement());
    }


    public void SetInput(int playerIndex, Vector3 dir)
    {
        Debug.Log($"SetInput called: player={playerIndex}, dir={dir}");
        if (playerIndex >= 0 && playerIndex < inputs.Length && activePlayers[playerIndex])
        {
            inputs[playerIndex] = new Vector3(dir.x, 0, dir.z);
            Debug.Log($"Applied input to player {playerIndex}: {inputs[playerIndex]}");

            if (showDebugInfo && debugText != null)
                debugText.text = $"Player {playerIndex + 1} input: {inputs[playerIndex]}";
        }
    }


    void FixedUpdate()
    {
        // Test keyboard input for debugging
        if (Input.GetKey(KeyCode.W)) inputs[0] = new Vector3(0, 0, 1);
        else if (Input.GetKey(KeyCode.S)) inputs[0] = new Vector3(0, 0, -1);
        else if (Input.GetKey(KeyCode.A)) inputs[0] = new Vector3(-1, 0, 0);
        else if (Input.GetKey(KeyCode.D)) inputs[0] = new Vector3(1, 0, 0);
        else if (!Input.anyKey && inputs[0].magnitude < 0.1f) inputs[0] = Vector3.zero;

        // Process movement for both players
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] != null)
                Drive(bodies[i], inputs[i]);
        }
    }

    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Player 1 Input: {inputs[0]}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Player 2 Input: {inputs[1]}");

            if (bodies[0] != null)
                GUI.Label(new Rect(10, 50, 300, 20), $"Player 1 Velocity: {bodies[0].linearVelocity.magnitude:F2}");
            if (bodies[1] != null)
                GUI.Label(new Rect(10, 70, 300, 20), $"Player 2 Velocity: {bodies[1].linearVelocity.magnitude:F2}");
        }
    }

    void Drive(Rigidbody rb, Vector3 moveDir)
    {
        if (rb == null) return;
        //Debug.Log($"Drive called with moveDir={moveDir}, sqrMagnitude={moveDir.sqrMagnitude}");
        // Apply movement if there's significant input
        if (moveDir.sqrMagnitude > 0.1f)
        {
            // Normalize if magnitude is greater than 1
            if (moveDir.sqrMagnitude > 1) moveDir.Normalize();

            // Option 1: Force-based movement (more physics-based)
            Debug.Log($"Applying force: {moveDir * moveSpeed}");
            //rb.AddForce(moveDir * moveSpeed, ForceMode.Acceleration);
            rb.linearVelocity = moveDir * moveSpeed;
            // Option 2: Direct velocity control (more responsive)
            // Uncomment this and comment the AddForce line above if you need more direct control
            // rb.velocity = moveDir * moveSpeed;

            // Rotate to face movement direction
            if (moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            // Add some drag when not actively moving
            rb.linearVelocity *= 0.95f;
        }
    }

    private System.Collections.IEnumerator TestMovement()
    {
        Debug.Log("Starting automatic movement test in 3 seconds...");
        yield return new WaitForSeconds(3);

        // Test forward movement
        Debug.Log("Testing FORWARD movement");
        SetInput(0, new Vector3(0, 0, 1));
        yield return new WaitForSeconds(1);

        // Test stop
        Debug.Log("Testing STOP");
        SetInput(0, Vector3.zero);
        yield return new WaitForSeconds(1);

        // Test right movement
        Debug.Log("Testing RIGHT movement");
        SetInput(0, new Vector3(1, 0, 0));
        yield return new WaitForSeconds(1);

        // Final stop
        SetInput(0, Vector3.zero);
        Debug.Log("Movement test complete");
    }


}
