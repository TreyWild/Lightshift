using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private TrailRenderer _overdriveTrailRenderer;
    [SerializeField] private ParticleSystem _particleSystem;
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
        _trailRenderer.sortingOrder = SortingOrders.ENGINE;
        _particleSystem.GetComponent<Renderer>().sortingOrder = SortingOrders.ENGINE;
        _overdriveTrailRenderer.sortingOrder = SortingOrders.OVERDRIVE_ENGINE;
        _trailRenderer.Clear();
        _trailRenderer.emitting = false;
        _overdriveTrailRenderer.Clear();
        _overdriveTrailRenderer.emitting = false;
    }

    public void SetColor(Color color) 
    {
        _trailRenderer.startColor = color;
        _overdriveTrailRenderer.endColor = color;
    }

    public void RunEngine(bool run, bool overDrive) 
    {
        _trailRenderer.emitting = run;
        _overdriveTrailRenderer.emitting = overDrive;

        if (!_particleSystem.isPlaying && run)
            _particleSystem.Play();
        else if (_particleSystem.isPlaying && !run) _particleSystem.Stop();
    }

    public void SetTrailLength(float length) 
    {
        _overdriveTrailRenderer.time = length;
        _trailRenderer.time = length;
    }

    public void SetTrailColor(Color color) 
    {
        _trailRenderer.startColor = color;
        _overdriveTrailRenderer.endColor = color;
    }
}
