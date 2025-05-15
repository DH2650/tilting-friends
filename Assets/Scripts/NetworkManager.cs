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
    [SerializeField] DebugOverlay debug;

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

        socket.On("playerJoined", (response) =>
        {
            // CRITICAL: The following Unity API calls (Instantiate, Add to Dictionary)
            // MUST execute on the main thread.
            Debug.Log("Try connect");

            MainThread.wkr.AddJob(() => {
                try
                {
                    // Attempt to get data without custom class deserialization.
                    // This assumes the server sends a JSON object as the first (or only) argument.
                    // response.GetValue<JToken>() might get the first JToken in the response payload.
                    string controllerId = GetField(response.ToString(), "controllerId");
                    string playerName = GetInput(response.ToString(), "assignedPlayerId");

                    if (string.IsNullOrEmpty(controllerId))
                    {
                        Debug.LogError("Received invalid or null controllerId for playerJoined.");
                        // Consider how your specific library handles response.ToString() for logging
                        Debug.Log($"controllerId not found: {controllerId}");
                        return;
                    }

                    Debug.Log("Controller connected (player joined): " + controllerId);
                    if (!players.ContainsKey(controllerId) && playerPrefab != null)
                    {
                        GameObject newPlayer = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity);

                        NetworkPlayerMovement ps = newPlayer.GetComponent<NetworkPlayerMovement>();
                        Rigidbody rb = newPlayer.GetComponent<Rigidbody>();
                        ps.Initialize(rb, playerName);

                        players.Add(controllerId, newPlayer);
                        Debug.Log("Spawned player: " + controllerId);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error processing playerJoined event: {ex.Message}. Ensure this runs on the main thread if using Unity API.");
                    Debug.Log($"Error processing controllerConnected event: {ex.Message}");
                }
            });
        });

        socket.On("disconnect", (response) =>
        {
            // CRITICAL: The following Unity API calls (Destroy, Remove from Dictionary)
            // MUST execute on the main thread.
            MainThread.wkr.AddJob(() => {
                try
                {
                    string controllerId = GetField(response.ToString(), "controllerId");

                    if (string.IsNullOrEmpty(controllerId))
                    {
                        Debug.LogError("Received invalid or null playerId for disconnect.");
                        Debug.Log($"No controllerId: {response.ToString()}");
                        return;
                    }

                    Debug.Log("Controller disconnected (player left): " + controllerId);
                    if (players.TryGetValue(controllerId, out GameObject playerObject))
                    {
                        Destroy(playerObject);
                        players.Remove(controllerId);
                        Debug.Log("Destroyed player: " + controllerId);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error processing controllerDisconnected event: {ex.Message}. Ensure this runs on the main thread if using Unity API.");
                    Debug.Log($"Error processing controllerDisconnected event: {ex.Message}");
                }
            });
        });

        socket.On("controllerInput", (response) =>
        {
            // CRITICAL: Any interaction with player GameObjects MUST execute on the main thread.
            MainThread.wkr.AddJob(() => {
                try
                {
                    string controllerId = GetField(response.ToString(), "controllerId");
                    string rawInput = GetInput(response.ToString(), "input");

                    if (string.IsNullOrEmpty(controllerId))
                    {
                        Debug.LogError("Received invalid or null playerId for inputFromController.");
                        Debug.Log($"No controllerId: {response.ToString()}");
                        return;
                    }
                    // rawInput might be null if not present, handle accordingly.

                    if (players.ContainsKey(controllerId))
                    {
                        Debug.Log($"Received input '{response.ToString() ?? "null"}' for player '{controllerId}'");

                        GameObject playerObject = players[controllerId];
                        playerObject.GetComponent<NetworkPlayerMovement>()?.ProcessInput(rawInput);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error processing controllerInput event: {ex.Message}. Ensure this runs on the main thread if using Unity API.");
                    Debug.Log($"Error processing controllerInput event: {ex.Message}");
                }
            });
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

    string GetField(string json, string find)
    {
//         Debug.Log($"JSON string: {json}");
        int start = json.IndexOf(find);
//         Debug.Log($"Start Index: {start}");
        string controllerId = json.Substring(start + find.Length + 3, 20);
        Debug.Log($"controllerId: {controllerId}");

        return controllerId;
    }

    string GetInput(string json, string find)
    {
//         Debug.Log($"JSON string: {json}");
        int start = json.IndexOf(find);
        Debug.Log($"Start Index: {start + find.Length + 3}");
        int end = json.LastIndexOf('"');
        Debug.Log($"End Index: {end}");
        Debug.Log($"Diff: {end - (start + find.Length + 3)}");
        string controllerId = json.Substring(start + find.Length + 3, end - (start + find.Length + 3));
        Debug.Log($"input: {controllerId}");

        return controllerId;
    }

    void Update()
    {
        string msg = "";

        foreach(var (controllerId, player) in players)
        {
            NetworkPlayerMovement ps = player.GetComponent<NetworkPlayerMovement>();

            msg += $"Name: {ps.name} - Moving: {ps.moving} - Pos: {ps.rb.transform.position}\n";
        }
        debug.debugText.text = msg;
    }
}