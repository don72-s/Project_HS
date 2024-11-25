using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Recorder))]
public class VoiceManager : MonoBehaviour
{

    Recorder recorder;

    [Header("Microphone")]
    [SerializeField]
    Image micImg;
    [SerializeField]
    Sprite micSprite;
    [SerializeField]
    Sprite micMuteSprite;

    private void Awake()
    {
        recorder = GetComponent<Recorder>();
        recorder.TransmitEnabled = true;
        micImg.sprite = micSprite;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {

            if (recorder.TransmitEnabled)
            {
                recorder.TransmitEnabled = false;
                micImg.sprite = micMuteSprite;
            }
            else 
            {
                recorder.TransmitEnabled = true;
                micImg.sprite = micSprite;
            }


        }

    }

}
