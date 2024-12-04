using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerList : MonoBehaviour
{

    public static SpeakerList Instance = null;

    Speaker[] speakers = new Speaker[10];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void SetSpeaker(Speaker speaker, int idx) {

        if (idx < 0 || idx >= speakers.Length) {
            Debug.Log("범위를 벗어난 스피커 인덱스");
            return;
        }

        speakers[idx] = speaker;

    }

    public Speaker GetSpeaker(int idx) {

        if (idx < 0 || idx >= speakers.Length)
        {
            Debug.Log("범위를 벗어난 스피커 인덱스");
            return null;
        }

        return speakers[idx];
    }

    public void RemoveSpeaker(int idx) {

        if (idx < 0 || idx >= speakers.Length)
        {
            Debug.Log("범위를 벗어난 스피커 인덱스");
            return;
        }

        speakers[idx] = null;
    }
}
