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

        Debug.Log("<color=lightblue>ģ��üũ��� ���� �ݹ�</color>");

        if (PhotonNetwork.AuthValues.UserId == FriendCheck.CheckName)
        {
            Debug.LogWarning("üũ����");
            loginAllowed = false;
            //userid�� ����ϰ��ִ� ����� �˻��ϱ� ���� ������ uid�� �ӽ÷� ����
            PhotonNetwork.FindFriends(new string[] { PhotonNetwork.LocalPlayer.NickName });
        }
        else
        {
            Debug.Log("<color=green>��������, ģ��üũ ����.</color>");
        }

    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        //�ߺ�Ȯ�ο� ����, �˻� ����, �������� �ƴ� ����� ���� ���.
        if (PhotonNetwork.AuthValues.UserId == CheckName && friendList.Count > 0 && !friendList[0].IsOnline)
        {
            Debug.Log("<color=green>���� ����.</color>");
            loginAllowed = true;
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.LogWarning("<color=green>�ߺ�������, ���� �Ұ�.</color>");
            loginAllowed = false;
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        Debug.Log("<color=lightblue>�׽�Ʈ �õ� ���� ���� �ݹ�.</color>");

        if (PhotonNetwork.AuthValues.UserId == CheckName)
        {

            //���������� ������
            if (loginAllowed)
            {
                Debug.Log("<color=green>�ߺ� ���� �׽�Ʈ �Ϸ�, �������� ����</color>");
                PhotonNetwork.AuthValues = new AuthenticationValues();
                PhotonNetwork.AuthValues.UserId = PhotonNetwork.LocalPlayer.NickName;
                PhotonNetwork.LocalPlayer.NickName = BackendManager.Auth.CurrentUser.DisplayName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else//���� ���� ���� 
            {
                Debug.Log("<color=red>�ߺ�����, ���� ���� ����</color>");
                BackendManager.Auth.SignOut();
            }

        }

    }


}


