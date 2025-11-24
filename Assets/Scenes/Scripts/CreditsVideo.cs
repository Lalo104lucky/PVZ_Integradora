using UnityEngine;
using UnityEngine.Video;

public class CreditsVideo : MonoBehaviour
{

    public GameObject menuCanvas;
    public GameObject videoPanel;

    private VideoPlayer videoPlayer;


    void Start()
    {
    }

    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Escape) && videoPanel.activeInHierarchy)
        {
            ExitCredits();
        }
    }

    public void StartCredits()
    {
        menuCanvas.SetActive(false);
        videoPanel.SetActive(true); 

        if (videoPlayer == null) 
        {
            videoPlayer = videoPanel.GetComponent<VideoPlayer>();
            
            videoPlayer.loopPointReached += EndReached;
        }

        videoPlayer.Play();
    }

    void EndReached(VideoPlayer vp)
    {
        vp.Stop();
        videoPanel.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void ExitCredits()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }
        videoPanel.SetActive(false);
        menuCanvas.SetActive(true);
    }
}
