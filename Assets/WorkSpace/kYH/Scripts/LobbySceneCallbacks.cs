using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Lobby, Room }      // �� �г��� ���������� �з�

    // �� �г� Ŭ����
    [SerializeField] private LobbySceneManager _manager;
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _lobbyPanel;

    private void Start()
    {
        // �� ���¿� ���� �ڵ����� �г� ��ȯ
        PhotonNetwork.AutomaticallySyncScene = true;

        // �� ���¿� ���� �ڵ����� ��ȯ�� �г� ����
        if (PhotonNetwork.InRoom == true)
        {
            SetActivePanel(Panel.Room);
        }
        else if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Lobby);
        }
        else
        {
            SetActivePanel(Panel.Login);
        }
    }

    // �α��� ���� �� MenuPanel�� ��ȯ
    public override void OnConnectedToMaster()
    {
        Debug.Log("Login Success!");
        SetActivePanel(Panel.Lobby);
    }

    // �α׾ƿ� �� LoginPanel�� ��ȯ
    // �α׷� �α׾ƿ� ���� ǥ��
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Logout! (Cause : {cause})");
        SetActivePanel(Panel.Login);
    }

    // ����� ���� �α� ���
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room complete!");
    }

    // �� ���� ���� ���� ������ ���� �α� ���
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Create Room Failed! (Cause : {message}");
    }

    // �� ������ �������� �� RoomPanel�� ��ȯ
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Enter Success!");
        SetActivePanel(Panel.Room);
    }

    // �濡 ���� �� RoomPanel�� EnterPlayer�� ������ �÷��̾�� ����
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _manager.EnterPlayer(newPlayer);
    }

    // �濡�� ���� �� RoomPanel�� ExitPlayer�� ������ �÷��̾�� ����
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _manager.ExitPlayer(otherPlayer);
    }

    // �濡 ������ �÷��̾��� ������Ƽ�� ����
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //_manager.UpdatePlayerProperty(targetPlayer, changedProps);
    }

    //public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    //{
    //    
    //}

    //public override void OnMasterClientSwitched(Player newMasterClient)
    //{
    //    PhotonNetwork.SetMasterClient(newMasterClient);
    //}

    // �� ���� ���� �� ���� ������ ���� �α� ���
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Room Enter Failed! (Cause : {message}");
    }

    // �濡�� ���� �� MenuPanel�� ��ȯ
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room Success!");
        SetActivePanel(Panel.Lobby);
    }

    // ������Ī ���� �� ���� ������ ���� �α� ���
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Random Match Failed! (Cause : {message}");
    }

    // �κ� ���� �� LobbyPanel�� ��ȯ
    public override void OnJoinedLobby()
    {
        Debug.Log("Join Lobby Success!");
        SetActivePanel(Panel.Lobby);
    }

    // �κ񿡼� ���� �� MenuPanel�� ��ȯ
    public override void OnLeftLobby()
    {
        Debug.Log("Quit Lobby Success!");
        SetActivePanel(Panel.Lobby);
    }

    // �� ��� ������Ʈ �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ���� ��Ͽ� ������ �ִ� ��� �������� ������ ������

        // <���ǻ���>
        // 1. ó�� �κ� ���� �� : ��� �� ����� ����
        // 2. ���� �� �� ����� ����Ǵ� ��� : ����� �� ��ϸ� ����
        _manager.UpdateRoomList(roomList);
    }

    // �� ���¿� �´� �г� ��ȯ ���
    private void SetActivePanel(Panel panel)
    {
        _loginPanel.gameObject.SetActive(panel == Panel.Login);
        _roomPanel.gameObject.SetActive(panel == Panel.Room);
        _lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
}
