using UnityEngine;

public class VideoSender : MonoBehaviour
{
    [Header("References")]
    public TCPClient tcpClient;
    public TCPServer tcpServer;

    [Header("Camera Settings")]
    public Camera captureCamera;
    public int captureWidth = 320;
    public int captureHeight = 240;

    private RenderTexture renderTexture;
    private Texture2D frameTexture;
    private bool isSending = false;

    void Start()
    {
        if (captureCamera != null)
        {
            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
            frameTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

            captureCamera.targetTexture = renderTexture;
        }
    }

    void Update()
    {
        if (isSending)
        {
            CaptureAndSendFrame();
        }
    }

    public void StartSending()
    {
        isSending = true;
    }

    public void StopSending()
    {
        isSending = false;
    }

    private void CaptureAndSendFrame()
    {
        if (captureCamera == null || frameTexture == null) return;

        // Render the camera to the RenderTexture
        RenderTexture.active = renderTexture;
        captureCamera.Render();

        // Copy pixels into the Texture2D
        frameTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        frameTexture.Apply();

        RenderTexture.active = null;

        // ✅ Now we send the Texture2D directly (no byte[] here)
        if (tcpClient != null)
        {
            tcpClient.SendImage(frameTexture);
        }

        if (tcpServer != null)
        {
            tcpServer.SendImage(frameTexture);
        }
    }
}
