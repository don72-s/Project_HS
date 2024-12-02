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
    /*[SerializeField] private Button _backLobbyButton;
    [SerializeField] private Button _startButton;*/
    [SerializeField] private GameManager _gameManager;

    [SerializeField] private GameObject voice;

    [Header("Tutorial Page")]
    [SerializeField] GameObject[] _runnerTutorials;
    [SerializeField] GameObject[] _seekerTutorials;

    /*private void Start()
    {
        _backLobbyButton.onClick.AddListener(BackToLobby);
        _startButton.onClick.AddListener(StartGame);
    }*/

    private void OnEnable()
    {
        StartCoroutine(RunnerTutorialCO());
        StartCoroutine(SeekerTutorialCO());
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

    IEnumerator RunnerTutorialCO()
    {
        int i = 0;

        while (true)
        {

            foreach (GameObject obj in _runnerTutorials)
            { 
                obj.SetActive(false);
            }

            if (i >= _runnerTutorials.Length)
            {
                i = 0;
            }

            _runnerTutorials[i].SetActive(true);

            yield return new WaitForSeconds(5.0f);

            i++;

        }

    }

    IEnumerator SeekerTutorialCO()
    {
        int i = 0;

        while (true)
        {

            foreach (GameObject obj in _seekerTutorials)
            {
                obj.SetActive(false);
            }

            if (i >= _seekerTutorials.Length)
            {
                i = 0;
            }

            _seekerTutorials[i].SetActive(true);

            yield return new WaitForSeconds(5.0f);

            i++;

        }
    }


}
