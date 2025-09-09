using UnityEngine;
using TMPro;

public class UdpServerUI : MonoBehaviour
{
    public TMP_InputField portInput;
    public TMP_InputField messageInput;

    [SerializeField] private UDPProtocol udpServer;

    private void Start()
    {
        if (udpServer != null)
        {
            udpServer.isServer = true; // this is the server
            udpServer.OnConnected += () => Debug.Log("✅ UDP Client connected to server!");
            udpServer.OnDataReceived += (msg) => Debug.Log("📩 From Client: " + msg);
        }
    }

    public void StartServer()
    {
        int port = int.TryParse(portInput.text, out int p) ? p : 5555;
        udpServer.StartUDP("0.0.0.0", port);
        Debug.Log($"UDP Server started on port {port}");
    }

    public void SendServerMessage()
    {
        if (udpServer != null && udpServer.isConnected)
        {
            udpServer.SendData(messageInput.text);
        }
        else
        {
            Debug.LogWarning("⚠️ No client connected yet!");
        }
    }
}
