using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject panelToHide;
    public GameObject panelToShow;

    public AudioSource clickSound;
    public AudioSource audioToStart;
    public AudioSource audioToStop;

    public void SwitchPanels()
    {
        // Play click sound
        if (clickSound != null)
            clickSound.Play();

        // Hide old panel
        if (panelToHide != null)
            panelToHide.SetActive(false);

        // Show new panel
        if (panelToShow != null)
            panelToShow.SetActive(true);

        // Stop old audio
        if (audioToStop != null && audioToStop.isPlaying)
            audioToStop.Stop();

        // Start new audio
        if (audioToStart != null && !audioToStart.isPlaying)
            audioToStart.Play();
    }
}
