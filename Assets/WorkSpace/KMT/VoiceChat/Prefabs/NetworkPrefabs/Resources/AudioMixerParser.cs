using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Voice.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerParser : MonoBehaviourPunCallbacks
{

    [SerializeField]
    AudioMixer audioMixer;

    AudioSource audioSource;

    int playerNumber;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

/*    public override void OnJoinedRoom()
    {
        Debug.Log("dd");


        playerNumber = PlayerNumberingExtensions.GetPlayerNumber(
            GetComponentInParent<PhotonView>().Owner);

        string mixerPath = $"Master/Player{playerNumber}";
        string mixerName = $"Player{playerNumber}";

        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(mixerPath);
        AudioMixerGroup mixer = null;

        Debug.Log(mixerPath);


        if (groups == null)
            return;

        foreach (AudioMixerGroup group in groups)
        {
            if (group.name == mixerName)
            {
                mixer = group;
                break;
            }

        }

        if (mixer == null)
        {
            Debug.Log("¹Í¼­°¡ ¾øÀ½");
            return;
        }

        audioSource.outputAudioMixerGroup = mixer;
        SpeakerList.Instance.SetSpeaker(GetComponent<Speaker>(), playerNumber);

    }*/

    // Start is called before the first frame update
    void Start()
    {

        if (photonView.IsMine)
        {
            StartCoroutine(WaitNumberingCO());
            return;
        }

        playerNumber = PlayerNumberingExtensions.GetPlayerNumber(
            GetComponentInParent<PhotonView>().Owner);

        string mixerPath = $"Master/Player{playerNumber}";
        string mixerName = $"Player{playerNumber}";

        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(mixerPath);
        AudioMixerGroup mixer = null;

        Debug.Log(mixerPath);


        if (groups == null)
            return;

        foreach (AudioMixerGroup group in groups)
        {
            if (group.name == mixerName)
            {
                mixer = group;
                break;
            }

        }

        if (mixer == null)
        {
            Debug.Log("¹Í¼­°¡ ¾øÀ½");
            return;
        }

        audioSource.outputAudioMixerGroup = mixer;
        SpeakerList.Instance.SetSpeaker(GetComponent<Speaker>(), playerNumber);

    }

    private void OnDestroy()
    {
        SpeakerList.Instance.RemoveSpeaker(playerNumber);
    }

    IEnumerator WaitNumberingCO() {

        while (PlayerNumberingExtensions.GetPlayerNumber(
            GetComponentInParent<PhotonView>().Owner) == -1) {

            yield return null;
        
        }

        playerNumber = PlayerNumberingExtensions.GetPlayerNumber(
            GetComponentInParent<PhotonView>().Owner);
        Debug.Log(playerNumber);
        SpeakerList.Instance.SetSpeaker(GetComponent<Speaker>(), playerNumber);

    }

}
