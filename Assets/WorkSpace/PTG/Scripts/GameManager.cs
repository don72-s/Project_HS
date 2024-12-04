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

    public float freezeTime = 15f;

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

    float[] timerUnitArr = new float[3] { 60f, 120f, 180f };

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
        toggle60.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle60, isOn));
        toggle120.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle120, isOn));
        toggle180.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle180, isOn));

        //UpdateTimeBasedOnToggle();

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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

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

        if (currentState == GameState.Playing)
        {
            if (otherPlayer == currentSeeker)
            {
                EndGame("Runners Win Seeker Left");
            }
            else
            {
                if (otherPlayer.GetAlive() == false)
                {
                    photonView.RPC("UpdateRunnersRemaining", RpcTarget.All, runnersRemaining);
                }
                else
                {
                    photonView.RPC("UpdateRunnersRemaining", RpcTarget.All, runnersRemaining - 1);
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

    public void TestGameStart()
    {
        if (currentState == GameState.Playing || currentState == GameState.InLoading)
            return;

        currentState = GameState.InLoading;

        if (PhotonNetwork.IsMasterClient)
        {
            List<Player> allPlayers = new List<Player>(PhotonNetwork.PlayerList);

            runnersRemaining = allPlayers.Count - 1;
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
            SetTeamsAndSpwan();

            photonView.RPC("RPC_StartGame", RpcTarget.All);

            StartCoroutine(WaitPlayerSpawnCO());
        }
        else
        {
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
            return;
        }
        inLoadingPlayer--;
    }

    private void SetTeamsAndSpwan()
    {
        List<Player> allPlayers = new List<Player>(PhotonNetwork.PlayerList);

        int randomIndex = Random.Range(0, allPlayers.Count);
        currentSeeker = allPlayers[randomIndex];

        photonView.RPC("RPC_SetSeeker", RpcTarget.All, currentSeeker.ActorNumber);

        runnersRemaining = allPlayers.Count - 1;
    }

    [PunRPC]
    private void RPC_SetSeeker(int seekerActorNumber)
    {
        currentSeeker = PhotonNetwork.CurrentRoom.GetPlayer(seekerActorNumber);
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 1000, Random.Range(-5, 5f));

        myRoomPlayer.GetComponent<PlayerControllerParent>().SetActiveTo(false);

        if (PhotonNetwork.LocalPlayer == currentSeeker)
        {
            myIngamePlayer = PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);

            photonView.RPC("SeekerFreeze", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, freezeTime);
        }
        else
        {
            GameObject runner = PhotonNetwork.Instantiate("Runner", randomPos, Quaternion.identity);
            runner.GetComponent<RunnerController>().OnDeadEvent.AddListener(OnPlayerCatch);
            myIngamePlayer = runner;
        }
    }

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

    [PunRPC]
    void PlayCountdownRpc(int count)
    {
        if (currentState == GameState.Playing)
            return;

        countdownAni.PlayCountdown(count);
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
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CustomProperties.TIME))
        {
            int idx = PhotonNetwork.CurrentRoom.GetTimeIdx();

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

    //private void UpdateTimeBasedOnToggle()
    //{
    //    if (gameDuration == 60f)
    //        toggle60.isOn = true;
    //    else if (gameDuration == 120f)
    //        toggle120.isOn = true;
    //    else if (gameDuration == 180f)
    //        toggle180.isOn = true;
    //}
}
