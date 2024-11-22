using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Recorder))]
public class VoiceManager : MonoBehaviour
{

    Recorder recorder;

    private void Awake()
    {
        recorder = GetComponent<Recorder>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {

            recorder.TransmitEnabled = !recorder.TransmitEnabled;

        }

    }

}
