using UnityEngine;

public class AudioSender : MonoBehaviour
{
    public UDPProtocol udp;         // Reference to your UDPProtocol
    public int sampleRate = 16000;  // Lower sample rate for smaller packets
    private AudioClip micClip;
    private string micDevice;
    private int lastSamplePos = 0;

    void Start()
    {
        // Pick first microphone
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            micClip = Microphone.Start(micDevice, true, 1, sampleRate);
            Debug.Log("🎙️ Recording from mic: " + micDevice);
        }
        else
        {
            Debug.LogError("❌ No microphone found!");
        }
    }

    void Update()
    {
        if (micClip == null) return;

        int pos = Microphone.GetPosition(micDevice);
        if (pos < lastSamplePos) lastSamplePos = 0; // looped

        int samplesToRead = pos - lastSamplePos;
        if (samplesToRead > 0)
        {
            float[] samples = new float[samplesToRead * micClip.channels];
            micClip.GetData(samples, lastSamplePos);

            // Convert to bytes
            byte[] data = FloatArrayToByteArray(samples);
            udp.SendData(data);

            lastSamplePos = pos;
        }
    }

    private byte[] FloatArrayToByteArray(float[] samples)
    {
        byte[] bytes = new byte[samples.Length * 4];
        for (int i = 0; i < samples.Length; i++)
        {
            byte[] b = System.BitConverter.GetBytes(samples[i]);
            b.CopyTo(bytes, i * 4);
        }
        return bytes;
    }
}
