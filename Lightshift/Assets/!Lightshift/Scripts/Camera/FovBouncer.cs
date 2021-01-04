using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovBouncer : MonoBehaviour
{
    public enum FovHopperTransition 
    {
        Warp,
        Standard, 
        Extra
    }

    public float Speed = 1;
    public float HyperTime = 20;
    private FovHopperTransition _transition = FovHopperTransition.Extra;
    [SerializeField] private AudioClip[] _clips;

    [Header("Camera (Optional)")]
    [SerializeField] private Camera _camera;
    private void Awake()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();

    }

    private int _direction = -1;
    private float _currentHyper;
    void Update()
    {
        _currentHyper -= 1 * Time.deltaTime;

        if (_camera.fieldOfView <= 20 && _direction == -1 && _currentHyper <= 0)
        {
            _currentHyper = 0.3f;
            _direction = 1;
        }

        if (_camera.fieldOfView >= 179 && _direction == 1 && _currentHyper <=0)
        {
            _direction = -1;
            _currentHyper = HyperTime;
            
        }
        if (_currentHyper < 0)
        {
            if (_direction > 0)
                _camera.fieldOfView += Speed * Time.deltaTime;
            else _camera.fieldOfView -= Speed * Time.deltaTime;
        }

        if (_camera.fieldOfView >= 132 && _camera.fieldOfView <= 133 && _transition != FovHopperTransition.Warp && _direction == 1)
        {
            _transition = FovHopperTransition.Warp;
            SoundManager.PlayMusicWithFade(_clips[(int)_transition]);
        }
        else if (_camera.fieldOfView >= 165 && _camera.fieldOfView < 166 && _transition != FovHopperTransition.Standard && _direction == -1)
        {
            _transition = FovHopperTransition.Standard;
            SoundManager.PlayMusicWithFade(_clips[(int)_transition]);
        }
        else if (_camera.fieldOfView >= 88 && _camera.fieldOfView < 89 && _transition != FovHopperTransition.Extra && _direction == -1)
        {
            _transition = FovHopperTransition.Extra;
            SoundManager.PlayMusicWithFade(_clips[(int)_transition]);
        }
    }
}
