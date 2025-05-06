//using TMPro;
//using UnityEngine;
//using Fusion;
//using UnityEngine.Video;

//public class karaoke_menu : NetworkBehaviour
//{
//    public TextMeshProUGUI volume_text; // Change TMPro to TextMeshProUGUI
//    public TextMeshProUGUI pause_text; // Change TMPro to TextMeshProUGUI
//    public VideoPlayer videoPlayer;
//    public void play_random()
//    {
//        Debug.Log("Playing random song");
//        int random = Random.Range(1, 5);
//        string videoPath = random + ".mp4";
//        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoPath);
//        videoPlayer.Play();
//    }
//    public void exit_play()
//    {
//        Debug.Log("Exiting play");
//        videoPlayer.Stop();
//    }

//    int pause_index = 2;
//    public void pause_video()
//    {
//        Debug.Log("Pausing video");
//        if (videoPlayer.isPlaying == true || pause_index == 1)
//        {
//            if (pause_index == 1)
//            {
//                videoPlayer.Play();
//                pause_index = 2;
//                pause_text.text = "Pause";
//            }
//            else
//            {
//                videoPlayer.Pause();
//                pause_index = 1;
//                pause_text.text = "Play";
//            }
//        }
//    }
//    int index_volume = 4;
//    public void volume()
//    {
//        Debug.Log("Changing volume");
//        if (index_volume == 1)
//        {
//            videoPlayer.SetDirectAudioVolume(0, 0.5f);
//            index_volume = 2;
//            volume_text.text = "Volume: 50%";
//        }
//        else if (index_volume == 2)
//        {
//            videoPlayer.SetDirectAudioVolume(0, 0.75f);
//            index_volume = 3;
//            volume_text.text = "Volume: 75%";
//        }
//        else if (index_volume == 3)
//        {
//            videoPlayer.SetDirectAudioVolume(0, 1.0f);
//            index_volume = 4;
//            volume_text.text = "Volume: 100%";
//        }
//        else if (index_volume == 4)
//        {
//            videoPlayer.SetDirectAudioVolume(0, 0f);
//            index_volume = 0;
//            volume_text.text = "Volume: 0%";
//        }
//        else if (index_volume == 0)
//        {
//            videoPlayer.SetDirectAudioVolume(0, 0.25f);
//            index_volume = 1;
//            volume_text.text = "Volume: 25%";
//        }
//    }
//}
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
public class karaoke_menu : NetworkBehaviour
{
    public TextMeshProUGUI volume_text;
    public TextMeshProUGUI pause_text;
    public VideoPlayer videoPlayer;

    [Networked] 
    private int videoIndex { get; set; }

    [Networked]
    private bool isPaused { get; set; }

    [Networked]
    private int volumeLevel { get; set; }

    public override void Spawned()
    {
        ApplyVideo(videoIndex);
        ApplyPause(isPaused);
        ApplyVolume(volumeLevel);
    }

    // ---------- PLAY RANDOM 
    public void play_random()
    {
        RpcPlayRandom();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcPlayRandom()
    {
        videoIndex = Random.Range(1, 5);
        isPaused = false;
    }

    private void ApplyVideo(int index)
    {
        Debug.Log($"Playing video {index}");
        string videoPath = $"{index}.mp4";
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoPath);
        videoPlayer.Play();
        pause_text.text = "Pause";
    }

    // ---------- EXIT 
    public void exit_play()
    {
        RpcExitPlay();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcExitPlay()
    {
        videoPlayer.Stop();
    }

    // ---------- PAUSE 
    public void pause_video()
    {
        RpcTogglePause();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcTogglePause()
    {
        isPaused = !isPaused;
    }

    private void ApplyPause(bool paused)
    {
        if (paused)
        {
            videoPlayer.Pause();
            pause_text.text = "Play";
        }
        else
        {
            videoPlayer.Play();
            pause_text.text = "Pause";
        }
    }

    // ---------- VOLUME 
    public void volume()
    {
        RpcChangeVolume();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcChangeVolume()
    {
        volumeLevel = (volumeLevel + 1) % 5;
    }

    private void ApplyVolume(int level)
    {
        float volume = 0f;
        string text = "";

        switch (level)
        {
            case 0: volume = 0f; text = "Volume: 0%"; break;
            case 1: volume = 0.25f; text = "Volume: 25%"; break;
            case 2: volume = 0.5f; text = "Volume: 50%"; break;
            case 3: volume = 0.75f; text = "Volume: 75%"; break;
            case 4: volume = 1f; text = "Volume: 100%"; break;
        }

        videoPlayer.SetDirectAudioVolume(0, volume);
        volume_text.text = text;
        Debug.Log($"Volume set to {volume}");
    }
}
