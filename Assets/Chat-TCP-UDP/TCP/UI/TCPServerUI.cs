using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TCPServerUI : MonoBehaviour
{
    public int serverPort = 5555;
    [SerializeField] private TCPServer _server;
    [SerializeField] private TMP_InputField messageInput;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI chatOutput; // Place to show messages

    public void SendServerMessage()
    {
        if (!_server.isServerRunning)
        {
            ShowMessage("⚠ The server is not running");
            return;
        }

        if (string.IsNullOrEmpty(messageInput.text))
        {
            ShowMessage("⚠ The chat entry is empty");
            return;
        }

        string message = messageInput.text;
        _server.SendData(message);

        // Show in UI
        ShowMessage($"Server: {message}");

        // Clear input field
        messageInput.text = "";
    }

    public void StartServer()
    {
        _server.StartServer(serverPort);
        ShowMessage("✅ Server started");
    }

    private void ShowMessage(string text)
    {
        if (chatOutput != null)
        {
            chatOutput.text += text + "\n"; // Append new line
        }
    }
}
