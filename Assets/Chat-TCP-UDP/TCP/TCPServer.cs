using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private TcpListener tcpListener;
    private TcpClient connectedClient;
    private NetworkStream networkStream;
    private byte[] receiveBuffer;

    public bool isServerRunning;

    // Events for received data
    public Action<string> OnTextReceived;
    public Action<Texture2D> OnImageReceived;
    public Action<byte[]> OnAudioReceived;

    public void StartServer(int port)
    {
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        Debug.Log("✅ Server started on port " + port);
        tcpListener.BeginAcceptTcpClient(HandleIncomingConnection, null);
        isServerRunning = true;
    }

    private void HandleIncomingConnection(IAsyncResult result)
    {
        connectedClient = tcpListener.EndAcceptTcpClient(result);
        networkStream = connectedClient.GetStream();
        Debug.Log("🔗 Client connected: " + connectedClient.Client.RemoteEndPoint);

        receiveBuffer = new byte[connectedClient.ReceiveBufferSize];
        networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);

        // Continue accepting other clients
        tcpListener.BeginAcceptTcpClient(HandleIncomingConnection, null);
    }

    private void ReceiveData(IAsyncResult result)
    {
        int bytesRead = networkStream.EndRead(result);

        if (bytesRead <= 0)
        {
            Debug.Log("❌ Client disconnected.");
            connectedClient.Close();
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
                MainThreadDispatcher.Enqueue(() =>
                {
                    Debug.Log("📩 Texto recibido: " + message);
                    OnTextReceived?.Invoke(message);
                });
            }
            else if (type == 1) // Image
            {
                byte[] imgData = data;
                MainThreadDispatcher.Enqueue(() =>
                {
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imgData);
                    Debug.Log("🖼️ Imagen recibida!");
                    OnImageReceived?.Invoke(tex);
                });
            }
            else if (type == 2) // Audio
            {
                byte[] audioData = data;
                MainThreadDispatcher.Enqueue(() =>
                {
                    Debug.Log("🎵 Audio recibido (" + audioData.Length + " bytes)");
                    OnAudioReceived?.Invoke(audioData);
                });
            }
        }

        networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
    }

    public void SendText(string message)
    {
        if (networkStream == null) return;
        byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
        SendDataWithType(0, data);
    }

    public void SendImage(Texture2D texture)
    {
        if (networkStream == null) return;
        byte[] data = texture.EncodeToPNG();
        SendDataWithType(1, data);
    }

    public void SendAudio(byte[] audioData)
    {
        if (networkStream == null) return;
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
        catch
        {
            Debug.Log("❌ Failed to send data.");
        }
    }
}
