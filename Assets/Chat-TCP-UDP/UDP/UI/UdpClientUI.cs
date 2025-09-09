using UnityEngine;
using TMPro;

public class UdpClientUI : MonoBehaviour
{
    public TMP_InputField ipInput;
    public TMP_InputField portInput;
    public TMP_InputField messageInput;

    [SerializeField] private UDPProtocol udpClient;

    private void Start()
    {
        if (udpClient != null)
        {
            udpClient.isServer = false; // this is the client
            udpClient.OnConnected += () => Debug.Log("✅ UDP Client connected to server!");
            udpClient.OnDataReceived += (msg) => Debug.Log("📩 From Server: " + msg);
        }
    }

    public void ConnectClient()
    {
        string ip = string.IsNullOrEmpty(ipInput.text) ? "127.0.0.1" : ipInput.text;
        int port = int.TryParse(portInput.text, out int p) ? p : 5555;

        udpClient.StartUDP(ip, port);
        Debug.Log($"UDP Client started -> {ip}:{port}");
    }

    public void SendClientMessage()
    {
        if (udpClient != null && udpClient.isConnected)
        {
            udpClient.SendData(messageInput.text);
        }
        else
        {
            Debug.LogWarning("⚠️ Client not connected yet!");
        }
    }
}
