using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    // --- COMPONENTES DE AUDIO ---
    public AudioSource audioSource; // Reproduce el audio grabado o recibido
    private AudioClip recordedClip; // Clip que contiene la grabación
    private int sampleRate = 44100; // Frecuencia de muestreo estándar

    // --- INICIA LA GRABACIÓN DE MICROFONO ---
    public void StartRecording()
    {
        if (Microphone.devices.Length > 0) // Verifica si hay un micrófono disponible
        {
            // Inicia la grabación por 10 segundos, sin loop
            recordedClip = Microphone.Start(null, false, 10, sampleRate);
            Debug.Log("🎙 Grabando...");
        }
    }

    // --- DETIENE LA GRABACIÓN ---
    public void StopRecording()
    {
        if (Microphone.IsRecording(null)) // Verifica si el micrófono está grabando
        {
            Microphone.End(null); // Termina la grabación
            Debug.Log("⏹ Grabación terminada");
        }
    }

    // --- REPRODUCE EL AUDIO GRABADO ---
    public void PlayRecording()
    {
        if (recordedClip != null)
        {
            audioSource.clip = recordedClip;
            audioSource.Play();
        }
    }

    // --- CONVIERTE EL AUDIO GRABADO A BYTES ---
    public byte[] GetRecordedData()
    {
        if (recordedClip == null) return null;

        // Obtiene todos los samples (flotantes) de la grabación
        float[] samples = new float[recordedClip.samples];
        recordedClip.GetData(samples, 0);

        // Convierte los floats en bytes (16 bits por sample)
        byte[] bytes = new byte[samples.Length * 2];
        int rescaleFactor = 32767; // Factor para pasar de float [-1,1] a short [-32767,32767]

        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * rescaleFactor); // Convertir float a short
            byte[] byteArr = System.BitConverter.GetBytes(value); // Convertir short a bytes
            bytes[i * 2] = byteArr[0];
            bytes[i * 2 + 1] = byteArr[1];
        }

        return bytes;
    }

    // --- CONVIERTE BYTES RECIBIDOS EN UN AUDIOCLIP ---
    public AudioClip BytesToAudioClip(byte[] audioData)
    {
        int samples = audioData.Length / 2; // Cada sample ocupa 2 bytes
        float[] floatData = new float[samples];

        // Convertir bytes de 16 bits a float [-1, 1]
        for (int i = 0; i < samples; i++)
        {
            short value = System.BitConverter.ToInt16(audioData, i * 2);
            floatData[i] = value / 32767f;
        }

        // Crear un AudioClip con los datos convertidos
        AudioClip clip = AudioClip.Create("ReceivedAudio", samples, 1, sampleRate, false);
        clip.SetData(floatData, 0);

        return clip;
    }
}
