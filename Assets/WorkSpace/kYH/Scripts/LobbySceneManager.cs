using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class LobbySceneManager : BaseUI
{
    [Header("Login Panel")]
    [SerializeField] TMP_InputField _emailInput;
    [SerializeField] TMP_InputField _passwordInput;
    [SerializeField] GameObject _signupPanel;
    [SerializeField] GameObject _verifyPanel;
    [SerializeField] GameObject _nicknamePanel;

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
        AddEvent("BackToLobbyButton", EventType.Click, BackToLobby);
        AddEvent("StartButton", EventType.Click, StartGame);
    }

    #region Lobby UI
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

    }

    public void Cancel_01(PointerEventData eventData)
    {

    }
    #endregion

    #region Verify UI
    public void Cancel_02(PointerEventData eventData)
    {

    }
    #endregion

    #region Nickname UI
    public void CheckNickname(PointerEventData eventData)
    {

    }

    public void Cancel_03(PointerEventData eventData)
    {

    }
    #endregion

    #region Lobby UI
    public void CreateRoom(PointerEventData eventData)
    {

    }

    public void MatchRandom(PointerEventData eventData)
    {

    }

    public void LogOut(PointerEventData eventData)
    {

    }
    #endregion

    #region Room UI
    public void BackToLobby(PointerEventData eventData)
    {

    }

    public void StartGame(PointerEventData eventData)
    {

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
        }
        else if (user.DisplayName == "")
        {
            _nicknamePanel.gameObject.SetActive(true);
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}
