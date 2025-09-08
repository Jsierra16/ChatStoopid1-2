using System;
using System.Net.Sockets;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream networkStream;
    private byte[] receiveBuffer;

    public bool isConnected;

    // Events for received data
    public Action<string> OnTextReceived;
    public Action<Texture2D> OnImageReceived;
    public Action<byte[]> OnAudioReceived; // 🔥 Added for audio

    public void ConnectToServer(string ip, int port)
    {
        try
        {
            client = new TcpClient();
            client.Connect(ip, port);
            networkStream = client.GetStream();
            isConnected = true;

            Debug.Log("✅ Connected to server: " + ip + ":" + port);

            receiveBuffer = new byte[client.ReceiveBufferSize];
            networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Connection failed: " + e.Message);
        }
    }

    private void ReceiveData(IAsyncResult result)
    {
        if (networkStream == null) return;

        int bytesRead = networkStream.EndRead(result);

        if (bytesRead <= 0)
        {
            Debug.Log("⚠️ Disconnected from server.");
            client.Close();
            isConnected = false;
            return;
        }

        int index = 0;
        while (index < bytesRead)
        {
            byte type = receiveBuffer[index];
            index++;

            int length = BitConverter.ToInt32(receiveBuffer, index);
            index += 4;

            byte[] data = new byte[length];
            Array.Copy(receiveBuffer, index, data, 0, length);
            index += length;

            if (type == 0) // Text
            {
                string message = System.Text.Encoding.UTF8.GetString(data);
                Debug.Log("📩 Text from server: " + message);
                OnTextReceived?.Invoke(message);
            }
            else if (type == 1) // Image
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(data);
                Debug.Log("🖼️ Image from server!");
                OnImageReceived?.Invoke(tex);
            }
            else if (type == 2) // Audio
            {
                Debug.Log("🎵 Audio from server (" + data.Length + " bytes)");
                OnAudioReceived?.Invoke(data);
            }
        }

        networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
    }

    public void SendText(string message)
    {
        if (!isConnected || networkStream == null) return;
        byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
        SendDataWithType(0, data);
    }

    public void SendImage(Texture2D texture)
    {
        if (!isConnected || networkStream == null) return;
        byte[] data = texture.EncodeToPNG();
        SendDataWithType(1, data);
    }

    public void SendAudio(byte[] audioData) // 🔥 New
    {
        if (!isConnected || networkStream == null) return;
        SendDataWithType(2, audioData);
    }

    private void SendDataWithType(byte type, byte[] data)
    {
        try
        {
            byte[] length = BitConverter.GetBytes(data.Length);
            byte[] packet = new byte[1 + 4 + data.Length];
            packet[0] = type;
            Array.Copy(length, 0, packet, 1, 4);
            Array.Copy(data, 0, packet, 5, data.Length);

            networkStream.Write(packet, 0, packet.Length);
            networkStream.Flush();
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Failed to send data: " + e.Message);
        }
    }

    public void Disconnect()
    {
        if (!isConnected) return;

        networkStream?.Close();
        client?.Close();
        isConnected = false;
        Debug.Log("🔌 Disconnected from server.");
    }
}
