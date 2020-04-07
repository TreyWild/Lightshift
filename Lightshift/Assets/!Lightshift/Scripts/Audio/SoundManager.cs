
using Lightshift;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public int soundEffectCount => _soundEffects.Count;
    [SerializeField] private List<AudioClip> _musicClips;
    [SerializeField] private List<AudioClip> _soundEffects;

    public List<GameObject> _pooledAudioSources;

    // Audio players components.
    public AudioSource EffectsSource;
    public AudioSource MusicSource;

    public GameObject audioPool;

    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Singleton instance.
    public static SoundManager Instance = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

        MusicSource.ignoreListenerVolume = true;
        EffectsSource.ignoreListenerVolume = true;

        audioPool = new GameObject("Audio Pool");
        for (int i = 0; i < 75; i++)
        {
            var audioObject = new GameObject("AudioSource");
            audioObject.transform.parent = audioPool.transform;
            audioObject.AddComponent<AudioSource>();
        }
    }

    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip, Vector2 position, bool isBullet = false)
    {
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        //EffectsSource.pitch = randomPitch;
        //EffectsSource.clip = clip;
        //EffectsSource.Play();

        AudioSource audioSource = null;
        var pooledSources = audioPool.GetComponentsInChildren<AudioSource>();

        foreach (var item in pooledSources)
        {
            if (item.isPlaying)
                continue;

            audioSource = item;
            break;
        }

        if (audioSource == null)
            audioSource = audioPool.AddComponent<AudioSource>();

        if (isBullet)
            audioSource.volume = Settings.Instance.soundEffectVolume / 3;
        else audioSource.volume = Settings.Instance.soundEffectVolume;

        audioSource.transform.position = position;
        audioSource.pitch = randomPitch;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void Play(int id, Vector2 position, bool isBullet = false)
    {
        var soundEffect = _soundEffects[id];
        if (soundEffect != null)
            Play(soundEffect, position, isBullet);
    }

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.volume = Settings.Instance.musicVolume;
        MusicSource.Play();
    }

    // Play a random clip from an array, and randomize the pitch slightly.
    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }

    private void Update()
    {
        if (!MusicSource.isPlaying)
            PlayRandomMusic();
    }
    public void PlayRandomMusic()
    {
        var clip = _musicClips[Random.Range(0, _musicClips.Count)];
        PlayMusic(clip);
    }

}