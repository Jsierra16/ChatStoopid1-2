using UnityEngine;

public class AudioChatManager : MonoBehaviour
{
    public AudioRecorder recorder;
    public TCPClient client;   // assign if this is a client
    public TCPServer server;   // assign if this is a server

    public void StartRecording()
    {
        recorder.StartRecording();
    }

    public void StopAndSendRecording()
    {
        recorder.StopRecording();
        byte[] audioData = recorder.GetRecordedData();

        if (audioData == null) return;

        if (client != null)
        {
            client.SendAudio(audioData);
            Debug.Log("📤 Sent audio from CLIENT (" + audioData.Length + " bytes)");
        }
        else if (server != null)
        {
            server.SendAudio(audioData);
            Debug.Log("📤 Sent audio from SERVER (" + audioData.Length + " bytes)");
        }
        else
        {
            Debug.LogWarning("⚠️ No TCPClient or TCPServer assigned to AudioChatManager.");
        }
    }
}
