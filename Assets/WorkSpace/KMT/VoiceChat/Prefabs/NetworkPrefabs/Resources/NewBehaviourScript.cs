using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField]
    AudioMixer audioMixer;

    AudioSource audioSource;

    int playerNumber;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerNumber = PlayerNumberingExtensions.GetPlayerNumber(
            GetComponentInParent<PhotonView>().Owner);

        string mixerPath = $"Master/Player{playerNumber}";
        string mixerName = $"Player{playerNumber}";

        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(mixerPath);
        AudioMixerGroup mixer = null;

        if (groups == null)
            return;

        foreach (AudioMixerGroup group in groups)
        {
            if (group.name == mixerName) { 
                mixer = group;
                break;
            }

        }

        if (mixer == null) {
            Debug.Log("¹Í¼­°¡ ¾øÀ½");
            return;
        }

        audioSource.outputAudioMixerGroup = mixer;
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
