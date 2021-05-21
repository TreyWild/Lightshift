
using Lightshift;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioMixer _mixer;

    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioSource _musicSource;

    private void OnDestroy()
    {
        Instance = null;
        _mixer = null;
        _menuMusic = null;
        _musicSource = null;
    }

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (SceneManager.GetActiveScene().name == "_AUTHENTICATION_")
            PlayMusicTrack(_menuMusic);
        else
        {
            var solarSystem = GalaxyManager.GetGalaxy();
            if (solarSystem != null)
                PlayMusicTrack(solarSystem.Music);
        }
    }

    private void PlayMusicTrack(AudioClip clip)
    {
        if (_musicSource.clip != null)
            if (_musicSource.clip.name == clip.name)
                return;

        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void UpdateSoundSetting()
    {

    }

    public void SetMusicVolume(float volume) 
    {
        if (volume == 0)
            volume = 0.001f;
        _mixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20);
    }

    public void SetEffectsVolume(float volume)
    {
        if (volume == 0)
            volume = 0.001f;
        _mixer.SetFloat("EffectsVolume", Mathf.Log(volume) * 20);
    }

    public void SetGlobalVolume(float volume)
    {
        if (volume == 0)
            volume = 0.001f;
        _mixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20);
    }
}
