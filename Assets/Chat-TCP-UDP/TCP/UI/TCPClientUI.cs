using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TCPClientUI : MonoBehaviour
{
    public int serverPort = 5555;
    public string serverAddress = "127.0.0.1";
    [SerializeField] private TCPClient _client;
    [SerializeField] private TMP_InputField messageInput;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI chatOutput; // Place to show messages

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

        // Also display locally
        ShowMessage($"You: {message}");

        // Clear input field
        messageInput.text = "";
    }

    public void ConnectClient()
    {
        _client.ConnectToServer(serverAddress, serverPort);
        ShowMessage("✅ Attempting to connect to server...");
    }

    private void ShowMessage(string text)
    {
        if (chatOutput != null)
        {
            chatOutput.text += text + "\n"; // Append new line
        }
    }
}
