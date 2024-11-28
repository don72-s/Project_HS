using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using WebSocketSharp;
using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class RoomManager : BaseUI
{
    [SerializeField] private GameObject _roomManager;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameManager _gameManager;

    [SerializeField]
    GameObject voice;

    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        AddEvent("BackToLobbyButton", EventType.Click, BackToLobby);
        _startButton.onClick.AddListener(StartGame);
    }

    public void BackToLobby(PointerEventData eventData)
    {
        if (voice != null)
            Destroy(voice);

        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        _gameManager.TestGameStart();
    }

}
