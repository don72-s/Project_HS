using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : BaseUI
{
    [SerializeField] private TMP_Text _readyText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _readyButton;
    private bool _isCheck;

    private void Update()
    {
        if (_isCheck && Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("레디!!!!!!!!!!!!!!!!!!!!!!");
            bool ready = PhotonNetwork.LocalPlayer.GetReady();

            if (ready)
            {
                Debug.Log("레디풀어라");
                PhotonNetwork.LocalPlayer.SetReady(false);
            }
            else
            {
                Debug.Log("레디해라");
                PhotonNetwork.LocalPlayer.SetReady(true);
            }
        }
    }

    public void SetPlayer(Player player)
    {
        if (player.IsMasterClient)
        {
            _nameText.text = player.NickName;
            _nameText.color = Color.yellow;
        }
        else
        {
            _nameText.text = player.NickName;
            _nameText.color = Color.black;
        }

        if (PhotonNetwork.LocalPlayer.NickName == _nameText.text)
        {
            _isCheck = true;
        }
        _readyButton.gameObject.SetActive(true);
        _readyButton.interactable = player == PhotonNetwork.LocalPlayer;

        if (player.GetReady())
        {
            Debug.Log("썼냐?");
            _readyText.text = "Ready";
        }
        else
        {
            Debug.Log("왜 못쓰냐고!!!!!!!!!!!!!!!!!!!!!!");
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
        Debug.Log("레디!!!!!!!!!!!!!!!!!!!!!!");
        bool ready = PhotonNetwork.LocalPlayer.GetReady();

        if (ready)
        {
            Debug.Log("레디풀어라");
            PhotonNetwork.LocalPlayer.SetReady(false);
        }
        else
        {
            Debug.Log("레디해라");
            PhotonNetwork.LocalPlayer.SetReady(true);
        }
    }
}
