
using Lightshift;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public GameObject audioObjectPrefab;

    [SerializeField] private AudioClip _explosionSoundEffect;

    [SerializeField] private List<AudioClip> _musicClips;

    [SerializeField] private AudioClip[] _titleScreenMusic;

    private List<AudioObject> _audioObjects = new List<AudioObject>();

    private List<AudioSource> _musicSources = new List<AudioSource>();

    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Singleton instance.
    public static SoundManager Instance = null;

    private int _activeMusicSource = 0;
    private AudioSource GetMusicSource() => _musicSource;

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

        for (int i = 0; i < 2; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.ignoreListenerVolume = true;
            _musicSources.Add(source);
        }

        _musicSource = _musicSources[0];

        UpdateVolume();
    }

    // Play a single clip through the sound effects source.
    public static void Play(AudioClip clip, Vector2 position)
    {
        var obj = Instance.GetUsableAudio();
        obj.gameObject.SetActive(true);
        obj.gameObject.transform.position = position;
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
            var gameObj = Instantiate(audioObjectPrefab);
            obj = gameObj.GetComponent<AudioObject>();
            _audioObjects.Add(obj);
        }
        return obj;
    }

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip)
    {
        try
        {
            GetMusicSource().clip = clip;
            UpdateVolume();
            GetMusicSource().Play();
        }
        catch { }
    }

    private void Update()
    {
        //if (!GetMusicSource().isPlaying)
        //{
        //    if (SceneManager.GetActiveScene().buildIndex == 0)
        //        PlayTitleScreenMusic();
        //    else
        //        PlayRandomMusic();
        //}

        if (_oldMusicSource != null)
        {
            if (_oldMusicSource.mute)
                return;

            if (_oldMusicSource.volume > 0)
                _oldMusicSource.volume -= Time.deltaTime;
            else
            {
                _oldMusicSource.mute = true;
                _oldMusicSource.Stop();
            }
        }

        if (_musicSource != null)
        {
            if (_musicSource.volume < Settings.musicVolume)
            {
                _musicSource.volume += (Settings.musicVolume) * Time.deltaTime;
            }
        }
    }
    public void PlayRandomMusic()
    {
        var clip = _musicClips[UnityEngine.Random.Range(0, _musicClips.Count)];
        PlayMusic(clip);
    }

    public void UpdateVolume()
    {
        if (Settings.musicVolume > 0.05f)
            foreach (var source in _musicSources)
                source.mute = false;
        else foreach (var source in _musicSources)
                source.mute = true;

        foreach (var source in _musicSources)
            source.volume = Settings.musicVolume;

        foreach (var audio in _audioObjects)
            audio.UpdateVolume();
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level > 0)
            PlayRandomMusic();
        else PlayTitleScreenMusic();
    }

    public void PlayTitleScreenMusic() 
    {
        PlayMusic(_titleScreenMusic[UnityEngine.Random.Range(0, _titleScreenMusic.Length)]);
    }

    private AudioSource _oldMusicSource;
    private AudioSource _musicSource;
    public static void PlayMusicWithFade(AudioClip music) 
    {
        Instance._oldMusicSource = Instance._musicSource;
        Instance._musicSource = Instance._musicSources.FirstOrDefault(s => s != Instance._oldMusicSource);

        Instance._musicSource.clip = music;
        Instance._musicSource.volume = 0;
        Instance._musicSource.mute = false;
        Instance._musicSource.Play();
    }
}