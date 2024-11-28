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
            Debug.Log("����!!!!!!!!!!!!!!!!!!!!!!");
            bool ready = PhotonNetwork.LocalPlayer.GetReady();

            if (ready)
            {
                Debug.Log("����Ǯ���");
                PhotonNetwork.LocalPlayer.SetReady(false);
            }
            else
            {
                Debug.Log("�����ض�");
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
            Debug.Log("���?");
            _readyText.text = "Ready";
        }
        else
        {
            Debug.Log("�� �����İ�!!!!!!!!!!!!!!!!!!!!!!");
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
        // ���� �ƴϾ����� �����Ű��
        // ���� �¾����� ���� Ǯ���ֱ�
        Debug.Log("����!!!!!!!!!!!!!!!!!!!!!!");
        bool ready = PhotonNetwork.LocalPlayer.GetReady();

        if (ready)
        {
            Debug.Log("����Ǯ���");
            PhotonNetwork.LocalPlayer.SetReady(false);
        }
        else
        {
            Debug.Log("�����ض�");
            PhotonNetwork.LocalPlayer.SetReady(true);
        }
    }
}
