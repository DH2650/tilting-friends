using UnityEngine;
// Add the namespace for your WebSocket library (e.g., using SocketIOClient;)
// Example using a hypothetical SocketIOUnity wrapper:
using SocketIOClient; // This will depend on the library you choose
using SocketIOClient.Newtonsoft.Json; // If using Newtonsoft for JSON
using Newtonsoft.Json.Linq; // Required for JToken
using System.Collections.Generic;
// using System; // No longer needed for Action if queue is removed

public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager Instance { get; private set; }

    private SocketIOUnity socket; // Example, replace with actual type

    public string serverURL = "ws://localhost:3000";
    public Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    public GameObject playerPrefab; // Assign your player prefab in the Inspector

    // No more _mainThreadActions queue

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ConnectToServer();
    }

    // No more Update() method for queue processing, unless you need it for other logic.
    // If your library doesn't run callbacks on the main thread, you might re-introduce
    // Update() to poll flags set by callbacks and then execute Unity API calls here.

    void ConnectToServer()
    {
        var uri = new System.Uri(serverURL);
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>(), // Add any necessary query params
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            EIO = 4
        });

        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        socket.OnConnected += (sender, e) =>
        {
            // Assuming this callback is on the main thread due to library behavior OR
            // we are only doing thread-safe operations here. Debug.Log is usually safe.
            Debug.Log("Unity connected to server!");
            socket.Emit("registerUnityGame");
        };

        socket.OnDisconnected += (sender, reason) =>
        {
            Debug.Log("Unity disconnected from server. Reason: " + reason);
            // If UI needs update, this MUST be on main thread.
        };

        socket.OnError += (sender, e_string) =>
        {
            Debug.LogError("Socket Error: " + e_string);
        };

        socket.On("controllerConnected", (response) =>
        {
            // CRITICAL: The following Unity API calls (Instantiate, Add to Dictionary)
            // MUST execute on the main thread.
            try
            {
                // Attempt to get data without custom class deserialization.
                // This assumes the server sends a JSON object as the first (or only) argument.
                // response.GetValue<JToken>() might get the first JToken in the response payload.
                JToken dataToken = response.GetValue<JToken>(); // Or response.GetValue(0) if your lib supports index
                string playerId = dataToken?.Value<string>("controllerId"); // Match key from server

                if (string.IsNullOrEmpty(playerId))
                {
                    Debug.LogError("Received invalid or null playerId for controllerConnected.");
                    // Consider how your specific library handles response.ToString() for logging
                    Debug.Log($"Raw response: {response.ToString()}");
                    return;
                }

                Debug.Log("Controller connected (player joined): " + playerId);
                if (!players.ContainsKey(playerId) && playerPrefab != null)
                {
                    GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                    // newPlayer.name = "Player_" + playerId; // Example: set name for easier debugging
                    // Example: PlayerScript ps = newPlayer.GetComponent<PlayerScript>();
                    // if (ps != null) ps.Initialize(playerId);
                    players.Add(playerId, newPlayer);
                    Debug.Log("Spawned player: " + playerId);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error processing controllerConnected event: {ex.Message}. Ensure this runs on the main thread if using Unity API.");
                Debug.Log($"Raw response: {response.ToString()}");
            }
        });

        socket.On("controllerDisconnected", (response) =>
        {
            // CRITICAL: The following Unity API calls (Destroy, Remove from Dictionary)
            // MUST execute on the main thread.
            try
            {
                JToken dataToken = response.GetValue<JToken>();
                string playerId = dataToken?.Value<string>("controllerId"); // Match key from server

                if (string.IsNullOrEmpty(playerId))
                {
                    Debug.LogError("Received invalid or null playerId for controllerDisconnected.");
                    Debug.Log($"Raw response: {response.ToString()}");
                    return;
                }

                Debug.Log("Controller disconnected (player left): " + playerId);
                if (players.TryGetValue(playerId, out GameObject playerObject))
                {
                    Destroy(playerObject);
                    players.Remove(playerId);
                    Debug.Log("Destroyed player: " + playerId);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error processing controllerDisconnected event: {ex.Message}. Ensure this runs on the main thread if using Unity API.");
                Debug.Log($"Raw response: {response.ToString()}");
            }
        });

        socket.On("inputFromController", (response) =>
        {
            // CRITICAL: Any interaction with player GameObjects MUST execute on the main thread.
            try
            {
                JToken dataToken = response.GetValue<JToken>();
                string playerId = dataToken?.Value<string>("controllerId"); // Match key from server
                // Assuming 'input' is also part of the same JSON object.
                // If 'input' is a complex object itself, dataToken["input"] would be another JToken.
                // If 'input' is a simple string:
                string rawInput = dataToken?.Value<string>("input"); // Match key from server

                if (string.IsNullOrEmpty(playerId))
                {
                    Debug.LogError("Received invalid or null playerId for inputFromController.");
                    Debug.Log($"Raw response: {response.ToString()}");
                    return;
                }
                // rawInput might be null if not present, handle accordingly.

                if (players.TryGetValue(playerId, out GameObject playerObject))
                {
                    Debug.Log($"Received input '{rawInput ?? "null"}' for player '{playerId}'");
                    // Example: playerObject.GetComponent<PlayerMovementController>()?.HandleInput(rawInput);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error processing inputFromController event: {ex.Message}. Ensure this runs on the main thread if using Unity API.");
                Debug.Log($"Raw response: {response.ToString()}");
            }
        });

        Debug.Log("Attempting to connect to server at " + serverURL);
        socket.Connect();
    }

    void OnDestroy()
    {
        if (socket != null)
        {
            if (socket.Connected)
            {
                socket.Disconnect();
            }
            // Some libraries might require explicit disposal if they implement IDisposable
            // (socket as System.IDisposable)?.Dispose();
        }
    }
}