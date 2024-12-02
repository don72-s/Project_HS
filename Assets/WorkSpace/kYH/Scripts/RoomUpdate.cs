using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
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
        //PhotonNetwork.LocalPlayer.SetReady(false);
        StartCoroutine(WaitForJoinCO());
        //PhotonNetwork.LocalPlayer.SetLoad(false);
        UpdatePlayers();
    }

    private void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
    }

    IEnumerator WaitForJoinCO() 
    {
        while (PhotonNetwork.NetworkClientState != ClientState.Joined)
        {
            yield return null;
        }

        PhotonNetwork.LocalPlayer.SetReady(false);

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

        if (PhotonNetwork.LocalPlayer.IsMasterClient && CheckAllReady())
        {
            StartCoroutine(StartCountdownCO());
        }
    }

    IEnumerator StartCountdownCO()
    {

        float cnt = 5;
        int displayeVal = 5;

        if (GameManager.Instance.currentState == GameManager.GameState.InLoading ||
           GameManager.Instance.currentState == GameManager.GameState.Playing)
            yield break;

        while (cnt > 0)
        {
            if (!CheckAllReady())
                yield break;

            yield return null;
            cnt -= Time.deltaTime;
            if (displayeVal > cnt)
            {
                GameManager.Instance.photonView.RPC("PlayCountdownRpc", RpcTarget.All, displayeVal);
                displayeVal--;
            }
        }

        GameManager.Instance.TestGameStart();

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

    public void UpdatePlayerProperty(Player targetPlayer, ExitGames.Client.Photon.Hashtable properties)
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
