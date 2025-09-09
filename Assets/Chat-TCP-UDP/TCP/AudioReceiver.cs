using UnityEngine;

public class AudioReceiver : MonoBehaviour
{
    // --- COMPONENTES DE AUDIO ---
    public AudioSource audioSource; // Reproduce los clips de audio recibidos
    public AudioRecorder recorder;  // Se reutiliza para convertir bytes a AudioClip

    // --- CONFIGURACIÓN PARA CLIENTE TCP ---
    public void Setup(TCPClient client)
    {
        // Suscribirse al evento de audio recibido del cliente
        client.OnAudioReceived += (audioData) =>
        {
            // Convertir los bytes recibidos en un AudioClip
            AudioClip clip = recorder.BytesToAudioClip(audioData);

            // Asignar el clip al AudioSource y reproducirlo
            audioSource.clip = clip;
            audioSource.Play();

            // Log para depuración
            Debug.Log("▶️ Playing received voice message");
        };
    }

    // --- CONFIGURACIÓN PARA SERVIDOR TCP ---
    public void Setup(TCPServer server)
    {
        // Suscribirse al evento de audio recibido del servidor
        server.OnAudioReceived += (audioData) =>
        {
            // Convertir los bytes recibidos en un AudioClip
            AudioClip clip = recorder.BytesToAudioClip(audioData);

            // Asignar el clip al AudioSource y reproducirlo
            audioSource.clip = clip;
            audioSource.Play();

            // Log para depuración
            Debug.Log("▶️ Playing received voice message");
        };
    }
}
