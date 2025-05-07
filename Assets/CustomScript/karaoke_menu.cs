using TMPro;
using UnityEngine;
using UnityEngine.Video;
using Fusion;
using System;

[RequireComponent(typeof(NetworkObject))]
public class karaoke_menu : NetworkBehaviour
{
    public TextMeshProUGUI volume_text;
    public TextMeshProUGUI pause_text;
    public VideoPlayer videoPlayer;

    public VideoClip[] clips;

    public struct PlaybackStatus : INetworkStruct
    {
        public long frame;
        public bool isPlaying;
        public int videoIndex;
    }

    [Networked]
    public PlaybackStatus NetworkPlaybackStatus { get; set; }

    private int videoIndex = 0;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        NetworkPlaybackStatus = new PlaybackStatus()
        {
            frame = videoPlayer.frame,
            isPlaying = videoPlayer.isPlaying,
            videoIndex = videoIndex
        };
    }

    public override void Spawned()
    {
        if (HasStateAuthority) return;
        videoPlayer.clip = clips[NetworkPlaybackStatus.videoIndex];
        videoPlayer.frame = NetworkPlaybackStatus.frame;
        if (NetworkPlaybackStatus.isPlaying) videoPlayer.Play();
    }

    public void play_random()
    {
        RpcPlayVideo(UnityEngine.Random.Range(0, clips.Length));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcPlayVideo(int index)
    {
        if (index >= clips.Length) return;
        videoIndex = index;
        videoPlayer.clip = clips[index];
        videoPlayer.Play();
        pause_text.text = "Pause";
    }

    public void exit_play()
    {
        RpcExitPlay();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcExitPlay()
    {
        videoPlayer.Stop();
        pause_text.text = "Play";
    }

    public void pause_video()
    {
        RpcTogglePause();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcTogglePause()
    {
        if (videoPlayer.isPlaying)
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

    public void volume()
    {
        RpcChangeVolume();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcChangeVolume()
    {
        float volume = videoPlayer.GetDirectAudioVolume(0) + 0.25f;
        if (volume > 1) volume = 0;
        string text = $"Volume: {Math.Round(volume * 100)}%";
        videoPlayer.SetDirectAudioVolume(0, volume);
        volume_text.text = text;
    }
}
