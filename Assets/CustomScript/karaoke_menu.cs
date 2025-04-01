using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

public class karaoke_menu : MonoBehaviour
{
    public TextMeshProUGUI volume_text; // Change TMPro to TextMeshProUGUI
    public TextMeshProUGUI pause_text; // Change TMPro to TextMeshProUGUI
    public VideoPlayer videoPlayer;
    public void play_random()
    {
        Debug.Log("Playing random song");
        int random = Random.Range(1, 5);
        string videoPath = random + ".mp4";
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoPath);
        videoPlayer.Play();
    }
    public void exit_play()
    {
        Debug.Log("Exiting play");
        videoPlayer.Stop();
    }

    int pause_index = 2;
    public void pause_video()
    {
        Debug.Log("Pausing video");
        if (videoPlayer.isPlaying == true || pause_index == 1)
        {
            if (pause_index == 1)
            {
                videoPlayer.Play();
                pause_index = 2;
                pause_text.text = "Pause";
            }
            else
            {
                videoPlayer.Pause();
                pause_index = 1;
                pause_text.text = "Play";
            }
        }
    }
    int index_volume = 4;
    public void volume()
    {
        Debug.Log("Changing volume");
        if (index_volume == 1)
        {
            videoPlayer.SetDirectAudioVolume(0, 0.5f);
            index_volume = 2;
            volume_text.text = "Volume: 50%";
        }
        else if (index_volume == 2)
        {
            videoPlayer.SetDirectAudioVolume(0, 0.75f);
            index_volume = 3;
            volume_text.text = "Volume: 75%";
        }
        else if (index_volume == 3)
        {
            videoPlayer.SetDirectAudioVolume(0, 1.0f);
            index_volume = 4;
            volume_text.text = "Volume: 100%";
        }
        else if (index_volume == 4)
        {
            videoPlayer.SetDirectAudioVolume(0, 0f);
            index_volume = 0;
            volume_text.text = "Volume: 0%";
        }
        else if (index_volume == 0)
        {
            videoPlayer.SetDirectAudioVolume(0, 0.25f);
            index_volume = 1;
            volume_text.text = "Volume: 25%";
        }
    }
}
