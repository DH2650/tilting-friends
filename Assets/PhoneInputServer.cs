using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Net;
using System.Net.Sockets;


// -----------------------------------------------------------------
// 1) Data container for the incoming JSON
[Serializable]
public class GyroData
{
    public float alpha;  // z-axis rotation
    public float beta;   // x-axis tilt
    public float gamma;  // y-axis tilt
}

// -----------------------------------------------------------------
// 2) WebSocket behavior: parses messages and forwards to server
public class InputBehavior : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        try
        {
            // Parse JSON → GyroData
            var data = JsonUtility.FromJson<GyroData>(e.Data);
            // Dispatch to the main MonoBehaviour
            PhoneInputServer.Instance.UpdateOrientation(data);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[InputBehavior] JSON parse error: {ex.Message}");
        }
    }
}

// -----------------------------------------------------------------
// 3) The main server component
public class PhoneInputServer : MonoBehaviour
{
    public static PhoneInputServer Instance { get; private set; }

    [Tooltip("Drag your Cube (or any object) here to have it follow the phone tilt")]
    public Transform cubeTransform;

    private WebSocketServer _wssv;
    private GyroData _latest;

    void Awake()
    {
        // Singleton-style instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize a default orientation so _latest is never null
        _latest = new GyroData();

        // Start WebSocket server on port 8081
        _wssv = _wssv = new WebSocketServer(System.Net.IPAddress.Any, 8081);
        _wssv.AddWebSocketService<InputBehavior>("/gyro");
        _wssv.Start();
        Debug.Log("WSServer started on ws://" + GetLocalIPAddress() + ":8081/gyro");


        Debug.Log("WebSocket server started on port 8081");

        // Print out your actual LAN IPv4 addresses
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                Debug.Log($"WS listening at → ws://{ip}:8081/gyro");
        }
    }
    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1"; // fallback
    }
    /// <summary>
    /// Called from InputBehavior whenever new data arrives.
    /// </summary>
    public void UpdateOrientation(GyroData data)
    {
        _latest = data;
    }

    void Update()
    {
        // If the user forgot to assign a cube, bail out
        if (cubeTransform == null) return;

        // Apply the latest orientation
        cubeTransform.rotation = Quaternion.Euler(
            _latest.beta,
            _latest.alpha,
            _latest.gamma
        );
    }

    void OnDestroy()
    {
        // Stop server cleanly
        _wssv?.Stop();
    }
}
