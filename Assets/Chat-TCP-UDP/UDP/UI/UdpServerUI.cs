using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UdpServerUI : MonoBehaviour
{
    public int serverPort = 5555;
    [SerializeField] private UDPServer _server;
    [SerializeField] private TMP_InputField messageInput;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI chatOutput; // Assign in Inspector

    private void Awake()
    {
        // Hook into server received message event (if implemented in UDPServer)
        if (_server != null)
        {
            _server.OnMessageReceived += HandleIncomingMessage;
        }
    }

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
        _server.StartUDPServer(serverPort);
        ShowMessage("✅ Server started...");
    }

    private void HandleIncomingMessage(string msg)
    {
        ShowMessage($"Client: {msg}");
    }

    private void ShowMessage(string text)
    {
        if (chatOutput != null)
        {
            chatOutput.text += text + "\n"; // Append with newline
        }
    }
}
