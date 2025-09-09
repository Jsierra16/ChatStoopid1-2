using UnityEngine;

public class AudioReceiver : MonoBehaviour
{
    public UDPProtocol udp;
    public int sampleRate = 16000;

    private AudioSource audioSource;
    private const int bufferSize = 16000;
    private float[] audioBuffer = new float[bufferSize];
    private int bufferIndex = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;

        // Create streaming clip
        AudioClip clip = AudioClip.Create("RemoteAudio", bufferSize, 1, sampleRate, true, OnAudioRead, OnAudioSetPosition);
        audioSource.clip = clip;
        audioSource.Play();

        udp.OnBytesReceived += HandleAudioData;
    }

    private void HandleAudioData(byte[] data)
    {
        if (data == null || data.Length == 0) return;

        float[] samples = ByteArrayToFloatArray(data);

        foreach (var sample in samples)
        {
            audioBuffer[bufferIndex] = sample;
            bufferIndex = (bufferIndex + 1) % bufferSize;
        }
    }

    private float[] ByteArrayToFloatArray(byte[] bytes)
    {
        int count = bytes.Length / 4;
        float[] floats = new float[count];
        for (int i = 0; i < count; i++)
        {
            floats[i] = System.BitConverter.ToSingle(bytes, i * 4);
        }
        return floats;
    }

    private void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = audioBuffer[(bufferIndex + i) % bufferSize];
        }
    }

    private void OnAudioSetPosition(int newPosition) { }
}
