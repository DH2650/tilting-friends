    using UnityEngine;
    using WebSocketSharp.Server;
    using TMPro;
    using System;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;

    public class LobbyManager : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI lobbyText;
        public TextMeshProUGUI roomCodeText;

        [Header("Server Settings")]
        public int port = 8080;

    // Add these new fields
    [Header("Game Settings")]
    public int minPlayers = 1;  // Minimum players needed to start
    public int maxPlayers = 2;  // Maximum players allowed
    public bool allowSinglePlayer = true;  // Enable single-player mode

    // Add a UI element to show player count
    public TextMeshProUGUI playerCountText;

    // Expose for the service
    public string CurrentRoomCode { get; private set; }
        private WebSocketServer _wssv;

        void Start()
        {
            // 1) Generate and display code
            CurrentRoomCode = GenerateRoomCode(5);
            roomCodeText.text = CurrentRoomCode;

       
            Debug.Log("ROOM CODE: " + CurrentRoomCode);
            // 2) Hook up the static reference
            JoinService.Lobby = this;

            // 3) Start WebSocketSharp server
            //_wssv = new WebSocketServer(port);
            _wssv = new WebSocketServer(System.Net.IPAddress.Any, port);
            _wssv.AddWebSocketService<JoinService>("/lobby");
            _wssv.Start();
            Debug.Log("WebSocket server started on port 8080");
            Debug.Log($"WebSocketSharp server listening on ws://<your-ip>:{port}/lobby");
        }

        void OnApplicationQuit()
        {
            _wssv?.Stop();
        }

    // Called by JoinService when a player joins

    //public void AppendJoin(string playerName, JoinService client)
    //{
    //    // Update lobby text
    //    lobbyText.text += "\n" + playerName.ToUpper() + " JOINED!";

    //    // Force UI refresh
    //    Canvas.ForceUpdateCanvases();

    //    // Assign player index
    //    client.PlayerIndex = JoinService.ConnectedClients.Count - 1;

    //    // First send status update
    //    client.SendToClient("{\"type\": \"status\", \"message\": \"You've joined the lobby.\"}");

    //    // Then send player assignment after a short delay
    //    StartCoroutine(DelayedAssign(client));
    //}

    public void AppendJoin(string playerName, JoinService client)
    {
        lobbyText.text += "\n" + playerName.ToUpper() + " JOINED!";

        // Force UI refresh
        Canvas.ForceUpdateCanvases();

        // Assign player index
        client.PlayerIndex = JoinService.ConnectedClients.Count - 1;

        // Update player count display
        int currentPlayers = JoinService.ConnectedClients.Count;
        if (playerCountText != null)
            playerCountText.text = $"Players: {currentPlayers}/{maxPlayers}";

        // Tell that client which player they are
        client.SendToClient("{\"type\": \"assign\", \"player\": " + client.PlayerIndex + "}");

        // If we have enough players and single player is allowed, enable the start button
        if (allowSinglePlayer && currentPlayers >= minPlayers)
        {
            // If you have a reference to your start button:
            // startGameButton.interactable = true;
        }
    }
    private System.Collections.IEnumerator DelayedAssign(JoinService client)
    {
        // Wait a moment to ensure status message is processed first
        yield return new WaitForSeconds(0.5f);

        // Send assign message
        client.SendToClient("{\"type\": \"assign\", \"player\": " + client.PlayerIndex + "}");
    }

    // Triggered by Start Game button
    //public void StartGame()
    //{
    //    Debug.Log("Starting game, notifying clients...");

    //    // Tell all connected clients to start the game
    //    foreach (var client in JoinService.ConnectedClients)
    //    {
    //        client.SendStartGame();
    //    }

    //    // Load the game scene
    //    SceneManager.LoadScene("SampleScene");
    //}

    // Update StartGame to check player count
    public void StartGame()
    {
        int playerCount = JoinService.ConnectedClients.Count;

        if (playerCount < minPlayers)
        {
            Debug.LogWarning($"Cannot start game: Need at least {minPlayers} players, but only have {playerCount}");
            return;
        }

        Debug.Log($"Starting game with {playerCount} players...");

        // Tell all connected clients to start the game
        foreach (var client in JoinService.ConnectedClients)
        {
            client.SendStartGame();
        }

        // Register a callback for after scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the game scene
        SceneManager.LoadScene("SampleScene");
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This code runs after the scene is loaded
        // You can put any initialization code here
        Debug.Log("Scene loaded: " + scene.name);

        // Unsubscribe to prevent duplicate calls
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    string GenerateRoomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var code = new char[length];
            for (int i = 0; i < length; i++)
                code[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
            return new string(code);
        }

        [Serializable]
        public class JoinMessage
        {
            public string type;
            public string name;
            public string room;
        }

    [Serializable]
    public class MoveMessage
    {
        public string type;
        public int player;
        public float x;
        public float z;
    }
}
