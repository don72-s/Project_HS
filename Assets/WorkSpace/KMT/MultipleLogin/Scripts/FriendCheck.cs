using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FriendCheck : MonoBehaviourPunCallbacks
{

    public const string CheckName = "Check";

    bool loginAllowed = false;

    public override void OnConnectedToMaster()
    {

        Debug.Log("<color=lightblue>친구체크방면 연결 콜백</color>");

        if (PhotonNetwork.AuthValues.UserId == FriendCheck.CheckName)
        {
            Debug.LogWarning("체크시작");
            loginAllowed = false;
            //userid를 사용하고있는 사람을 검색하기 위해 본인의 uid를 임시로 지정
            PhotonNetwork.FindFriends(new string[] { PhotonNetwork.LocalPlayer.NickName });
        }
        else
        {
            Debug.Log("<color=green>정상접속, 친구체크 안함.</color>");
        }

    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        //중복확인용 접속, 검색 가능, 접속중이 아닌 경우라면 접속 허용.
        if (PhotonNetwork.AuthValues.UserId == CheckName && friendList.Count > 0 && !friendList[0].IsOnline)
        {
            Debug.Log("<color=green>접속 가능.</color>");
            loginAllowed = true;
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.LogWarning("<color=green>중복접속중, 접속 불가.</color>");
            loginAllowed = false;
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        Debug.Log("<color=lightblue>테스트 시도 접속 해제 콜백.</color>");

        if (PhotonNetwork.AuthValues.UserId == CheckName)
        {

            //정상적으로 재접속
            if (loginAllowed)
            {
                Debug.Log("<color=green>중복 접속 테스트 완료, 정상접속 시작</color>");
                PhotonNetwork.AuthValues = new AuthenticationValues();
                PhotonNetwork.AuthValues.UserId = PhotonNetwork.LocalPlayer.NickName;
                PhotonNetwork.LocalPlayer.NickName = BackendManager.Auth.CurrentUser.DisplayName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else//강제 접속 해제 
            {
                Debug.Log("<color=red>중복접속, 강제 접속 해제</color>");
                BackendManager.Auth.SignOut();
            }

        }

    }


}


