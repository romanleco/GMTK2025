using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Rendering.PostProcessing;

public class SaveManager : MonoSingleton<SaveManager>
{
    public void Save(float musicVol, float effectsVol, bool bloom, bool vignette)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.datasave";
        FileStream stream = new FileStream(path, FileMode.Create);

        DataContainer data = new DataContainer();

        data.musicVolume = musicVol;
        data.sEffectsVolume = effectsVol;
        data.bloom = bloom;
        data.vignette = vignette;

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public void Save(float volume, string affected)
    {
        DataContainer loadedData = Load();
        
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.datasave";
        FileStream stream = new FileStream(path, FileMode.Create);

        DataContainer data = new DataContainer();

        if(loadedData != null)
        {
            data.sEffectsVolume = loadedData.sEffectsVolume;
            data.musicVolume = loadedData.musicVolume;
            data.bloom = loadedData.bloom;
            data.vignette = loadedData.vignette;
        }

        if(affected == "MUSIC")
        {
            data.musicVolume = volume;
            
        }
        else if(affected == "EFFECTS")
        {
            data.sEffectsVolume = volume;
        }

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public void Save(bool enabled, string affected)
    {
        DataContainer loadedData = Load();
        
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.datasave";
        FileStream stream = new FileStream(path, FileMode.Create);

        DataContainer data = new DataContainer();

        if(loadedData != null)
        {
            data.musicVolume = loadedData.musicVolume;
            data.sEffectsVolume = loadedData.sEffectsVolume;
            data.bloom = loadedData.bloom;
            data.vignette = loadedData.vignette;
        }

        switch(affected)
        {
            case "BLOOM":
                data.bloom = enabled;
            break;

            case "VIGNETTE":
                data.vignette = enabled;
            break;

            default:
                Debug.LogError("Unknown Affected Value");
            break;
        }


        formatter.Serialize(stream, data);
        stream.Close();
    }

    public DataContainer Load()
    {
        string path = Application.persistentDataPath + "/save.datasave";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            if(stream.Length == 0)
            {
                stream.Close();
                return null;
            }

            DataContainer data = (DataContainer)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}
