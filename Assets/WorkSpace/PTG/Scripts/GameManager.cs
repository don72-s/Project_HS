using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoomPTKq";

    public static GameManager Instance;

    // 상태
    public enum GameState { Waiting, Playing, Finished }

    public GameState currentState = GameState.Waiting;

    // 시간 설정
    public float gameDuration = 10f; 

    private float timer;

    // 승리 조건
    private int runnersRemaining;

    public Text resultText;
    public Slider timeSlider;

    GameObject myRoomPlayer;
    GameObject myIngamePlayer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //단위 테스트용 코드
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();

        resultText.gameObject.SetActive(false); 
        timeSlider.maxValue = gameDuration;     
        timeSlider.value = gameDuration;

        //StartCoroutine(WaitCO(2));
    }

    IEnumerator WaitCO(float waitTime) { 
    
        yield return new WaitForSeconds(waitTime);
        myRoomPlayer = PhotonNetwork.Instantiate("WaitPlayer", Vector3.zero, Quaternion.identity);

    }

    private void Update()
    {

        if (currentState == GameState.Playing)
        {
            timer -= Time.deltaTime;

            timeSlider.value = timer;

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
        myRoomPlayer = PhotonNetwork.Instantiate("WaitPlayer", Vector3.zero, Quaternion.identity);
        Debug.Log("시작");

    }


    IEnumerator WaitPlayerSpawnCO() {

        yield return new WaitForSeconds(3f);
        StageData.Instance.StartChangeFormSlot();

    }

    //TODO : 게임 중복시작 무시 예외처리 필요.
    public void TestGameStart()
    {
        List<Player> allPlayers = new List<Player>(PhotonNetwork.PlayerList);

        if (PhotonNetwork.IsMasterClient)
        {
            runnersRemaining = allPlayers.Count - 1; // 나머지는 러너
            LoadStage();
        }
    }

    private void LoadStage()
    {
        Coroutine waitLoadStageCoroudine = StartCoroutine(WaitLoadStageCO());
        photonView.RPC("LoadSceneAdditive", RpcTarget.All);
    }

    int inLoadingPlayer;
    IEnumerator WaitLoadStageCO() {

        float waitTime = 0;
        inLoadingPlayer = PhotonNetwork.PlayerList.Length;
        while (waitTime < 30f && inLoadingPlayer > 0) {

            yield return null;
            waitTime += Time.deltaTime;

        }

        if (inLoadingPlayer == 0)
        {
            Debug.Log("로딩 완료!!");

            SetTeams(); // 팀 배정 및 플레이어 스폰

            //타이머 활성화
            photonView.RPC("RPC_StartGame", RpcTarget.All);

            StartCoroutine(WaitPlayerSpawnCO());

        }
        else 
        {
            Debug.Log("로딩 시간 초과...");
        }

    }

    [PunRPC]
    void LoadSceneAdditive() {

        AsyncOperation op = SceneManager.LoadSceneAsync("lsy_GameScene_Additive", LoadSceneMode.Additive);
        op.completed += (_op) => { Debug.Log("완료!");photonView.RPC("LoadSceneFinished", RpcTarget.MasterClient);};

    }

    void UnLoadScene()
    {
        SceneManager.UnloadSceneAsync("lsy_GameScene_Additive");
    }

    [PunRPC]
    void LoadSceneFinished() {


        if (inLoadingPlayer <= 0) {
            Debug.Log("뭔가 잘못됨.");
            return;
        }

        inLoadingPlayer--;

    }

    [PunRPC]
    private void PlayerSpawn(int ranNumber)
    {
        myRoomPlayer.GetComponent<PlayerControllerParent>().SetActiveTo(false);

        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5, 5f));
        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == ranNumber)
        {
            myIngamePlayer = PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);
        }
        else
        {
            GameObject runner = PhotonNetwork.Instantiate("Runner", randomPos, Quaternion.identity);
            runner.GetComponent<RunnerController>().OnDeadEvent.AddListener(OnPlayerCatch);
            myIngamePlayer = runner;
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
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5, 5f));

        myRoomPlayer.GetComponent<PlayerControllerParent>().SetActiveTo(false);

        if (PhotonNetwork.LocalPlayer == seeker)
        {
            Debug.Log("You are Seeker");
            myIngamePlayer = PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("You are Runner");
            GameObject runner = PhotonNetwork.Instantiate("Runner", randomPos, Quaternion.identity);
            runner.GetComponent<RunnerController>().OnDeadEvent.AddListener(OnPlayerCatch);
            myIngamePlayer = runner;
        }
    }

    [PunRPC]
    private void RPC_StartGame()
    {
        currentState = GameState.Playing;
        timer = gameDuration;
        timeSlider.maxValue = gameDuration;
        timeSlider.value = gameDuration;
        timeSlider.gameObject.SetActive(true);
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

        resultText.text = message;
        resultText.gameObject.SetActive(true); // 결과 텍스트 활성화
        timeSlider.gameObject.SetActive(false); // 슬라이더 비활성화

        StartCoroutine(ReturnToLobby());
    }

    private IEnumerator ReturnToLobby()
    {

        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(myIngamePlayer);
        myRoomPlayer.transform.position = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5, 5f));
        myRoomPlayer.GetComponent<RoomPlayerController>().SetActiveTo(true);
        UnLoadScene();

        Debug.Log("LeaveRoom");

        Camera.main.transform.SetParent(null);
        Camera.main.GetComponent<CameraController>().FollowTarget = null;

    }

    public override void OnLeftRoom()
    {
        Debug.Log("LeftRoom");
        //PhotonNetwork.LoadLevel("LobbyScene"); // 로비 씬으로 복귀
    }

    public void OnPlayerCatch() {

        photonView.RPC("OnPlayerCatchRpc", RpcTarget.MasterClient);

    }

    [PunRPC]
    public void OnPlayerCatchRpc()
    {
        runnersRemaining--;
        Debug.LogWarning("남은사람 : " + runnersRemaining);
        if (runnersRemaining <= 0)
        {
            EndGame("Seekers Win");
        }
    }
}
