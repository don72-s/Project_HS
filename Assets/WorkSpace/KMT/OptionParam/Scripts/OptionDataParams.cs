using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionDataParams : MonoBehaviour
{

    float bgmVolume = 0.5f;
    float sfxVolume = 0.5f;
    float mouseSensitivity = 1f;

    public event Action<float> bgmValueChanged;
    public event Action<float> sfxValueChanged;

    public float BGM_Volume 
    {
        get 
        { 
            return bgmVolume; 
        }
        set
        {
            bgmVolume = value;
            bgmValueChanged?.Invoke(bgmVolume);
        }
    }
    public float SFX_Volume
    {
        get
        {
            return sfxVolume;
        }
        set
        {
            sfxVolume = value;
            sfxValueChanged?.Invoke(sfxVolume);
        }
    }
    public float MouseSensitivity { get; set; } = 1;

}
