using UnityEngine;
using UnityEngine.UI;

public class VideoReceiver : MonoBehaviour
{
    [SerializeField] private TCPServer server;
    [SerializeField] private TCPClient client;
    [SerializeField] private RawImage rawImage;

    private void OnEnable()
    {
        if (server != null)
            server.OnImageReceived += OnImageReceived;

        if (client != null)
            client.OnImageReceived += OnImageReceived;
    }

    private void OnDisable()
    {
        if (server != null)
            server.OnImageReceived -= OnImageReceived;

        if (client != null)
            client.OnImageReceived -= OnImageReceived;
    }

    private void OnImageReceived(Texture2D texture)
    {
        if (rawImage != null)
        {
            rawImage.texture = texture;
            rawImage.SetNativeSize();
        }
    }
}
