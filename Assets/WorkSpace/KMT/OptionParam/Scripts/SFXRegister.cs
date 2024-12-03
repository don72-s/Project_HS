using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXRegister : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.volume = DataManager.Instance.OptionDataParams.SFX_Volume;
        DataManager.Instance.OptionDataParams.sfxValueChanged += VolumeChanged;
    }

    void VolumeChanged(float val)
    {
        audioSource.volume = val;
    }

    private void OnDestroy()
    {
        DataManager.Instance.OptionDataParams.sfxValueChanged -= VolumeChanged;
    }

}
