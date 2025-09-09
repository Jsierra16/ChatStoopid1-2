using UnityEngine;
using UnityEngine.UI;

public class VideoReceiver : MonoBehaviour
{
    // --- COMPONENTES DE VIDEO ---
    [SerializeField] private TCPServer server; // Referencia al servidor TCP
    [SerializeField] private TCPClient client; // Referencia al cliente TCP
    [SerializeField] private RawImage rawImage; // UI para mostrar la imagen recibida

    // --- SUSCRIPCIÓN A EVENTOS ---
    private void OnEnable()
    {
        // Si el servidor existe, suscribirse al evento de imagen recibida
        if (server != null)
            server.OnImageReceived += OnImageReceived;

        // Si el cliente existe, suscribirse al evento de imagen recibida
        if (client != null)
            client.OnImageReceived += OnImageReceived;
    }

    // --- DESUSCRIPCIÓN A EVENTOS ---
    private void OnDisable()
    {
        // Si el servidor existe, remover la suscripción
        if (server != null)
            server.OnImageReceived -= OnImageReceived;

        // Si el cliente existe, remover la suscripción
        if (client != null)
            client.OnImageReceived -= OnImageReceived;
    }

    // --- MÉTODO QUE SE LLAMA CUANDO SE RECIBE UNA IMAGEN ---
    private void OnImageReceived(Texture2D texture)
    {
        if (rawImage != null)
        {
            // Asignar la textura recibida al RawImage
            rawImage.texture = texture;

            // Ajustar el tamaño del RawImage al tamaño nativo de la textura
            rawImage.SetNativeSize();
        }
    }
}
