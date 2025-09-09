using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;

    public bool isServerRunning = false;

    public event Action<string> OnTextReceived;
    public event Action<byte[]> OnRawDataReceived;

    public void StartUDPServer(int port)
    {
        udpServer = new UdpClient(port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        Debug.Log("✅ UDP Server started on port " + port);
        udpServer.BeginReceive(ReceiveData, null);
        isServerRunning = true;
    }

    private void ReceiveData(IAsyncResult result)
    {
        byte[] receivedBytes = udpServer.EndReceive(result, ref remoteEndPoint);

        // Fire raw event
        OnRawDataReceived?.Invoke(receivedBytes);

        // Optional: treat it as text
        string msg = System.Text.Encoding.UTF8.GetString(receivedBytes);
        OnTextReceived?.Invoke(msg);

        udpServer.BeginReceive(ReceiveData, null);
    }

    public void SendText(string message)
    {
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(message);
        udpServer.Send(sendBytes, sendBytes.Length, remoteEndPoint);
    }

    public void SendRawData(byte[] data)
    {
        udpServer.Send(data, data.Length, remoteEndPoint);
    }
}
