using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class RoomCallbacks : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Lobby, Room }

    [SerializeField] private RoomUpdate _roomUpdate;

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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        _roomUpdate.UpdatePlayerProperty(targetPlayer, changedProps);
    }
}
