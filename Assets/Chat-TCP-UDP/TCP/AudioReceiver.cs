using UnityEngine;

public class AudioReceiver : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioRecorder recorder; // we reuse its BytesToAudioClip()

    public void Setup(TCPClient client)
    {
        client.OnAudioReceived += (audioData) =>
        {
            AudioClip clip = recorder.BytesToAudioClip(audioData);
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log("▶️ Playing received voice message");
        };
    }

    public void Setup(TCPServer server)
    {
        server.OnAudioReceived += (audioData) =>
        {
            AudioClip clip = recorder.BytesToAudioClip(audioData);
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log("▶️ Playing received voice message");
        };
    }
}
