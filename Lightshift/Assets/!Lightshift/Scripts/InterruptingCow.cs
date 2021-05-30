using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptingCow : MonoBehaviour
{

    [SerializeField] List<AudioClip> _clips = new List<AudioClip>();
    [SerializeField] AudioSource _audioSource;

    private int _duration;
    private bool _enabled;

    public bool IGNORE_DEV_MODE;
    public void Awake()
    {
        if (IGNORE_DEV_MODE)
            return;

        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey("interruptingCow") && PlayerPrefs.GetInt("interruptingCow", 0) != 0)
        {
            Init();
        }
    }

    public void UpdateSettings(int duration)
    {
        PlayerPrefs.SetInt("interruptingCow", duration);
        PlayerPrefs.Save();

        Init(); 
    }

    public void Init(int duration = 300)
    {
        _duration = PlayerPrefs.GetInt("interruptingCow");

        if (_duration == 0)
        {
            _enabled = false;
            return;
        }
        else _enabled = true;


        StartCoroutine(Moo());
    }

    private IEnumerator Moo() 
    {
        while (true)
        {
            if (_enabled)
                PlayMoo();

            yield return new WaitForSeconds(_duration);
        }
    }

    private void PlayMoo() 
    {
        var mooEffect = _clips[UnityEngine.Random.Range(0, _clips.Count)];

        _audioSource.clip = mooEffect;
        _audioSource.Play();

    }
}
