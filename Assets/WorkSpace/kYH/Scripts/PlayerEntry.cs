using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _readyText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _readyButton;

    public void SetPlayer(Player player)
    {
        if (player.IsMasterClient)
        {
            _nameText.text = $"Master\n{player.NickName}";
        }
        else
        {
            _nameText.text = player.NickName;
        }

        _readyButton.gameObject.SetActive(true);
        _readyButton.interactable = player == PhotonNetwork.LocalPlayer;

        if (player.GetReady())
        {
            _readyText.text = "Ready";
        }
        else
        {
            _readyText.text = "";
        }
    }

    public void SetEmpty()
    {
        _nameText.text = "";
        _readyText.text = "";
        _readyButton.gameObject.SetActive(false);
    }

    public void Ready()
    {
        // 레디가 아니었으면 레디시키기
        // 레디가 맞았으면 레디 풀어주기
        bool ready = PhotonNetwork.LocalPlayer.GetReady();

        if (ready)
        {
            PhotonNetwork.LocalPlayer.SetReady(false);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
        }
    }
}
