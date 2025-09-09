using UnityEngine;

public class VideoSender : MonoBehaviour
{
    // --- REFERENCIAS A TCP ---
    [Header("References")]
    public TCPClient tcpClient; // Cliente TCP para enviar video
    public TCPServer tcpServer; // Servidor TCP para enviar video

    // --- AJUSTES DE LA CÁMARA ---
    [Header("Camera Settings")]
    public Camera captureCamera;   // Cámara que captura el video
    public int captureWidth = 320; // Ancho de la captura
    public int captureHeight = 240; // Alto de la captura

    // --- VARIABLES INTERNAS ---
    private RenderTexture renderTexture; // RenderTexture para capturar la cámara
    private Texture2D frameTexture;      // Texture2D que se enviará por TCP
    private bool isSending = false;      // Controla si se está enviando video

    // --- CONFIGURACIÓN INICIAL ---
    void Start()
    {
        if (captureCamera != null)
        {
            // Crear un RenderTexture con las dimensiones deseadas
            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);

            // Crear un Texture2D donde se copiarán los pixels del RenderTexture
            frameTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

            // Asignar el RenderTexture a la cámara para que renderice allí
            captureCamera.targetTexture = renderTexture;
        }
    }

    // --- ACTUALIZACIÓN CADA FRAME ---
    void Update()
    {
        // Si estamos enviando, capturamos y enviamos un frame
        if (isSending)
        {
            CaptureAndSendFrame();
        }
    }

    // --- MÉTODOS PÚBLICOS PARA INICIAR / DETENER EL ENVÍO ---
    public void StartSending()
    {
        isSending = true;
    }

    public void StopSending()
    {
        isSending = false;
    }

    // --- CAPTURA Y ENVÍO DEL FRAME ---
    private void CaptureAndSendFrame()
    {
        if (captureCamera == null || frameTexture == null) return;

        // Renderizamos la cámara al RenderTexture
        RenderTexture.active = renderTexture;
        captureCamera.Render();

        // Copiamos los pixels del RenderTexture al Texture2D
        frameTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        frameTexture.Apply();

        // Liberamos el RenderTexture activo
        RenderTexture.active = null;

        // Enviamos la imagen a través del cliente y/o servidor TCP
        if (tcpClient != null)
        {
            tcpClient.SendImage(frameTexture);
        }

        if (tcpServer != null)
        {
            tcpServer.SendImage(frameTexture);
        }
    }
}
