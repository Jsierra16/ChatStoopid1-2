using UnityEngine;

public class HybridNetworkManager : MonoBehaviour
{
    public TCPServer tcpServer;
    public TCPClient tcpClient;

    private void Start()
    {
        // Hook up events
        tcpServer.OnTextReceived += OnServerText;
        tcpServer.OnImageReceived += OnServerImage;

        tcpClient.OnTextReceived += OnClientText;
        tcpClient.OnImageReceived += OnClientImage;
    }

    private void OnServerText(string msg)
    {
        Debug.Log("[Server Received Text] " + msg);
        tcpServer.SendText("Echo from server: " + msg);
    }

    private void OnServerImage(Texture2D tex)
    {
        Debug.Log("[Server Received Image]");
        tcpServer.SendImage(tex);
    }

    private void OnClientText(string msg)
    {
        Debug.Log("[Client Received Text] " + msg);
        tcpClient.SendText("Echo from client: " + msg);
    }

    private void OnClientImage(Texture2D tex)
    {
        Debug.Log("[Client Received Image]");
        tcpClient.SendImage(tex);
    }
}
