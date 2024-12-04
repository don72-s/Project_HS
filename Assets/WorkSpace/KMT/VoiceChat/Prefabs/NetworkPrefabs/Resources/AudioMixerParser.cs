using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerParser : MonoBehaviourPunCallbacks
{

    [SerializeField]
    AudioMixer audioMixer;

    AudioSource audioSource;

    Player ownPlayer;
    int playerNumber;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


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
        ownPlayer = GetComponentInParent<PhotonView>().Owner;

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
            Debug.Log("믹서가 없음");
            return;
        }

        audioSource.outputAudioMixerGroup = mixer;
        SpeakerList.Instance.SetSpeaker(GetComponent<Speaker>(), playerNumber);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == ownPlayer)
        {
            Debug.Log("가써..." + otherPlayer.NickName);
            SpeakerList.Instance.RemoveSpeaker(playerNumber);
            Destroy(gameObject);
        }
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
