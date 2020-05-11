using Lightshift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    [SerializeField] private ParticleSystem _overDriveParticles;
    [SerializeField] private ParticleSystem _particleSystem;

    private AudioSource _audioSource;
    private Light _cacheLight;
    private bool _isActive;
    private bool _overDriveActive;
    //public bool Overdrive;
    //{
    //    get => _overdrive;
    //    set {
    //        if (_overdrive == false)
    //            _overdriveTrailRenderer.emitting = false;
    //        else _overdriveTrailRenderer.emitting = true;
    //        _overdrive = value;
    //    }
    //}

    private void Awake()
    {
        _particleSystem.GetComponent<Renderer>().sortingOrder = SortingOrders.ENGINE;
        _overDriveParticles.GetComponent<Renderer>().sortingOrder = SortingOrders.OVERDRIVE_ENGINE;
    }

    private void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();

        _audioSource.loop = true;
        _audioSource.volume = Settings.Instance.soundEffectVolume;
        _audioSource.mute = true;
        _audioSource.Play();

        _cacheLight = transform.GetComponentInChildren<Light>().GetComponent<Light>();
    }

    public void StartThruster(bool overDrive)
    {
        _isActive = true;
        _overDriveActive = overDrive;
    }

    public void StopThruster()
    {
        _isActive = false;
    }

    private void Update()
    {
        if (_cacheLight != null)
        {
            // Set the intensity based on the number of particles
            var divider = 30;
            if (_overDriveActive)
                divider = 10;

            _cacheLight.intensity = _particleSystem.particleCount / divider;
        }

        if (_isActive)
        {

            // ...and if audio is muted...
            if (_audioSource.mute)
            {
                // Unmute the audio
                _audioSource.mute = false;
            }
            // If the audio volume is lower than the sound effect volume...
            var volume = Settings.Instance.soundEffectVolume;
            if (!_overDriveActive)
                volume /= 2;
            if (_audioSource.volume < volume)
            {
                // ...fade in the sound (to avoid clicks if just played straight away)
                _audioSource.volume += 1f * Time.deltaTime;
            }
            else _audioSource.volume = volume;

            // If the particle system is intact...
            if (_particleSystem != null)
            {
                _particleSystem.Play();

                if (_overDriveActive)
                    _overDriveParticles.Play();
                else _overDriveParticles.Stop();
            }
        }
        else 
        {

            if (_audioSource.volume > 0.01f)
            {
                // ...fade out volume
                _audioSource.volume -= 5f * Time.deltaTime;
            }
            else
            {
                // ...and mute it when it has faded out
                _audioSource.mute = true;
            }

            // If the particle system is intact...
            if (_particleSystem != null)
            {
                _particleSystem.Stop();
                _overDriveParticles.Stop();
            }
        }
    }

    public void SetColor(Color color) 
    {
        _cacheLight.color = color;
    }
}
