using UnityEngine;

public class WebCamSenderUDP : MonoBehaviour
{
    [SerializeField] private UDPProtocol udp;   // Referencia a tu protocolo UDP
    [SerializeField] private int captureWidth = 320;   // Ancho del frame
    [SerializeField] private int captureHeight = 240;  // Alto del frame
    [SerializeField] private int quality = 75;         // Calidad JPEG 1-100

    private WebCamTexture webCam;

    void Start()
    {
        // Inicializar la cámara
        webCam = new WebCamTexture(captureWidth, captureHeight);
        webCam.Play();
    }

    void Update()
    {
        if (webCam.width <= 16) return; // Cámara no lista aún

        // Capturar frame
        Texture2D tex = new Texture2D(webCam.width, webCam.height, TextureFormat.RGB24, false);
        tex.SetPixels(webCam.GetPixels());
        tex.Apply();

        // Convertir a JPG para reducir tamaño
        byte[] data = tex.EncodeToJPG(quality);

        // Enviar por UDP
        if (udp != null)
            udp.SendData(data);

        Destroy(tex); // Liberar memoria del frame temporal
    }
}
