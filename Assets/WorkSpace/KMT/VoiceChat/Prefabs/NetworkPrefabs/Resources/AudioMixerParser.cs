using Photon.Pun;
using Photon.Pun.UtilityScripts;
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

    public override void OnJoinedRoom()
    {
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
    }

    // Start is called before the first frame update
    void Start()
    {

        if (photonView.IsMine)
            return;

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

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(PlayerNumberingExtensions.GetPlayerNumber(
            GetComponentInParent<PhotonView>().Owner));
        }
    }

}
