using UnityEngine;

public class AudioSender : MonoBehaviour
{
    // --- REFERENCIAS ---
    public UDPProtocol udp;         // Referencia al protocolo UDP para enviar datos
    public int sampleRate = 16000;  // Tasa de muestreo baja para paquetes más pequeños

    private AudioClip micClip;      // Clip de audio donde se graba el micrófono
    private string micDevice;       // Nombre del dispositivo de micrófono
    private int lastSamplePos = 0;  // Posición del último sample leído

    // --- INICIALIZACIÓN ---
    void Start()
    {
        // Elegir el primer micrófono disponible
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];

            // Iniciar grabación en loop de 1 segundo
            micClip = Microphone.Start(micDevice, true, 1, sampleRate);
            Debug.Log("🎙️ Recording from mic: " + micDevice);
        }
        else
        {
            Debug.LogError("❌ No microphone found!");
        }
    }

    // --- ENVÍO CONTINUO DE AUDIO ---
    void Update()
    {
        if (micClip == null) return;

        // Posición actual de grabación
        int pos = Microphone.GetPosition(micDevice);

        // Si se reinició el buffer (looped), reiniciamos la posición
        if (pos < lastSamplePos) lastSamplePos = 0;

        int samplesToRead = pos - lastSamplePos;
        if (samplesToRead > 0)
        {
            // Leer los samples del micrófono
            float[] samples = new float[samplesToRead * micClip.channels];
            micClip.GetData(samples, lastSamplePos);

            // Convertir float[] a byte[] y enviar vía UDP
            byte[] data = FloatArrayToByteArray(samples);
            udp.SendData(data);

            // Actualizar última posición
            lastSamplePos = pos;
        }
    }

    // --- CONVERSIÓN DE FLOAT A BYTE ---
    private byte[] FloatArrayToByteArray(float[] samples)
    {
        byte[] bytes = new byte[samples.Length * 4]; // 4 bytes por float
        for (int i = 0; i < samples.Length; i++)
        {
            byte[] b = System.BitConverter.GetBytes(samples[i]);
            b.CopyTo(bytes, i * 4);
        }
        return bytes;
    }
}
