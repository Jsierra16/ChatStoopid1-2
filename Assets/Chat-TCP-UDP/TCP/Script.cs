using UnityEngine;
using UnityEngine.UI;

public class Script : MonoBehaviour
{
    [SerializeField] private RawImage img = default;

    private WebCamTexture webCam;

    void Start()
    {
        webCam = new WebCamTexture();
        if (!webCam.isPlaying) webCam.Play();
        img.texture = webCam;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
