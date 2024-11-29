using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class RoomUpdate : MonoBehaviour
{
    [SerializeField] private PlayerEntry[] _playerEntries;
    [SerializeField] private Button _startButton;

    private void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
        UpdatePlayers();
    }

    private void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
    }

    public void UpdatePlayers()
    {
        foreach (PlayerEntry entry in _playerEntries)
        {
            entry.SetEmpty();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetPlayerNumber() == -1)
                continue;
            int num = player.GetPlayerNumber();
            _playerEntries[num].SetPlayer(player);
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _startButton.interactable = CheckAllReady();
        }
        else
        {
            _startButton.interactable = false;
        }
    }

    public void EnterPlayer(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} Enter!");
        UpdatePlayers();
    }

    public void ExitPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} Exit!");
        UpdatePlayers();
    }

    public void UpdatePlayerProperty(Player targetPlayer, Hashtable properties)
    {
        Debug.Log($"{targetPlayer.NickName} Update!");

        // 레디 커스텀 프로퍼티를 변경한 경우면 READY 키가 있음
        if (properties.ContainsKey(CustomProperties.READY))
        {
            UpdatePlayers();
        }
    }

    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady() == false)
                return false;
        }

        return true;
    }

    public void StartGame()
    {
        //PhotonNetwork.LoadLevel("GameScene");
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    //public void LeaveRoom()
    //{
    //    PhotonNetwork.LeaveRoom();
    //}
}
