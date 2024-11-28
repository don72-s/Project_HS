using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Recorder))]
public class VoiceManager : MonoBehaviourPunCallbacks
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey(CustomProperties.ALIVE))
        {
            bool isAlive = (bool)changedProps[CustomProperties.ALIVE];

            if (!isAlive)
            {
                recorder.TransmitEnabled = false;
                micImg.sprite = micMuteSprite;
            }
            else {
                recorder.TransmitEnabled = true;
                micImg.sprite = micSprite;
            }
        }
    }

    private void Update()
    {

        if (!PhotonNetwork.LocalPlayer.GetAlive()) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.T) && PhotonNetwork.LocalPlayer.GetAlive())
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
