using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataContainer
{
    public float musicVolume = 100;
    public float sEffectsVolume = 100;
    public bool bloom = true;
    public bool vignette = true;

    public DataContainer(float mVol, float eVol, bool bloom, bool vignette)
    {
        musicVolume = mVol;
        sEffectsVolume = eVol;
        this.bloom = bloom;
        this.vignette = vignette;
    }

    public DataContainer()
    {
        
    }
}
