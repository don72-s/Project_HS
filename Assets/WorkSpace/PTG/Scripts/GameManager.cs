using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoomPTK";

    public const string STAGE_MAP_NAME = "lsy_GameScene_Clone";

    public static GameManager Instance;

    // 상태
    public enum GameState { Waiting, Playing, Finished, InLoading }

    public GameState currentState = GameState.Waiting;

    // 시간 설정
    public float gameDuration = 120f;

    public float freezeTime = 5f;

    private float timer;

    public Toggle toggle60;
    public Toggle toggle120;
    public Toggle toggle180;

    // 승리 조건
    private int runnersRemaining;

    private Player currentSeeker;

    public TextMeshProUGUI resultText;
    public Slider timeSlider;
    public Image blind;

    public TextMeshProUGUI freezeTimer;

    GameObject myRoomPlayer;
    GameObject myIngamePlayer;

    // 룸플레이어 프리팹
    public GameObject[] ghostPrefabs;

    private List<int> ghostIndex = new List<int>();

    [SerializeField]
    RoomManager roomManager;

    [SerializeField]
    CountdownAni countdownAni;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
/*        if (PhotonNetwork.IsMasterClient)
        {
            EnableToggles(true);
        }
        else
        {
            EnableToggles(false);
        }*/

        toggle60.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle60, isOn));
        toggle120.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle120, isOn));
        toggle180.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle180, isOn));

        UpdateTimeBasedOnToggle();

        resultText.gameObject.SetActive(false);
        timeSlider.maxValue = gameDuration;
        timeSlider.value = gameDuration;

        AudioManager.instance.PlayBGMScene("Room");

        for (int i = 0; i < ghostPrefabs.Length; i++)
        {
            ghostIndex.Add(i);
        }

        StartCoroutine(WaitCO(2));
        StartCoroutine(waitRoomPropertyCO());

    }

    IEnumerator waitRoomPropertyCO()
    {

        while (PhotonNetwork.NetworkClientState == ClientState.Joining)
        {
            yield return null;
        }

        while (PhotonNetwork.CurrentRoom.GetTimeIdx() == -1)
        {
            yield return null;
        }

        int idx = PhotonNetwork.CurrentRoom.GetTimeIdx();

        if (idx == 0) { toggle60.isOn = true; toggle120.isOn = false; toggle180.isOn = false; }
        else if (idx == 1) { toggle60.isOn = false; toggle120.isOn = true; toggle180.isOn = false; }
        else if( idx == 2) { toggle60.isOn = false; toggle120.isOn = false; toggle180.isOn = true; }

        if (!PhotonNetwork.IsMasterClient)
        {
            toggle60.interactable = false;
            toggle120.interactable = false;
            toggle180.interactable = false;
        }

    }

    IEnumerator WaitCO(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 2, Random.Range(-5, 5));
        if (ghostIndex.Count > 0)
        {
            int randomIndex = Random.Range(0, ghostIndex.Count);

            GameObject selectedGhost = ghostPrefabs[ghostIndex[randomIndex]];

            myRoomPlayer = PhotonNetwork.Instantiate(selectedGhost.name, randomPos, Quaternion.identity);

            ghostIndex.RemoveAt(randomIndex);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ghostIndex.Count);
            foreach (var index in ghostIndex)
            {
                stream.SendNext(index);
            }
        }
        else
        {

            int count = (int)stream.ReceiveNext();
            ghostIndex.Clear();
            for (int i = 0; i < count; i++)
            {
                ghostIndex.Add((int)stream.ReceiveNext());
            }
        }
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
    }

    private bool isMasterClientSet = false; // 방장이 설정된 여부를 확인하는 변수
    private float previousGameDuration = 120; // 방장이 설정한 이전 게임 시간 저장

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        // 방장이 설정되었으면 더 이상 변경되지 않도록 합니다.
        /*        if (!isMasterClientSet)
                {
                    // 새 방장이 될 경우, 이전 방장의 설정을 반영
                    if (PhotonNetwork.IsMasterClient)
                    {
                        // 방장이 선택한 시간에 맞게 토글 상태 설정
                        if (previousGameDuration == 60f)
                        {
                            toggle60.isOn = true;
                            toggle120.interactable = true;
                            toggle180.interactable = true;
                            toggle60.interactable = false; // 방장은 다시 선택할 수 없도록
                        }
                        else if (previousGameDuration == 120f)
                        {
                            toggle120.isOn = true;
                            toggle60.interactable = true;
                            toggle180.interactable = true;
                            toggle120.interactable = false; // 방장은 다시 선택할 수 없도록
                        }
                        else if (previousGameDuration == 180f)
                        {
                            toggle180.isOn = true;
                            toggle60.interactable = true;
                            toggle120.interactable = true;
                            toggle180.interactable = false; // 방장은 다시 선택할 수 없도록
                        }
                    }
                    else
                    {
                        // 다른 클라이언트는 방장이 설정한 게임 시간을 그대로 반영
                        if (previousGameDuration == 60f)
                        {
                            toggle60.isOn = true;
                        }
                        else if (previousGameDuration == 120f)
                        {
                            toggle120.isOn = true;
                        }
                        else if (previousGameDuration == 180f)
                        {
                            toggle180.isOn = true;
                        }
                    }

                    // 방장이 설정한 후에는 더 이상 설정을 변경하지 않도록 플래그 설정
                    isMasterClientSet = true;
                }
                else
                {
                    // 방장이 이미 설정된 경우, 새 방장이 설정한 값을 갱신
                    if (PhotonNetwork.IsMasterClient)
                    {
                        // 새 방장이 선택한 시간을 이전 값으로 저장
                        if (toggle60.isOn) previousGameDuration = 60f;
                        if (toggle120.isOn) previousGameDuration = 120f;
                        if (toggle180.isOn) previousGameDuration = 180f;
                    }
                }*/

        if (PhotonNetwork.LocalPlayer == newMasterClient)
        {
            toggle60.interactable = true;
            toggle120.interactable = true;
            toggle180.interactable = true;
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.Log($"Player {otherPlayer.NickName} left the room.");

        if (currentState == GameState.Playing)
        {
            if (otherPlayer == currentSeeker)
            {
                Debug.Log("Seeker has left the game Runners win");
                EndGame("Runners Win Seeker Left");
            }
            else
            {
                if (otherPlayer.GetAlive() == false)
                {
                    photonView.RPC("UpdateRunnersRemaining", RpcTarget.All, runnersRemaining);
                    Debug.Log("죽은사람 탈주");
                }
                else
                {
                    photonView.RPC("UpdateRunnersRemaining", RpcTarget.All, runnersRemaining - 1);
                    Debug.Log("산 사람 탈주");
                }

                if (runnersRemaining <= 0)
                {
                    EndGame("Seeker Win");
                }
            }
        }
    }

    IEnumerator WaitPlayerSpawnCO()
    {
        yield return new WaitForSeconds(5f);
        StageData.Instance.StartChangeFormSlot();
    }

    //TODO : 게임 중복시작 무시 예외처리 필요. 임시 해결
    public void TestGameStart()
    {
        if (currentState == GameState.Playing || currentState == GameState.InLoading)
            return;

        currentState = GameState.InLoading;

        if (PhotonNetwork.IsMasterClient)
        {
            List<Player> allPlayers = new List<Player>(PhotonNetwork.PlayerList);

            runnersRemaining = allPlayers.Count - 1; // 나머지는 러너
            photonView.RPC("UpdateRunnersRemaining", RpcTarget.All, runnersRemaining);

            PhotonNetwork.CurrentRoom.IsOpen = false;

            LoadStage();
        }
    }

    private void LoadStage()
    {
        photonView.RPC("WaitLoading", RpcTarget.All);
        photonView.RPC("LoadSceneAdditive", RpcTarget.All);
    }

    [PunRPC]
    void WaitLoading()
    {
        StartCoroutine(WaitLoadStageCO());
    }

    int inLoadingPlayer;
    IEnumerator WaitLoadStageCO()
    {
        float waitTime = 0;
        inLoadingPlayer = PhotonNetwork.PlayerList.Length;
        while (waitTime < 20f && inLoadingPlayer > 0)
        {

            yield return null;
            waitTime += Time.deltaTime;
        }

        if (!PhotonNetwork.IsMasterClient || currentState == GameState.Playing)
        {
            yield break;
        }

        if (inLoadingPlayer == 0)
        {
            Debug.Log("로딩 완료!!");

            SetTeamsAndSpwan(); // 팀 배정 및 플레이어 스폰

            //타이머 활성화
            photonView.RPC("RPC_StartGame", RpcTarget.All);

            StartCoroutine(WaitPlayerSpawnCO());
        }
        else
        {
            Debug.Log("로딩 시간 초과...");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            photonView.RPC("CancelPlayGame", RpcTarget.All);
        }
    }

    [PunRPC]
    void LoadSceneAdditive()
    {
        AudioManager.instance.PlayBGMScene("Game");
        AsyncOperation op = SceneManager.LoadSceneAsync(STAGE_MAP_NAME, LoadSceneMode.Additive);
        op.completed += (_op) => { Debug.Log("완료!"); photonView.RPC("LoadSceneFinished", RpcTarget.MasterClient); };
    }
    
    [PunRPC]
    void CancelPlayGame()
    {
        if (SceneManager.GetSceneByName(STAGE_MAP_NAME).isLoaded)
        {
            SceneManager.UnloadSceneAsync(STAGE_MAP_NAME);
        }
    }

    void UnLoadScene()
    {
        if (SceneManager.GetSceneByName(STAGE_MAP_NAME).isLoaded)
        {
            SceneManager.UnloadSceneAsync(STAGE_MAP_NAME);
        }
    }

    [PunRPC]
    void LoadSceneFinished()
    {

        if (inLoadingPlayer <= 0)
        {
            Debug.Log("뭔가 잘못됨.");
            return;
        }
        inLoadingPlayer--;
    }

    private void SetTeamsAndSpwan()
    {
        List<Player> allPlayers = new List<Player>(PhotonNetwork.PlayerList);

        // 술래 한 명 랜덤 선택
        int randomIndex = Random.Range(0, allPlayers.Count);
        currentSeeker = allPlayers[randomIndex];

        photonView.RPC("RPC_SetSeeker", RpcTarget.All, currentSeeker.ActorNumber);

        runnersRemaining = allPlayers.Count - 1; // 나머지는 러너
    }

    [PunRPC]
    private void RPC_SetSeeker(int seekerActorNumber)
    {
        currentSeeker = PhotonNetwork.CurrentRoom.GetPlayer(seekerActorNumber);
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 1000, Random.Range(-5, 5f));

        myRoomPlayer.GetComponent<PlayerControllerParent>().SetActiveTo(false);

        if (PhotonNetwork.LocalPlayer == currentSeeker)
        {
            Debug.Log("You are Seeker");
            myIngamePlayer = PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);

            photonView.RPC("SeekerFreeze", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, freezeTime); // 술래 멈추기
        }
        else
        {
            Debug.Log("You are Runner");
            GameObject runner = PhotonNetwork.Instantiate("Runner", randomPos, Quaternion.identity);
            runner.GetComponent<RunnerController>().OnDeadEvent.AddListener(OnPlayerCatch);
            myIngamePlayer = runner;
        }

    }

    float[] timerUnitArr = new float[3] { 60f, 120f, 180f };

    [PunRPC]
    private void RPC_StartGame()
    {
        currentState = GameState.Playing;
        float duration = timerUnitArr[PhotonNetwork.CurrentRoom.GetTimeIdx()];
        timer = duration;
        timeSlider.maxValue = duration;
        timeSlider.value = duration;
        timeSlider.gameObject.SetActive(true);

        toggle60.gameObject.SetActive(false);
        toggle120.gameObject.SetActive(false);
        toggle180.gameObject.SetActive(false);

        Debug.Log("Game Start");

        roomManager.gameObject.SetActive(false);
    }

    private void EndGame(string message)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_EndGame", RpcTarget.All, message);
        }
        StartCoroutine(DelayBGM());
    }

    IEnumerator DelayBGM()
    {
        yield return new WaitForSeconds(3f);
        AudioManager.instance.PlayBGMScene("Room");
    }

    [PunRPC]
    private void RPC_EndGame(string message)
    {
        currentState = GameState.Finished;
        Debug.Log("Game Over: " + message);

        resultText.text = message;

        resultText.gameObject.SetActive(true);
        timeSlider.gameObject.SetActive(false);

        toggle60.gameObject.SetActive(true);
        toggle120.gameObject.SetActive(true);
        toggle180.gameObject.SetActive(true);

        PhotonNetwork.LocalPlayer.SetReady(false);

        StartCoroutine(ReturnToLobby());
        StartCoroutine(DelayBGM());
    }

    private IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(3f);

        currentState = GameState.Waiting;

        resultText.gameObject.SetActive(false);

        PhotonNetwork.Destroy(myIngamePlayer);
        myRoomPlayer.transform.position = new Vector3(Random.Range(-5f, 5f), 2, Random.Range(-5, 5f));
        myRoomPlayer.GetComponent<RoomPlayerController>().SetActiveTo(true);
        UnLoadScene();

        Debug.Log("LeaveRoom");

        Camera.main.transform.SetParent(null);
        Camera.main.GetComponent<CameraController>().FollowTarget = null;

        PhotonNetwork.LocalPlayer.SetAlive(true);

        roomManager.gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("LeftRoom");
        SceneManager.LoadSceneAsync("KYH_LobbyScene");
        AudioManager.instance.PlayBGMScene("Robby");
        PhotonNetwork.JoinLobby();
    }

    public void OnPlayerCatch()
    {
        if (currentState == GameState.Finished)
        {
            Debug.LogWarning("Game is finished.");
            return;
        }

        photonView.RPC("OnPlayerCatchRpc", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void OnPlayerCatchRpc()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentState == GameState.Finished)
            {
                Debug.LogWarning("Game is finished.");
                return;
            }

            photonView.RPC("UpdateRunnersRemaining", RpcTarget.All, runnersRemaining - 1);

            if (runnersRemaining <= 0)
            {
                EndGame("Seekers Win");
            }
        }
    }

    [PunRPC]
    private void UpdateRunnersRemaining(int Runners)
    {
        runnersRemaining = Runners;

        Debug.LogWarning("Remaining runners : " + runnersRemaining);
    }

    [PunRPC]
    private void SeekerFreeze(int seekerActorNumber, float freezeDuration)
    {
        GameObject seeker = myIngamePlayer;

        if (PhotonNetwork.LocalPlayer.ActorNumber == seekerActorNumber)
        {
            seeker.GetComponent<PlayerController>().enabled = false;

            blind.gameObject.SetActive(true);
        }

        photonView.RPC("UpdateFreezeTimer", RpcTarget.All, freezeDuration);
        StartCoroutine(UnfreezeSeeker(seeker, freezeDuration));
    }

    private IEnumerator UnfreezeSeeker(GameObject seeker, float freezeDuration)
    {
        yield return new WaitForSeconds(freezeDuration);

        if (PhotonNetwork.LocalPlayer.ActorNumber == currentSeeker.ActorNumber)
        {
            seeker.GetComponent<PlayerController>().enabled = true;

            blind.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void UpdateFreezeTimer(float freezeDuration)
    {
        StartCoroutine(CountdownFreezeTimer(freezeDuration));
    }

    private IEnumerator CountdownFreezeTimer(float freezeDuration)
    {
        freezeTimer.gameObject.SetActive(true);

        while (freezeDuration > 0)
        {
            freezeTimer.text = $"{Mathf.CeilToInt(freezeDuration)}";
            yield return new WaitForSeconds(1f);
            freezeDuration--;
        }

        freezeTimer.text = "";
        freezeTimer.gameObject.SetActive(false);
    }

    [PunRPC]
    void PlayCountdownRpc(int count)
    {
        if (currentState == GameState.Playing)
            return;

        countdownAni.PlayCountdown(count);
    }

    private void EnableToggles(bool enable)
    {
        toggle60.interactable = enable;
        toggle120.interactable = enable;
        toggle180.interactable = enable;
    }

    //마스터만 접근 가능.
    private void OnToggleChanged(Toggle changedToggle, bool isOn)
    {
        if (isOn)
        {

            if (changedToggle == toggle60)
            {
                PhotonNetwork.CurrentRoom.SetTimeIdx(0);
            }
            else if (changedToggle == toggle120)
            {
                PhotonNetwork.CurrentRoom.SetTimeIdx(1);
            }
            else if (changedToggle == toggle180)
            {
                PhotonNetwork.CurrentRoom.SetTimeIdx(2);
            }

/*            if (toggle60 != changedToggle) toggle60.interactable = true;
            if (toggle120 != changedToggle) toggle120.interactable = true;
            if (toggle180 != changedToggle) toggle180.interactable = true;

            if (changedToggle != toggle60 && toggle60.isOn) toggle60.isOn = false;
            if (changedToggle != toggle120 && toggle120.isOn) toggle120.isOn = false;
            if (changedToggle != toggle180 && toggle180.isOn) toggle180.isOn = false;*/

/*            changedToggle.interactable = false;
*/
/*            if (currentState == GameState.Playing)
            {
                timer = gameDuration;
                timeSlider.value = gameDuration;
            }*/

/*            photonView.RPC("UpdateGameDuration", RpcTarget.Others, gameDuration);
*/        
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

        if (propertiesThatChanged.ContainsKey(CustomProperties.TIME))
        {

            int idx = PhotonNetwork.CurrentRoom.GetTimeIdx();

/*            toggle60.isOn = false;
            toggle120.isOn = false;
            toggle180.isOn = false;*/

            if (idx == 0)
            {
                toggle60.isOn = true;
                toggle120.isOn = false;
                toggle180.isOn = false;
            }
            else if (idx == 1)
            {
                toggle60.isOn = false;
                toggle120.isOn = true;
                toggle180.isOn = false;
            }
            else if (idx == 2)
            {
                toggle60.isOn = false;
                toggle120.isOn = false;
                toggle180.isOn = true;
            }
        }

    }

    [PunRPC]
    private void UpdateGameDuration(float newDuration)
    {
        gameDuration = newDuration; 
        if (currentState == GameState.Playing)
        {
            timer = gameDuration;
            timeSlider.value = gameDuration;
        }
    }

    private void UpdateTimeBasedOnToggle()
    {
        if (gameDuration == 60f)
            toggle60.isOn = true;
        else if (gameDuration == 120f)
            toggle120.isOn = true;
        else if (gameDuration == 180f)
            toggle180.isOn = true;
    }
}
