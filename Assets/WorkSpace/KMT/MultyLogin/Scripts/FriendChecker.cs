using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendChecker : MonoBehaviourPunCallbacks
{

    public const string CheckName = "Check";

    public override void OnConnectedToMaster()
    {
        //
        Debug.Log("ddda?");

        if (PhotonNetwork.AuthValues.UserId == FriendChecker.CheckName)
        {
            Debug.LogWarning("체크시작");
            PhotonNetwork.FindFriends(new string[] { PhotonNetwork.LocalPlayer.NickName });
        }
        else { 
        //정상접속.
        }

    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        if (PhotonNetwork.AuthValues.UserId == CheckName)
        {
            Debug.LogWarning("다시 체크 시작");
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = PhotonNetwork.LocalPlayer.NickName;
            PhotonNetwork.LocalPlayer.NickName = BackendManager.Auth.CurrentUser.DisplayName;
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        if (PhotonNetwork.AuthValues.UserId == CheckName && friendList.Count > 0 && !friendList[0].IsOnline)
        {
            Debug.LogWarning("접속하자");
            PhotonNetwork.Disconnect();
        }
        else {
            Debug.LogWarning("ㅋㅋ중복");

        }
    }


}
