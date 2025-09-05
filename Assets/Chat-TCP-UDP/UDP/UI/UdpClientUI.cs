using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UdpClientUI : MonoBehaviour
{
    public int serverPort = 5555;
    public string serverAddress = "127.0.0.1";
    [SerializeField] private UDPClient _client;
    [SerializeField] private TMP_InputField messageInput;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI chatOutput; // Assign in Inspector

    private void Awake()
    {
        // Hook into client received message event (if implemented in UDPClient)
        if (_client != null)
        {
            _client.OnMessageReceived += HandleIncomingMessage;
        }
    }

    public void SendClientMessage()
    {
        if (!_client.isServerConnected)
        {
            ShowMessage("⚠ The client is not connected");
            return;
        }

        if (string.IsNullOrEmpty(messageInput.text))
        {
            ShowMessage("⚠ The chat entry is empty");
            return;
        }

        string message = messageInput.text;
        _client.SendData(message);

        // Show locally
        ShowMessage($"You: {message}");

        // Clear input field
        messageInput.text = "";
    }

    public void ConnectClient()
    {
        _client.StartUDPClient(serverAddress, serverPort);
        ShowMessage("✅ Attempting to connect to server...");
    }

    private void HandleIncomingMessage(string msg)
    {
        ShowMessage($"Server: {msg}");
    }

    private void ShowMessage(string text)
    {
        if (chatOutput != null)
        {
            chatOutput.text += text + "\n";
        }
    }
}
