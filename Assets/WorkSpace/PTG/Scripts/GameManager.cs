using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoomPTK";

    public static GameManager Instance;

    // ����
    public enum GameState { Waiting, Playing, Finished }

    public GameState currentState = GameState.Waiting;

    // �ð� ����
    public float gameDuration = 10f; 

    private float timer;

    // �¸� ����
    private int runnersRemaining;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (currentState == GameState.Playing)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                EndGame("Runner Win");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestGameStart();
        }
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = false;

        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
       // StartCoroutine(StartDelayRoutine());
        Debug.Log("����");
    }

    IEnumerator StartDelayRoutine()
    {
        yield return new WaitForSeconds(1f);
        TestGameStart();
    }

    public void TestGameStart()
    {
        PlayerSpawn(); // �÷��̾� ����

        if (PhotonNetwork.IsMasterClient)
        {
           // SetTeams(); // �� ����
            photonView.RPC("RPC_StartGame", RpcTarget.All);
        }
    }

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5, 5f));
        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
        {
            PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);
        }
        else
        {
            GameObject runner = PhotonNetwork.Instantiate("Runner", randomPos, Quaternion.identity);
            runner.GetComponent<RunnerController>().OnDeadEvent.AddListener(OnPlayerCatch);
        }
    }

    private void SetTeams()
    {
        List<Player> allPlayers = new List<Player>(PhotonNetwork.PlayerList);

        // ���� �� �� ���� ����
        int randomIndex = Random.Range(0, allPlayers.Count);
        Player seeker = allPlayers[randomIndex];

        photonView.RPC("RPC_SetSeeker", RpcTarget.All, seeker.ActorNumber);

        runnersRemaining = allPlayers.Count - 1; // �������� ����
    }

    [PunRPC]
    private void RPC_SetSeeker(int seekerActorNumber)
    {
        Player seeker = PhotonNetwork.CurrentRoom.GetPlayer(seekerActorNumber);

        if (PhotonNetwork.LocalPlayer == seeker)
        {
            Debug.Log("You are Seeker");
        }
        else
        {
            Debug.Log("You are Runner");
        }
    }

    [PunRPC]
    private void RPC_StartGame()
    {
        currentState = GameState.Playing;
        timer = gameDuration;
        Debug.Log("Game Start");
    }

    private void EndGame(string message)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_EndGame", RpcTarget.All, message);
        }
    }

    [PunRPC]
    private void RPC_EndGame(string message)
    {
        currentState = GameState.Finished;
        Debug.Log("Game Over: " + message);

        StartCoroutine(ReturnToLobby());
    }

    private IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("LeaveRoom");
        //PhotonNetwork.LeaveRoom(); // ������ ����
    }

    public override void OnLeftRoom()
    {
        Debug.Log("LeftRoom");
        //PhotonNetwork.LoadLevel("LobbyScene"); // �κ� ������ ����
    }

    public void OnPlayerCatch()
    {
        runnersRemaining--;
        Debug.LogWarning("������� : " + runnersRemaining);
        // ȥ�� �׽�Ʈ�� �ּ�
        //if (runnersRemaining <= 0)
        //{
        //    EndGame("Seekers Win");
        //}
    }
}
