using System;
using System.Net.Sockets;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    // --- VARIABLES DE RED ---
    private TcpClient client;                // Cliente TCP nativo de .NET
    private NetworkStream networkStream;     // Flujo de datos (entrada/salida con el servidor)
    private byte[] receiveBuffer;            // Buffer temporal para datos recibidos

    public bool isConnected;                 // Estado de conexión

    // --- EVENTOS PARA DATOS RECIBIDOS ---
    public Action<string> OnTextReceived;      // Mensajes de texto
    public Action<Texture2D> OnImageReceived;  // Imágenes
    public Action<byte[]> OnAudioReceived;     // Audio en bytes

    // --- CONEXIÓN AL SERVIDOR ---
    public void ConnectToServer(string ip, int port)
    {
        try
        {
            client = new TcpClient();
            client.Connect(ip, port);                // Intentar conectar
            networkStream = client.GetStream();
            isConnected = true;

            Debug.Log("✅ Connected to server: " + ip + ":" + port);

            // Preparar buffer y comenzar lectura asíncrona
            receiveBuffer = new byte[client.ReceiveBufferSize];
            networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Connection failed: " + e.Message);
        }
    }

    // --- RECEPCIÓN DE DATOS DEL SERVIDOR ---
    private void ReceiveData(IAsyncResult result)
    {
        if (networkStream == null) return;

        int bytesRead = networkStream.EndRead(result);

        // Si no hay datos, la conexión se cerró
        if (bytesRead <= 0)
        {
            Debug.Log("⚠️ Disconnected from server.");
            client.Close();
            isConnected = false;
            return;
        }

        // Procesar paquetes dentro del buffer
        int index = 0;
        while (index < bytesRead)
        {
            // 1 byte para el tipo de dato (0=texto, 1=imagen, 2=audio)
            byte type = receiveBuffer[index];
            index++;

            // 4 bytes siguientes = tamaño del contenido
            int length = BitConverter.ToInt32(receiveBuffer, index);
            index += 4;

            // Copiar contenido real en un nuevo arreglo
            byte[] data = new byte[length];
            Array.Copy(receiveBuffer, index, data, 0, length);
            index += length;

            // --- Diferenciar según el tipo ---
            if (type == 0) // TEXTO
            {
                string message = System.Text.Encoding.UTF8.GetString(data);
                MainThreadDispatcher.Enqueue(() =>
                {
                    Debug.Log("📩 Text from server: " + message);
                    OnTextReceived?.Invoke(message);
                });
            }
            else if (type == 1) // IMAGEN
            {
                byte[] imgData = data;
                MainThreadDispatcher.Enqueue(() =>
                {
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imgData);
                    Debug.Log("🖼️ Image from server!");
                    OnImageReceived?.Invoke(tex);
                });
            }
            else if (type == 2) // AUDIO
            {
                byte[] audioData = data;
                MainThreadDispatcher.Enqueue(() =>
                {
                    Debug.Log("🎵 Audio from server (" + audioData.Length + " bytes)");
                    OnAudioReceived?.Invoke(audioData);
                });
            }
        }

        // Seguir escuchando de forma continua
        networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
    }

    // --- ENVÍO DE TEXTO ---
    public void SendText(string message)
    {
        if (!isConnected || networkStream == null) return;
        byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
        SendDataWithType(0, data);
    }

    // --- ENVÍO DE IMAGEN ---
    public void SendImage(Texture2D texture)
    {
        if (!isConnected || networkStream == null) return;
        byte[] data = texture.EncodeToPNG();
        SendDataWithType(1, data);
    }

    // --- ENVÍO DE AUDIO ---
    public void SendAudio(byte[] audioData)
    {
        if (!isConnected || networkStream == null) return;
        SendDataWithType(2, audioData);
    }

    // --- MÉTODO GENERAL DE ENVÍO ---
    private void SendDataWithType(byte type, byte[] data)
    {
        try
        {
            // Paquete = [1 byte tipo] + [4 bytes tamaño] + [contenido]
            byte[] length = BitConverter.GetBytes(data.Length);
            byte[] packet = new byte[1 + 4 + data.Length];
            packet[0] = type;
            Array.Copy(length, 0, packet, 1, 4);
            Array.Copy(data, 0, packet, 5, data.Length);

            // Enviar al servidor
            networkStream.Write(packet, 0, packet.Length);
            networkStream.Flush();
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Failed to send data: " + e.Message);
        }
    }

    // --- DESCONECTAR DEL SERVIDOR ---
    public void Disconnect()
    {
        if (!isConnected) return;

        networkStream?.Close();
        client?.Close();
        isConnected = false;
        Debug.Log("🔌 Disconnected from server.");
    }
}
