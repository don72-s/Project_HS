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

    // 상태
    public enum GameState { Waiting, Playing, Finished }

    public GameState currentState = GameState.Waiting;

    // 시간 설정
    public float gameDuration = 10f; 

    private float timer;

    // 승리 조건
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
        Debug.Log("시작");
    }

    IEnumerator StartDelayRoutine()
    {
        yield return new WaitForSeconds(1f);
        TestGameStart();
    }

    public void TestGameStart()
    {
        PlayerSpawn(); // 플레이어 스폰

        if (PhotonNetwork.IsMasterClient)
        {
           // SetTeams(); // 팀 배정
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

        // 술래 한 명 랜덤 선택
        int randomIndex = Random.Range(0, allPlayers.Count);
        Player seeker = allPlayers[randomIndex];

        photonView.RPC("RPC_SetSeeker", RpcTarget.All, seeker.ActorNumber);

        runnersRemaining = allPlayers.Count - 1; // 나머지는 러너
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
        //PhotonNetwork.LeaveRoom(); // 방으로 복귀
    }

    public override void OnLeftRoom()
    {
        Debug.Log("LeftRoom");
        //PhotonNetwork.LoadLevel("LobbyScene"); // 로비 씬으로 복귀
    }

    public void OnPlayerCatch()
    {
        runnersRemaining--;
        Debug.LogWarning("남은사람 : " + runnersRemaining);
        // 혼자 테스트용 주석
        //if (runnersRemaining <= 0)
        //{
        //    EndGame("Seekers Win");
        //}
    }
}
