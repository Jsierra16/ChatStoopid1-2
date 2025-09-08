using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayButton : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(PlayVideo);
    }

    private void PlayVideo()
    {
        if (videoPlayer != null && !videoPlayer.isPlaying)
            videoPlayer.Play();
    }
}
