using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TCPClientUI : MonoBehaviour
{
    // --- CONFIGURACIÓN DEL CLIENTE ---
    [Header("Client Settings")]
    public string serverIP = "127.0.0.1";   // Dirección IP del servidor (por defecto localhost)
    public int serverPort = 5555;           // Puerto de conexión
    [SerializeField] private TCPClient _client;          // Referencia al cliente TCP
    [SerializeField] private TMP_InputField messageInput; // Campo de entrada de texto

    // --- BOTONES DE LA UI ---
    [Header("Buttons")]
    [SerializeField] private Button connectButton;
    [SerializeField] private Button sendTextButton;
    [SerializeField] private Button sendImageButton;
    [SerializeField] private Button recordButton;
    [SerializeField] private Button stopRecordButton;

    // --- ELEMENTOS DEL CHAT ---
    [Header("Chat UI")]
    [SerializeField] private Transform chatContent;   // Contenedor para los mensajes
    [SerializeField] private GameObject textPrefab;   // Prefab para mensajes de texto
    [SerializeField] private GameObject imagePrefab;  // Prefab para mensajes con imágenes
    [SerializeField] private GameObject audioPrefab;  // Prefab para mensajes de audio
    [SerializeField] private ScrollRect scrollRect;   // Scroll del chat

    // --- AUDIO ---
    [Header("Audio")]
    [SerializeField] private AudioRecorder recorder; // Grabador de audio

    private void Start()
    {
        // Configuración de botones de UI → asocian acciones
        connectButton.onClick.AddListener(() => ConnectClient());
        sendTextButton.onClick.AddListener(SendClientMessage);
        sendImageButton.onClick.AddListener(SendClientImage);
        recordButton.onClick.AddListener(StartRecording);
        stopRecordButton.onClick.AddListener(StopAndSendRecording);

        // Suscripción a eventos del cliente TCP → manejo de datos recibidos
        _client.OnTextReceived += (msg) => AddTextMessage("Servidor: " + msg);
        _client.OnImageReceived += (tex) => AddImageMessage(tex, "Servidor");
        _client.OnAudioReceived += (data) => AddAudioMessage(data, "Servidor");
    }

    // --- CONEXIÓN AL SERVIDOR ---
    public void ConnectClient()
    {
        _client.ConnectToServer(serverIP, serverPort);

        if (_client.isConnected)
            AddTextMessage("Conectado al servidor en " + serverIP + ":" + serverPort);
    }

    #region --- Sending ---
    // --- MÉTODOS PARA ENVIAR MENSAJES AL SERVIDOR ---

    // Enviar texto
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
        _client.SendText(message);                   // Enviar al servidor
        AddTextMessage("Cliente: " + message);       // Mostrar en la UI
        messageInput.text = "";                      // Limpiar campo
    }

    // Enviar imagen (solo en el editor Unity)
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

    // Iniciar y detener grabación de audio
    public void StartRecording() => recorder.StartRecording();

    public void StopAndSendRecording()
    {
        recorder.StopRecording();
        byte[] audioData = recorder.GetRecordedData();
        if (audioData != null)
        {
            _client.SendAudio(audioData);                            // Enviar audio
            CreateAudioMessage(recorder.BytesToAudioClip(audioData), // Mostrar en UI
                               "Cliente");
        }
    }
    #endregion

    #region --- Receiving ---
    // --- MÉTODOS PARA MOSTRAR MENSAJES RECIBIDOS ---

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

        // Añadir etiqueta opcional con el remitente
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
    // --- MÉTODOS DE APOYO PARA LA INTERFAZ ---

    // Crear un mensaje de audio en la UI con botón de reproducir
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

    // Mantener siempre el scroll en el último mensaje
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
    #endregion
}
