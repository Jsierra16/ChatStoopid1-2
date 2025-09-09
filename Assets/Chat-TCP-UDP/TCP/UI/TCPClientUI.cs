using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TCPClientUI : MonoBehaviour
{
    [Header("Client Settings")]
    public string serverIP = "127.0.0.1";   // Default to localhost
    public int serverPort = 5555;

    [SerializeField] private TCPClient _client;
    [SerializeField] private TMP_InputField messageInput;

    [Header("Buttons")]
    [SerializeField] private Button connectButton;
    [SerializeField] private Button sendTextButton;
    [SerializeField] private Button sendImageButton;
    [SerializeField] private Button recordButton;
    [SerializeField] private Button stopRecordButton;

    [Header("Chat UI")]
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private GameObject audioPrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Audio")]
    [SerializeField] private AudioRecorder recorder;

    private void Start()
    {
        connectButton.onClick.AddListener(() => ConnectClient());
        sendTextButton.onClick.AddListener(SendClientMessage);
        sendImageButton.onClick.AddListener(SendClientImage);
        recordButton.onClick.AddListener(StartRecording);
        stopRecordButton.onClick.AddListener(StopAndSendRecording);

        _client.OnTextReceived += (msg) => AddTextMessage("Servidor: " + msg);
        _client.OnImageReceived += (tex) => AddImageMessage(tex, "Servidor");
        _client.OnAudioReceived += (data) => AddAudioMessage(data, "Servidor");
    }

    public void ConnectClient()
    {
        _client.ConnectToServer(serverIP, serverPort);

        if (_client.isConnected)
            AddTextMessage("Conectado al servidor en " + serverIP + ":" + serverPort);
    }

    #region --- Sending ---

    public void SendClientMessage()
    {
        if (!_client.isConnected)
        {
            Debug.Log("The client is not connected");
            return;
        }

        if (string.IsNullOrEmpty(messageInput.text))
        {
            Debug.Log("The chat entry is empty");
            return;
        }

        string message = messageInput.text;
        _client.SendText(message);
        AddTextMessage("Cliente: " + message);
        messageInput.text = "";
    }

    public void SendClientImage()
    {
#if UNITY_EDITOR
        if (!_client.isConnected) return;

        string path = UnityEditor.EditorUtility.OpenFilePanel("Select image", "", "png,jpg");
        if (string.IsNullOrEmpty(path)) return;

        byte[] bytes = System.IO.File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        _client.SendImage(tex);
        AddImageMessage(tex, "Cliente");
#endif
    }

    public void StartRecording() => recorder.StartRecording();

    public void StopAndSendRecording()
    {
        recorder.StopRecording();
        byte[] audioData = recorder.GetRecordedData();
        if (audioData != null)
        {
            _client.SendAudio(audioData);
            CreateAudioMessage(recorder.BytesToAudioClip(audioData), "Cliente");
        }
    }

    #endregion

    #region --- Receiving ---

    public void AddTextMessage(string message)
    {
        GameObject go = Instantiate(textPrefab, chatContent);
        go.GetComponent<TMP_Text>().text = message;
        ScrollToBottom();
    }

    public void AddImageMessage(Texture2D tex, string sender)
    {
        GameObject go = Instantiate(imagePrefab, chatContent);
        go.GetComponent<RawImage>().texture = tex;

        // Optional: add a label above image
        TMP_Text label = go.GetComponentInChildren<TMP_Text>();
        if (label != null) label.text = sender + " (Imagen)";

        ScrollToBottom();
    }

    public void AddAudioMessage(byte[] audioData, string sender)
    {
        AudioClip clip = recorder.BytesToAudioClip(audioData);
        CreateAudioMessage(clip, sender);
    }

    #endregion

    #region --- UI Helpers ---

    private void CreateAudioMessage(AudioClip clip, string sender)
    {
        GameObject newMsg = Instantiate(audioPrefab, chatContent);
        newMsg.GetComponentInChildren<TMP_Text>().text = sender + " (Audio)";
        Button playButton = newMsg.GetComponentInChildren<Button>();
        AudioSource audioSource = newMsg.GetComponent<AudioSource>();

        audioSource.clip = clip;
        playButton.onClick.AddListener(() => audioSource.Play());

        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    #endregion
}
