using UnityEngine;
using UnityEngine.Video;

public class CreditsVideo : MonoBehaviour
{

    public GameObject menuCanvas;
    public GameObject videoPanel;
    public AudioSource backgroundMusic;

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

        // Stop the background music
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }

        videoPlayer.Play();
    }

    void EndReached(VideoPlayer vp)
    {
        vp.Stop();
        videoPanel.SetActive(false);
        menuCanvas.SetActive(true);
        
        // Resume the background music
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.UnPause();
        }
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
      
        // Resume the background music
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.UnPause();
        }
    }
}
