using UnityEngine;
using UnityEngine.UI;

public class WebcamReceiverUDP : MonoBehaviour
{
    [Header("UDP Settings")]
    public UDPProtocol udp;       // Referencia al protocolo UDP que maneja la recepción
    [Header("UI")]
    public RawImage displayImage; // UI donde mostraremos la imagen recibida

    private Texture2D receivedTexture; // Texture2D donde se actualizarán los pixeles recibidos

    // ================================
    // Inicialización
    // ================================
    private void OnEnable()
    {
        // Nos suscribimos al evento de bytes recibidos del UDPProtocol
        // Esto asegura que OnDataReceivedBytes se llame cada vez que lleguen datos
        udp.OnBytesReceived += OnDataReceivedBytes;
    }

    private void OnDisable()
    {
        // Quitamos la suscripción para evitar errores al destruir el objeto
        udp.OnBytesReceived -= OnDataReceivedBytes;
    }

    // ================================
    // Recepción de imágenes
    // ================================
    private void OnDataReceivedBytes(byte[] data)
    {
        // Si no tenemos una textura creada, la inicializamos con tamaño arbitrario
        // Se sobrescribirá automáticamente cuando se reciba la primera imagen
        if (receivedTexture == null)
            receivedTexture = new Texture2D(2, 2);

        // Se intenta cargar la imagen recibida desde bytes (espera formato JPG)
        if (receivedTexture.LoadImage(data))
        {
            // Actualizamos el RawImage para mostrar la textura recibida
            if (displayImage != null)
            {
                displayImage.texture = receivedTexture;
                displayImage.SetNativeSize(); // Ajusta el tamaño de la UI al de la imagen
            }
        }
    }
}
