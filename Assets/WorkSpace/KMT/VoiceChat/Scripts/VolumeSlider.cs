using Photon.Pun.UtilityScripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    Scrollbar scrollbar;
    [SerializeField]
    TextMeshProUGUI text;

    AudioMixer audioMixer;
    int playerNum = -1;

    private void Awake()
    {
        scrollbar.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void SetPlayerNum(int playerNumber) { 
        playerNum = playerNumber;
    }

    public void SetNickname(in string playerNickName) {
        text.text = playerNickName;
    }

    public void SetAudioMixer(AudioMixer mixer) {
        audioMixer = mixer;
    }

    void OnSliderValueChanged(float changedValue) {

        audioMixer.SetFloat(
            StageData.Instance.GetPlayerMixerName(playerNum), 
            Mathf.Lerp(-20, 20, changedValue));

        if (changedValue <= 0.01f) {
            audioMixer.SetFloat(
            StageData.Instance.GetPlayerMixerName(playerNum),
            -80);
        }

    }




}
