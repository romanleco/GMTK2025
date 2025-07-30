using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoSingleton<PostProcessingManager>
{
    [SerializeField] private Volume _globalVolume;

    void Start()
    {
        DataContainer loadedData = SaveManager.Instance.Load();
        if(loadedData != null)
        {
            SetVignetteActive(loadedData.vignette);
            SetBloomActive(loadedData.bloom);
        }
    }

    public void SetVignetteActive(bool value)
    {
        if(_globalVolume.profile.TryGet(out Vignette vignette))
        {
            vignette.active = value;
        }
    }

    public void SetBloomActive(bool value)
    {
        if(_globalVolume.profile.TryGet(out Bloom bloom))
        {
            bloom.active = value;
        }
    }
}
