using Photon.Pun.UtilityScripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class VoiceSliderRegister : MonoBehaviourPunCallbacks
{

    [SerializeField]
    VolumeSlider volumeSliderPrefab;

    VolumeSlider myVolumeSlider;
    Player ownPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponentInParent<PhotonView>().IsMine)
        {
            Debug.Log("본인것이므로 등록하지 않음");
            return;
        }

        myVolumeSlider = Instantiate(volumeSliderPrefab);
        VoiceVolumePanel.Instance.AddVoiceSlider(myVolumeSlider);

        myVolumeSlider.SetPlayerNum(PlayerNumberingExtensions.GetPlayerNumber(
            GetComponentInParent<PhotonView>().Owner));

        ownPlayer = GetComponentInParent<PhotonView>().Owner;

        myVolumeSlider.SetNickname(GetComponentInParent<PhotonView>().Owner.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == ownPlayer) {
            VoiceVolumePanel.Instance.RemoveVoiceSlicer(myVolumeSlider);
        }
    }

}
