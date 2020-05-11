﻿
using Lightshift;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip _explosionSoundEffect;

    [SerializeField] private List<AudioClip> _musicClips;

    [SerializeField] private AudioClip _titleScreenMusic;

    private List<AudioObject> _audioObjects = new List<AudioObject>();

    private AudioSource _musicSource;

    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Singleton instance.
    public static SoundManager Instance = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

        _musicSource = gameObject.AddComponent<AudioSource>();

        _musicSource.ignoreListenerVolume = true;

        UpdateVolume();
    }

    // Play a single clip through the sound effects source.
    public static void Play(AudioClip clip, Vector2 position)
    {
        var obj = Instance.GetUsableAudio();
        obj.gameObject.SetActive(true);
        obj.transform.position = position;
        obj.PlayClip(clip);
    }

    public static void PlayExplosion(Vector2 position)
    {
        Play(Instance._explosionSoundEffect, position);
    }

    private AudioObject GetUsableAudio()
    {
        var obj = _audioObjects.FirstOrDefault(a => !a.gameObject.activeInHierarchy);
        if (obj == null)
        {
            var gameObj = new GameObject("Audio Source");
            gameObj.transform.parent = transform;
            obj = gameObj.AddComponent<AudioObject>();
            _audioObjects.Add(obj);
        }
        return obj;
    }

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        UpdateVolume();
        _musicSource.Play();
    }

    private void Update()
    {
        if (!_musicSource.isPlaying)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                PlayMusic(_titleScreenMusic);
            else
                PlayRandomMusic();
        }
    }
    public void PlayRandomMusic()
    {
        var clip = _musicClips[UnityEngine.Random.Range(0, _musicClips.Count)];
        PlayMusic(clip);
    }

    public void UpdateVolume()
    {
        if (Settings.Instance.soundEffectVolume > 0.05f)
            _musicSource.mute = false;
        else _musicSource.mute = true;

        _musicSource.volume = Settings.Instance.musicVolume;

        Debug.Log(_musicSource.volume);

        foreach (var audio in _audioObjects)
            audio.UpdateVolume();
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level > 0)
            PlayRandomMusic();
        else PlayMusic(_titleScreenMusic);
    }
}