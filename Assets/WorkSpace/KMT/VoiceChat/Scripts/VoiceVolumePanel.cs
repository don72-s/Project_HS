using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VoiceVolumePanel : MonoBehaviour
{

    [SerializeField]
    AudioMixer audioMixer;
    [SerializeField]
    GameObject contentParent;

    List<VolumeSlider> volumeSliders = new List<VolumeSlider>();
     
    public static VoiceVolumePanel Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CloseWindow();
    }

    public void OpenWindow() {
        gameObject.SetActive(true);
    }

    public void CloseWindow() {
        gameObject.SetActive(false);
    }

    public void AddVoiceSlider(VolumeSlider volumeSlider) {
        volumeSlider.transform.SetParent(contentParent.transform);
        volumeSlider.SetAudioMixer(audioMixer);
        volumeSliders.Add(volumeSlider);
    }

    public void RemoveVoiceSlicer(VolumeSlider volumeSlider) {
        volumeSliders.Remove(volumeSlider);

        //TODO : 디버그하기
        Destroy(volumeSlider.gameObject);
    }

}
