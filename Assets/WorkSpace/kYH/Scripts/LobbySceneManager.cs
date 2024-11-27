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

public class LobbySceneManager : BaseUI
{
    [Header("Login Panel")]
    [SerializeField] private TMP_InputField _emailInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private GameObject _signupPanel;
    [SerializeField] private GameObject _verifyPanel;
    [SerializeField] private GameObject _nicknamePanel;
    [SerializeField] private DataManager _dataManager;

    [Header("Sign UP Panel")]
    [SerializeField] private TMP_InputField _emailSignupInput;
    [SerializeField] private TMP_InputField _passwordSignupInput;
    [SerializeField] private TMP_InputField _passwordConfirmInput;

    [Header("Nickname Panel")]
    [SerializeField] private TMP_InputField _nicknameInput;
    [SerializeField] private GameObject _lobbyPanel;

    [Header("Lobby Panel")]
    [SerializeField] private GameObject _createRoomPanel;
    private Dictionary<string, RoomEntry> _roomDic = new Dictionary<string, RoomEntry>();
    [SerializeField] private RoomEntry _roomPrefab;
    [SerializeField] private RectTransform _roomTransform;

    [Header("Create Room Panel")]
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private TMP_InputField _maxPlayerInput;

    [Header("Room Panel")]
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        BindAll();
    }

    private void Start()
    {
        AddEvent("LoginButton", EventType.Click, Login);
        AddEvent("SignupButton_01", EventType.Click, SignUp_01);
        AddEvent("SettingButton", EventType.Click, Settings);
        AddEvent("QuitGameButton", EventType.Click, QuitGame);
        AddEvent("SignupButton_02", EventType.Click, SignUp_02);
        AddEvent("CancelButton_01", EventType.Click, Cancel_01);
        AddEvent("CancelButton_02", EventType.Click, Cancel_02);
        AddEvent("CheckButton", EventType.Click, CheckNickname);
        AddEvent("CancelButton_03", EventType.Click, Cancel_03);
        AddEvent("CreateRoomButton", EventType.Click, CreateRoom);
        AddEvent("RandomMatchButton", EventType.Click, MatchRandom);
        AddEvent("LogOutButton", EventType.Click, LogOut);
        AddEvent("CreateButton", EventType.Click, MakeRoom);
        AddEvent("CancelButton_04", EventType.Click, Cancel_04);
        AddEvent("BackToLobbyButton", EventType.Click, BackToLobby);
        AddEvent("StartButton", EventType.Click, StartGame);
    }

    #region Login UI
    public void Login(PointerEventData eventData)
    {
        string email = _emailInput.text;
        string password = _passwordInput.text;

        if (string.IsNullOrEmpty(email))
        {
            // TODO : 이메일 입력하라는 안내 팝업창 생성
            Debug.LogWarning("이메일 적으라고!!!!!!!!!!!!!!!!");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            // TODO : 비밀번호 입력하라는 안내 팝업창 생성
            Debug.LogWarning("비밀번호 적으라고!!!!!!!!!!!!!!!!");
            return;
        }

        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogWarning("로그인 취소!!!!!!!");
                return;
            }
            if (task.IsFaulted)
            {
                // TODO : 로그인 실패 팝업창 생성
                Debug.LogWarning($"로그인 실패! : " + task.Exception);
                return;
            }

            AuthResult result = task.Result;
            Debug.Log($"로그인 성공! (이름 : {result.User.DisplayName} / UID : {result.User.UserId}");
            CheckUserInfo();
        });
    }

    public void SignUp_01(PointerEventData eventData)
    {
        _signupPanel.SetActive(true);
    }

    public void Settings(PointerEventData eventData)
    {
        // TODO : 설정 패널 활성화
    }

    public void QuitGame(PointerEventData eventData)
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
    #endregion

    #region Sign Up UI
    public void SignUp_02(PointerEventData eventData)
    {
        string email = _emailSignupInput.text;
        string password = _passwordSignupInput.text;
        string confirm = _passwordConfirmInput.text;

        if (email.IsNullOrEmpty())
        {
            Debug.LogWarning("유효한 이메일을 입력해주세요!!!!!!!!!!!!!");
            // TODO : 유효한 이메일 입력 알림 팝업창 생성
            return;
        }
        if (password != confirm)
        {
            Debug.LogWarning("정확한 비밀번호를 입력해주세요!!!!!!!!!!!!!");
            // TODO : 정확한 비밀번호 입력 알림 팝업창 생성
            return;
        }

        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("회원가입이 취소되었습니다.");
                return;
            }
            if (task. IsFaulted)
            {
                Debug.LogError($"회원가입에 실패했습니다! (사유 : {task.Exception})");
                return;
            }

            AuthResult result = task.Result;
            Debug.Log($"회원가입이 완료됐습니다! (UID : {result.User.UserId})");
            _signupPanel.SetActive(false);
        });
    }

    public void Cancel_01(PointerEventData eventData)
    {
        _signupPanel.SetActive(false);
    }
    #endregion

    #region Verify UI
    public void Cancel_02(PointerEventData eventData)
    {
        _verifyPanel.SetActive(false);
    }
    #endregion

    #region Nickname UI
    public void CheckNickname(PointerEventData eventData)
    {
        string nickname = _nicknameInput.text;

        if (nickname == "")
        {
            Debug.LogWarning("닉네임 입력하라고!!!!!!!!!!!!!!!!");
            return;
        }

        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null) return;

        UserProfile profile = new UserProfile();
        profile.DisplayName = nickname;

        BackendManager.Auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("닉네임 설정이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"닉네임 설정에 실패했습니다! (사유 : {task.Exception})");
                return;
            }

            Debug.Log("닉네임 설정이 완료되었습니다!");

            Debug.Log($"Display Name : {user.DisplayName}");
            Debug.Log($"E-Mail : {user.Email}");
            Debug.Log($"E-Mail Varified : {user.IsEmailVerified}");
            Debug.Log($"User ID : {user.UserId}");

            PhotonNetwork.LocalPlayer.NickName = nickname;
            _nicknamePanel.SetActive(false);
            PhotonNetwork.ConnectUsingSettings();
        });
    }

    public void Cancel_03(PointerEventData eventData)
    {
        _nicknamePanel.SetActive(false);
    }
    #endregion

    #region Lobby UI
    public void CreateRoom(PointerEventData eventData)
    {
        _createRoomPanel.SetActive(true);
    }

    public void MatchRandom(PointerEventData eventData)
    {
        string name = $"Room {Random.Range(1000, 10000)}";                              // 방 이름을 랜덤으로 설정
        RoomOptions options = new RoomOptions() { MaxPlayers = 10 };                     // 방 최대 인원 수를 8로 설정
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    public void LogOut(PointerEventData eventData)
    {
        _nicknamePanel.SetActive(false);
        PhotonNetwork.Disconnect();
    }
    #endregion

    #region Create Room UI
    public void MakeRoom(PointerEventData eventData)
    {
        string roomName = _roomNameInput.text;
        int maxPlayer = int.Parse(_maxPlayerInput.text);
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 10);

        if (roomName == "")
        {
            Debug.LogWarning("방 이름 입력하라고!!!!!!!!!!!!");
            // TODO : 방 이름을 입력하라는 팝업창 생성
            return;
        }

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;

        _createRoomPanel.SetActive(false);
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void Cancel_04(PointerEventData eventData)
    {
        _createRoomPanel.SetActive(false);
    }
    #endregion

    #region Room UI
    public void BackToLobby(PointerEventData eventData)
    {
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame(PointerEventData eventData)
    {
        PhotonNetwork.LoadLevel("GameScene");
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    #endregion

    private void CheckUserInfo()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        if (user == null) return;

        Debug.Log($"Display Name : {user.DisplayName}");
        Debug.Log($"E-Mail : {user.Email}");
        Debug.Log($"E-Mail Varified : {user.IsEmailVerified}");
        Debug.Log($"User ID : {user.UserId}");

        if (user.IsEmailVerified == false)
        {
            _verifyPanel.gameObject.SetActive(true);
            SendVerificationMail();
        }
        else if (user.DisplayName == "")
        {
            _nicknamePanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("데이터 매니저 나와 주세요.");
            _dataManager.CheckLogin();
        }
    }

    private void SendVerificationMail()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("인증 이메일 보내기가 취소됐습니다.");
                gameObject.SetActive(false);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"이메일 인증에 실패했습니다! (사유 : {task.Exception})");
                gameObject.SetActive(false);
                return;
            }

            Debug.Log("Email sent successfully.");
            checkRoutine = StartCoroutine(CheckRoutine());
        });
    }

    Coroutine checkRoutine;
    IEnumerator CheckRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(2.0f);

        while (true)
        {
            BackendManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("인증 이메일 보내기가 취소됐습니다.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"이메일 인증에 실패했습니다! (사유 : {task.Exception})");
                    return;
                }

                if (BackendManager.Auth.CurrentUser.IsEmailVerified == true)
                {
                    Debug.Log("이메일 인증이 완료되었습니다!!!!!!!!!!!!!!!");
                    _verifyPanel.SetActive(false);
                    _nicknamePanel.SetActive(true);
                }
            });
            yield return delay;
        }
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList == true || info.IsVisible == false || info.IsOpen == false)
            {
                if (_roomDic.ContainsKey(info.Name) == false)
                    continue;

                Destroy(_roomDic[info.Name].gameObject);   
                _roomDic.Remove(info.Name);                
            }
            else if (_roomDic.ContainsKey(info.Name) == false)
            {
                RoomEntry roomEntry = Instantiate(_roomPrefab, _roomTransform);
                _roomDic.Add(info.Name, roomEntry);
                roomEntry.SetRoomInfo(info);
            }
            else if (_roomDic.ContainsKey((string)info.Name) == true)
            {
                RoomEntry roomEntry = _roomDic[info.Name];
                roomEntry.SetRoomInfo(info);
            }
        }
    }
}
