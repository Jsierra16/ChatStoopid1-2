using UnityEngine;
using UnityEngine.UI;

public class Script : MonoBehaviour
{
    // --- REFERENCIA ---
    [SerializeField] private RawImage img = default; 
    // RawImage donde se mostrará el video de la cámara

    private WebCamTexture webCam; 
    // Textura que recibirá el stream de la cámara web

    // --- INICIALIZACIÓN ---
    void Start()
    {
        webCam = new WebCamTexture(); // Crear la textura de la cámara
        if (!webCam.isPlaying)        // Verificar si no está reproduciéndose
            webCam.Play();            // Iniciar la cámara

        img.texture = webCam;         // Asignar la textura al RawImage
    }

    // --- ACTUALIZACIÓN POR FRAME ---
    void Update()
    {
        // Actualmente no se realiza ninguna acción por frame
    }
}
