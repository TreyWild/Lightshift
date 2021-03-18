using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnEnable : MonoBehaviour
{
    public List<AudioClip> _sounds;

    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
    }
    private void OnEnable()
    {
        if (_sounds != null && _sounds.Count > 0)
        {
            _audioSource.clip = _sounds[Random.Range(0, _sounds.Count)];
            _audioSource.Play();
        }
    }
}
