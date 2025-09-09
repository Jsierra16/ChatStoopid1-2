using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TCPServerUI : MonoBehaviour
{
    public int serverPort = 5555;
    [SerializeField] private TCPServer _server;
    [SerializeField] private TMP_InputField messageInput;

    [Header("Buttons")]
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
        sendTextButton.onClick.AddListener(SendServerMessage);
        sendImageButton.onClick.AddListener(SendServerImage);
        recordButton.onClick.AddListener(StartRecording);
        stopRecordButton.onClick.AddListener(StopAndSendRecording);

        _server.OnTextReceived += (msg) => AddTextMessage("Cliente: " + msg);
        _server.OnImageReceived += (tex) => AddImageMessage(tex, "Cliente");
        _server.OnAudioReceived += (data) => AddAudioMessage(data, "Cliente");
    }

    public void StartServer()
    {
        _server.StartServer(serverPort);
        AddTextMessage("Servidor iniciado en puerto: " + serverPort);
    }

    #region --- Sending ---

    public void SendServerMessage()
    {
        if (!_server.isServerRunning)
        {
            Debug.Log("The server is not running");
            return;
        }

        if (string.IsNullOrEmpty(messageInput.text))
        {
            Debug.Log("The chat entry is empty");
            return;
        }

        string message = messageInput.text;
        _server.SendText(message);
        AddTextMessage("Servidor: " + message);
        messageInput.text = "";
    }

    public void SendServerImage()
    {
#if UNITY_EDITOR
        if (!_server.isServerRunning) return;

        string path = UnityEditor.EditorUtility.OpenFilePanel("Select image", "", "png,jpg");
        if (string.IsNullOrEmpty(path)) return;

        byte[] bytes = System.IO.File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        _server.SendImage(tex);
        AddImageMessage(tex, "Servidor");
#endif
    }

    public void StartRecording() => recorder.StartRecording();

    public void StopAndSendRecording()
    {
        recorder.StopRecording();
        byte[] audioData = recorder.GetRecordedData();
        if (audioData != null)
        {
            _server.SendAudio(audioData);
            CreateAudioMessage(recorder.BytesToAudioClip(audioData), "Servidor");
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
        playButton.onClick.AddListener(() =>
        {
            audioSource.Play();
        });

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
