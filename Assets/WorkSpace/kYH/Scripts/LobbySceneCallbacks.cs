using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class LobbySceneCallbacks : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Lobby, Room }      // �� �г��� ���������� �з�

    // �� �г� Ŭ����
    [SerializeField] private RoomUpdate _roomUpdate;
    [SerializeField] private LobbySceneManager _manager;
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _dataManager;

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
        _dataManager.SetActive(true);
        SetActivePanel(Panel.Lobby);
        PhotonNetwork.JoinLobby();
    }

    // �α׾ƿ� �� LoginPanel�� ��ȯ
    // �α׷� �α׾ƿ� ���� ǥ��
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Logout! (Cause : {cause})");
        SetActivePanel(Panel.Login);
        PhotonNetwork.LeaveLobby();
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
        //SetActivePanel(Panel.Room);
        //PhotonNetwork.LoadLevel("Test_RoomScene");
        PhotonNetwork.LoadLevel("GameScene_Test_KYH");
    }

    // �濡 ���� �� RoomPanel�� EnterPlayer�� ������ �÷��̾�� ����
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _roomUpdate.EnterPlayer(newPlayer);
    }

    // �濡�� ���� �� RoomPanel�� ExitPlayer�� ������ �÷��̾�� ����
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _roomUpdate.ExitPlayer(otherPlayer);
    }

    // �濡 ������ �÷��̾��� ������Ƽ�� ����
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        _roomUpdate.UpdatePlayerProperty(targetPlayer, changedProps);
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

    public override void OnJoinedLobby()
    {
        Debug.Log("Join Lobby Success!");
        SetActivePanel(Panel.Lobby);
    }

    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Login);
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
        _loginPanel.SetActive(panel == Panel.Login);
        _roomPanel.SetActive(panel == Panel.Room);
        _lobbyPanel.SetActive(panel == Panel.Lobby);
    }
}
