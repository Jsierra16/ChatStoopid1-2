using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayButton : MonoBehaviour
{
    // --- REFERENCIA AL VIDEO ---
    public VideoPlayer videoPlayer; // El VideoPlayer que queremos reproducir

    // --- REFERENCIA AL BOTÓN ---
    private Button button; // El botón que activará la reproducción

    // --- CONFIGURACIÓN INICIAL ---
    private void Awake()
    {
        // Obtenemos el componente Button del mismo GameObject
        button = GetComponent<Button>();

        // Si existe, agregamos la función PlayVideo al evento onClick
        if (button != null)
            button.onClick.AddListener(PlayVideo);
    }

    // --- MÉTODO PARA REPRODUCIR EL VIDEO ---
    private void PlayVideo()
    {
        // Reproducimos el video solo si el VideoPlayer existe y no está reproduciendo
        if (videoPlayer != null && !videoPlayer.isPlaying)
            videoPlayer.Play();
    }
}
