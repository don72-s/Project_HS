using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class RoomCallbacks : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Lobby, Room }

    [SerializeField] private RoomUpdate _roomUpdate;

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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        _roomUpdate.UpdatePlayerProperty(targetPlayer, changedProps);
    }
}
