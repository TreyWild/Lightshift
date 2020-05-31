using Lightshift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AudioObject : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
    }


    public void PlayClip(AudioClip clip) 
    {
        _audioSource.mute = false;
        UpdateVolume();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.mute = true;
            gameObject.SetActive(false);
        }
    }

    public void UpdateVolume() 
    {
        _audioSource.volume = Settings.soundEffectVolume / 2;
    }
}
