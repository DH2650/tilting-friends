using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;

public class JoinService : WebSocketBehavior
{
    // We'll use a static reference back to the LobbyManager
    public static LobbyManager Lobby;
    public static List<JoinService> ConnectedClients = new List<JoinService>();
    public int PlayerIndex = -1;
    // Add this queue for thread-safe message processing
    private static ConcurrentQueue<KeyValuePair<int, string>> _messageQueue = new ConcurrentQueue<KeyValuePair<int, string>>();
    public void SendToClient(string msg) {
        Debug.Log($"Sending to client: {msg}");
        Send(msg); }
    protected override void OnOpen()
    {
        ConnectedClients.Add(this);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        ConnectedClients.Remove(this);
    }
    protected override void OnMessage(MessageEventArgs e)
    {
        var raw = e.Data;
        Debug.Log($"Raw WebSocket message received: {raw}");

        // Queue message with this client's player index for processing on main thread
        _messageQueue.Enqueue(new KeyValuePair<int, string>(PlayerIndex, raw));
    }


    // Allows LobbyManager to tell this client to start the game
    public void SendStartGame()
    {
        Debug.Log($"Sending start game to client with player index: {PlayerIndex}");
        Send("{\"type\": \"start\"}");
    }

 

    // Add this method to process the message queue on the main thread
    public static void ProcessMessageQueue()
    {
        while (_messageQueue.TryDequeue(out var item))
        {
            int playerIndex = item.Key;
            string message = item.Value;

            try
            {
                // Parse the message
                var baseMsg = JsonUtility.FromJson<LobbyManager.JoinMessage>(message);

                if (baseMsg.type == "move")
                {
                    var move = JsonUtility.FromJson<LobbyManager.MoveMessage>(message);
                    Debug.Log($"Processing move message from player {move.player}: ({move.x}, 0, {move.z})");

                    if (TwoPlayerController.Instance != null)
                    {
                        TwoPlayerController.Instance.SetInput(
                            move.player,
                            new Vector3(move.x, 0, move.z)
                        );
                    }
                    else
                    {
                        Debug.LogError("TwoPlayerController.Instance is null! Cannot set input.");
                    }
                }
                else if (baseMsg.type == "join" && baseMsg.room == Lobby.CurrentRoomCode)
                {
                    // Notify lobby and pass the JoinService
                    var client = ConnectedClients.Find(c => c.PlayerIndex == playerIndex);
                    if (client != null)
                    {
                        Lobby.AppendJoin(baseMsg.name, client);
                        Debug.Log($"Player {baseMsg.name} joined room {baseMsg.room}");

                        // Reply to the client: waiting for others
                        client.Send("{\"type\": \"status\", \"message\": \"Waiting for other players...\"}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing message: {ex.Message}");
            }
        }
    }
}
