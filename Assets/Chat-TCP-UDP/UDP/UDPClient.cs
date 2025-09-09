using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    public bool isServerConnected = false;

    public event Action<string> OnTextReceived;
    public event Action<byte[]> OnRawDataReceived;

    public void StartUDPClient(string ipAddress, int port)
    {
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        udpClient.BeginReceive(ReceiveData, null);
        isServerConnected = true;
    }

    private void ReceiveData(IAsyncResult result)
    {
        byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEndPoint);

        // Fire raw event
        OnRawDataReceived?.Invoke(receivedBytes);

        // Optional: treat it as text
        string msg = System.Text.Encoding.UTF8.GetString(receivedBytes);
        OnTextReceived?.Invoke(msg);

        udpClient.BeginReceive(ReceiveData, null);
    }

    public void SendText(string message)
    {
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(message);
        udpClient.Send(sendBytes, sendBytes.Length, remoteEndPoint);
    }

    public void SendRawData(byte[] data)
    {
        udpClient.Send(data, data.Length, remoteEndPoint);
    }
}
