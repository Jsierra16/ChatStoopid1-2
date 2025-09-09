using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPProtocol : MonoBehaviour, IProtocolUDP
{
    private UdpClient udp;
    private IPEndPoint remoteEndPoint;

    public bool isServerRunning = false;
    public bool isServer = false;
    public bool isConnected = false;

    bool IProtocolUDP.isServer { get => isServer; set => isServer = value; }

    // Events
    public event Action OnConnected;
    public event Action<string> OnDataReceived;    // For text messages
    public event Action<byte[]> OnBytesReceived;   // 🔥 New: For raw data (audio/video)

    public void StartUDP(string ipAddress, int port)
    {
        if (isServer)
        {
            udp = new UdpClient(port);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        }
        else
        {
            udp = new UdpClient();
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }

        udp.BeginReceive(ReceiveData, null);
        isServerRunning = true;

        if (!isServer)
        {
            SendData("HELLO");
        }
    }

    public void ReceiveData(IAsyncResult result)
    {
        byte[] receivedBytes = udp.EndReceive(result, ref remoteEndPoint);

        // Try to interpret as text first
        string receivedMessage = null;
        try
        {
            receivedMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);
        }
        catch { /* not valid text, treat as raw data */ }

        if (isServer)
        {
            if (receivedMessage == "HELLO" && !isConnected)
            {
                isConnected = true;
                SendData("WELCOME");
                Debug.Log("Client connected!");
                OnConnected?.Invoke();
            }
            else if (receivedMessage != null)
            {
                OnDataReceived?.Invoke(receivedMessage);
            }
            else
            {
                OnBytesReceived?.Invoke(receivedBytes);
            }
        }
        else
        {
            if (receivedMessage == "WELCOME" && !isConnected)
            {
                isConnected = true;
                Debug.Log("Connected to the server!");
                OnConnected?.Invoke();
            }
            else if (receivedMessage != null)
            {
                OnDataReceived?.Invoke(receivedMessage);
            }
            else
            {
                OnBytesReceived?.Invoke(receivedBytes);
            }
        }

        udp.BeginReceive(ReceiveData, null);
    }

    // Send text
    public void SendData(string message)
    {
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(message);
        udp.Send(sendBytes, sendBytes.Length, remoteEndPoint);
        Debug.Log("Sent string: " + message);
    }

    // 🔥 Send raw byte[]
    public void SendData(byte[] data)
    {
        udp.Send(data, data.Length, remoteEndPoint);
        Debug.Log("Sent raw bytes (" + data.Length + ")");
    }
}
