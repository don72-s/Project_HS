using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _currentPlayer;
    [SerializeField] private Button _joinRoomButton;

    public void SetRoomInfo(RoomInfo info)
    {
        _roomName.text = info.Name;
        _currentPlayer.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        _joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;
    }

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(_roomName.text);
    }
}
