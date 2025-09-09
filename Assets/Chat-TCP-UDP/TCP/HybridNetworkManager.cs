using UnityEngine;

public class HybridNetworkManager : MonoBehaviour
{
    // --- REFERENCIAS A CLIENTE Y SERVIDOR TCP ---
    public TCPServer tcpServer; // Servidor TCP
    public TCPClient tcpClient; // Cliente TCP

    private void Start()
    {
        // --- SUSCRIPCIÓN A EVENTOS DEL SERVIDOR ---
        // Cuando el servidor reciba texto o imagen, se llaman estos métodos
        tcpServer.OnTextReceived += OnServerText;
        tcpServer.OnImageReceived += OnServerImage;

        // --- SUSCRIPCIÓN A EVENTOS DEL CLIENTE ---
        // Cuando el cliente reciba texto o imagen, se llaman estos métodos
        tcpClient.OnTextReceived += OnClientText;
        tcpClient.OnImageReceived += OnClientImage;
    }

    // --- MÉTODOS DEL SERVIDOR ---
    // Se ejecutan cuando el servidor recibe un mensaje de texto
    private void OnServerText(string msg)
    {
        Debug.Log("[Server Received Text] " + msg); // Log en la consola
        tcpServer.SendText("Echo from server: " + msg); // Reenvía el texto de vuelta (echo)
    }

    // Se ejecuta cuando el servidor recibe una imagen
    private void OnServerImage(Texture2D tex)
    {
        Debug.Log("[Server Received Image]");
        tcpServer.SendImage(tex); // Reenvía la misma imagen (echo)
    }

    // --- MÉTODOS DEL CLIENTE ---
    // Se ejecutan cuando el cliente recibe un mensaje de texto
    private void OnClientText(string msg)
    {
        Debug.Log("[Client Received Text] " + msg); // Log en la consola
        tcpClient.SendText("Echo from client: " + msg); // Reenvía el texto de vuelta (echo)
    }

    // Se ejecuta cuando el cliente recibe una imagen
    private void OnClientImage(Texture2D tex)
    {
        Debug.Log("[Client Received Image]");
        tcpClient.SendImage(tex); // Reenvía la misma imagen (echo)
    }
}
