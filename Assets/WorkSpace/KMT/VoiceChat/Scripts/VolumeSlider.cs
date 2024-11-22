using Photon.Pun.UtilityScripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    Scrollbar scrollbar;

    AudioMixer audioMixer;
    int playerNum = -1;

    private void Awake()
    {
        scrollbar.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void SetPlayerNum(int playerNumber) { 
        playerNum = playerNumber;
    }

    public void SetAudioMixer(AudioMixer mixer) {
        audioMixer = mixer;
    }

    void OnSliderValueChanged(float changedValue) {

        audioMixer.SetFloat(
            StageData.Instance.GetPlayerMixerName(playerNum), 
            Mathf.Lerp(-80, 20, changedValue));

    }




}
