using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class LobbySceneCallbacks : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Lobby, Room }      // 각 패널을 열거형으로 분류

    // 각 패널 클래스
    [SerializeField] private RoomUpdate _roomUpdate;
    [SerializeField] private LobbySceneManager _manager;
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _dataManager;

    private void Start()
    {
        // 각 상태에 따라 자동으로 패널 전환
        PhotonNetwork.AutomaticallySyncScene = true;

        // 각 상태에 따라 자동으로 전환할 패널 설정
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

    // 로그인 성공 시 MenuPanel로 전환
    public override void OnConnectedToMaster()
    {
        Debug.Log("Login Success!");
        _dataManager.SetActive(true);
        SetActivePanel(Panel.Lobby);
        PhotonNetwork.JoinLobby();
    }

    // 로그아웃 시 LoginPanel로 전환
    // 로그로 로그아웃 사유 표시
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Logout! (Cause : {cause})");
        SetActivePanel(Panel.Login);
        PhotonNetwork.LeaveLobby();
    }

    // 방생성 성공 로그 출력
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room complete!");
    }

    // 방 생성 실패 실패 사유가 적힌 로그 출력
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Create Room Failed! (Cause : {message}");
    }

    // 방 참여에 성공했을 때 RoomPanel로 전환
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Enter Success!");
        //SetActivePanel(Panel.Room);
        //PhotonNetwork.LoadLevel("Test_RoomScene");
        PhotonNetwork.LoadLevel("GameScene_Test_KYH");
    }

    // 방에 입장 시 RoomPanel의 EnterPlayer을 입장한 플레이어에게 실행
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _roomUpdate.EnterPlayer(newPlayer);
    }

    // 방에서 퇴장 시 RoomPanel의 ExitPlayer을 퇴장한 플레이어에게 실행
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _roomUpdate.ExitPlayer(otherPlayer);
    }

    // 방에 입장한 플레이어의 프로퍼티를 변경
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

    // 방 입장 실패 시 실패 사유가 적힌 로그 출력
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Room Enter Failed! (Cause : {message}");
    }

    // 방에서 퇴장 시 MenuPanel로 전환
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room Success!");
        SetActivePanel(Panel.Lobby);
    }

    // 랜덤매칭 실패 시 실패 사유가 적힌 로그 출력
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

    // 방 목록 업데이트 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방의 목록에 변경이 있는 경우 서버에서 보내는 정보들

        // <주의사항>
        // 1. 처음 로비 입장 시 : 모든 방 목록을 전달
        // 2. 입장 중 방 목록이 변경되는 경우 : 변경된 방 목록만 전달
        _manager.UpdateRoomList(roomList);
    }

    // 각 상태에 맞는 패널 전환 기능
    private void SetActivePanel(Panel panel)
    {
        _loginPanel.SetActive(panel == Panel.Login);
        _roomPanel.SetActive(panel == Panel.Room);
        _lobbyPanel.SetActive(panel == Panel.Lobby);
    }
}
