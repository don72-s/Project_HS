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
    [SerializeField] private LobbySceneManager _manager;

    private void OnEnable()
    {
        _manager.UpdatePlayers();

        PlayerNumbering.OnPlayerNumberingChanged += _manager.UpdatePlayers;

        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
    }

    private void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= _manager.UpdatePlayers;
    }
}
