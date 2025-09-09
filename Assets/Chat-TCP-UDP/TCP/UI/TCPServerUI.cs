using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TCPServerUI : MonoBehaviour
{
    // --- CONFIGURACIÓN DEL SERVIDOR ---
    public int serverPort = 5555;                  // Puerto en el que el servidor escuchará conexiones
    [SerializeField] private TCPServer _server;    // Referencia al objeto servidor TCP
    [SerializeField] private TMP_InputField messageInput; // Entrada de texto para enviar mensajes

    // --- BOTONES DE LA UI ---
    [Header("Buttons")]
    [SerializeField] private Button sendTextButton;
    [SerializeField] private Button sendImageButton;
    [SerializeField] private Button recordButton;
    [SerializeField] private Button stopRecordButton;

    // --- INTERFAZ DEL CHAT ---
    [Header("Chat UI")]
    [SerializeField] private Transform chatContent;   // Contenedor para mensajes dentro del scroll
    [SerializeField] private GameObject textPrefab;   // Prefab para mensajes de texto
    [SerializeField] private GameObject imagePrefab;  // Prefab para mensajes de imagen
    [SerializeField] private GameObject audioPrefab;  // Prefab para mensajes de audio
    [SerializeField] private ScrollRect scrollRect;   // Scroll que muestra los mensajes

    // --- AUDIO ---
    [Header("Audio")]
    [SerializeField] private AudioRecorder recorder; // Manejador de grabación de audio

    private void Start()
    {
        // Configuración de los botones y sus acciones
        sendTextButton.onClick.AddListener(SendServerMessage);
        sendImageButton.onClick.AddListener(SendServerImage);
        recordButton.onClick.AddListener(StartRecording);
        stopRecordButton.onClick.AddListener(StopAndSendRecording);

        // Suscripción a eventos del servidor para manejar mensajes entrantes
        _server.OnTextReceived += (msg) => AddTextMessage("Cliente: " + msg);
        _server.OnImageReceived += (tex) => AddImageMessage(tex, "Cliente");
        _server.OnAudioReceived += (data) => AddAudioMessage(data, "Cliente");
    }

    // --- INICIO DEL SERVIDOR ---
    public void StartServer()
    {
        _server.StartServer(serverPort);
        AddTextMessage("Servidor iniciado en puerto: " + serverPort);
    }

    #region --- Sending ---
    // --- ENVÍO DE MENSAJES DEL SERVIDOR AL CLIENTE ---

    // Enviar texto al cliente
    public void SendServerMessage()
    {
        if (!_server.isServerRunning) // Validación: verificar si el servidor está activo
        {
            Debug.Log("The server is not running");
            return;
        }

        if (string.IsNullOrEmpty(messageInput.text)) // Validación: evitar mensajes vacíos
        {
            Debug.Log("The chat entry is empty");
            return;
        }

        string message = messageInput.text;
        _server.SendText(message);               // Enviar el mensaje al cliente
        AddTextMessage("Servidor: " + message);  // Mostrar en la UI
        messageInput.text = "";                  // Limpiar campo de entrada
    }

    // Enviar imagen (solo disponible dentro del editor de Unity)
    public void SendServerImage()
    {
#if UNITY_EDITOR
        if (!_server.isServerRunning) return;

        // Abrir selector de archivos para cargar la imagen
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select image", "", "png,jpg");
        if (string.IsNullOrEmpty(path)) return;

        // Convertir archivo a textura
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        // Enviar y mostrar en UI
        _server.SendImage(tex);
        AddImageMessage(tex, "Servidor");
#endif
    }

    // Comenzar grabación de audio
    public void StartRecording() => recorder.StartRecording();

    // Detener grabación, convertir a bytes y enviar al cliente
    public void StopAndSendRecording()
    {
        recorder.StopRecording();
        byte[] audioData = recorder.GetRecordedData();
        if (audioData != null)
        {
            _server.SendAudio(audioData); // Enviar audio al cliente
            CreateAudioMessage(recorder.BytesToAudioClip(audioData), "Servidor"); // Mostrar en UI
        }
    }
    #endregion

    #region --- Receiving ---
    // --- RECEPCIÓN DE MENSAJES DESDE EL CLIENTE ---

    // Mostrar mensaje de texto recibido
    public void AddTextMessage(string message)
    {
        GameObject go = Instantiate(textPrefab, chatContent);
        go.GetComponent<TMP_Text>().text = message;
        ScrollToBottom();
    }

    // Mostrar imagen recibida
    public void AddImageMessage(Texture2D tex, string sender)
    {
        GameObject go = Instantiate(imagePrefab, chatContent);
        go.GetComponent<RawImage>().texture = tex;

        // Etiqueta opcional con el remitente
        TMP_Text label = go.GetComponentInChildren<TMP_Text>();
        if (label != null) label.text = sender + " (Imagen)";

        ScrollToBottom();
    }

    // Mostrar audio recibido
    public void AddAudioMessage(byte[] audioData, string sender)
    {
        AudioClip clip = recorder.BytesToAudioClip(audioData);
        CreateAudioMessage(clip, sender);
    }
    #endregion

    #region --- UI Helpers ---
    // --- MÉTODOS AUXILIARES PARA LA UI ---

    // Crear mensaje de audio en la interfaz con botón de reproducción
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

    // Forzar scroll siempre hacia abajo para mostrar el último mensaje
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
    #endregion
}
