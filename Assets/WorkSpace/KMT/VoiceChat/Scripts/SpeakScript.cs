using Photon.Pun;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakScript : MonoBehaviour
{
    PhotonVoiceView photonVoiceView;

    [SerializeField]
    GameObject SpeakerMark;

    private void Awake()
    {
        photonVoiceView = GetComponentInParent<PhotonVoiceView>();
    }

    private void Update()
    {
        
        if (SpeakerMark.activeSelf != photonVoiceView.IsRecording) {
            SpeakerMark.SetActive(photonVoiceView.IsRecording);
        }

    }

}
