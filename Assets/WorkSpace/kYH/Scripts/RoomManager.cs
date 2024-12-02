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
    [SerializeField] private Button _backLobbyButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameManager _gameManager;

    [SerializeField] private GameObject voice;

    [Header("Runner Tutorial")]
    [SerializeField] GameObject[] _runnerTutorials;

    [Header("Seeker Tutorial")]
    [SerializeField] GameObject[] _seekerTutorials;

    private void Start()
    {
        _backLobbyButton.onClick.AddListener(BackToLobby);
        _startButton.onClick.AddListener(StartGame);
    }

    private void Update()
    {
        StartCoroutine(Changeroutine());
    }

    public void BackToLobby()
    {
        if (voice != null)
            Destroy(voice);

        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        _gameManager.TestGameStart();
    }

    IEnumerator Changeroutine()
    {
        yield return new WaitForSeconds(5.0f);

        for (int i = 0; i < _runnerTutorials.Length; i++)
        {
            if (i == _runnerTutorials.Length - 1)
            {
                i = 0;
            }

            _runnerTutorials[i].SetActive(false);
            _runnerTutorials[i + 1].SetActive(true);
        }

        /*for (int i = 0; i < _seekerTutorials.Length; i++)
        {
            if (i == _seekerTutorials.Length - 1)
            {
                i = 0;
            }

            _seekerTutorials[i].SetActive(false);
            _seekerTutorials[i + 1].SetActive(true);
        }*/
    }
}
