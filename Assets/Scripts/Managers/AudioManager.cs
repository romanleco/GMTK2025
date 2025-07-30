using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioClip[] _soundEffectsClips;
    [SerializeField] private AudioClip[] _musicClips;
    [SerializeField] private AudioSource _effectsAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;

    void Start()
    {
        DataContainer loadedData = SaveManager.Instance.Load();
        if(loadedData != null)
        {
            _effectsAudioSource.volume = loadedData.sEffectsVolume;
            _musicAudioSource.volume = loadedData.musicVolume;
        }
    }

    public void PlayMusic(int musicClipIndex, bool playRandom = false)
    {
        if(musicClipIndex <= _musicClips.Length)
        {
            _musicAudioSource.Stop();
            if(playRandom == false)
                _musicAudioSource.clip = _musicClips[musicClipIndex];
            else
                _musicAudioSource.clip = _musicClips[Random.Range(0, _musicClips.Length + 1)];
            _musicAudioSource.Play();
        }
        else
        {
            Debug.LogError("Music Clip Index does not exist");
        }
    }

    public void PlaySFX(int soundEffectClipIndex)
    {
        if(soundEffectClipIndex <= _soundEffectsClips.Length)
        {
            _effectsAudioSource.PlayOneShot(_soundEffectsClips[soundEffectClipIndex]);
        }
        else
        {
            Debug.LogError("Sound Effect Clip does not exist");
        }
    }

    public void SetMusicVolume(int volPercentage)
    {
        _musicAudioSource.volume = (float)volPercentage / 100;
    }

    public void SetEffectsVolume(int volPercentage)
    {
        _effectsAudioSource.volume = (float)volPercentage / 100;
    }
}
