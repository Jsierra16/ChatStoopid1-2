using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    public AudioSource audioSource;
    private AudioClip recordedClip;
    private int sampleRate = 44100;

    public void StartRecording()
    {
        if (Microphone.devices.Length > 0)
        {
            recordedClip = Microphone.Start(null, false, 10, sampleRate);
            Debug.Log("🎙 Grabando...");
        }
    }

    public void StopRecording()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            Debug.Log("⏹ Grabación terminada");
        }
    }

    public void PlayRecording()
    {
        if (recordedClip != null)
        {
            audioSource.clip = recordedClip;
            audioSource.Play();
        }
    }

    public byte[] GetRecordedData()
    {
        if (recordedClip == null) return null;

        float[] samples = new float[recordedClip.samples];
        recordedClip.GetData(samples, 0);

        byte[] bytes = new byte[samples.Length * 2];
        int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = System.BitConverter.GetBytes(value);
            bytes[i * 2] = byteArr[0];
            bytes[i * 2 + 1] = byteArr[1];
        }

        return bytes;
    }

    public AudioClip BytesToAudioClip(byte[] audioData)
    {
        int samples = audioData.Length / 2;
        float[] floatData = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            short value = System.BitConverter.ToInt16(audioData, i * 2);
            floatData[i] = value / 32767f;
        }

        AudioClip clip = AudioClip.Create("ReceivedAudio", samples, 1, sampleRate, false);
        clip.SetData(floatData, 0);

        return clip;
    }
}