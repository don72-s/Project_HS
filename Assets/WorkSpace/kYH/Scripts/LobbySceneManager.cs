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
using WebSocketSharp;

public class LobbySceneManager : BaseUI
{
    [Header("Login Panel")]
    [SerializeField] private TMP_InputField _emailInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private GameObject _signupPanel;
    [SerializeField] private GameObject _verifyPanel;
    [SerializeField] private GameObject _nicknamePanel;

    [Header("Sign UP Panel")]
    [SerializeField] private TMP_InputField _emailSignupInput;
    [SerializeField] private TMP_InputField _passwordSignupInput;
    [SerializeField] private TMP_InputField _passwordConfirmInput;

    [Header("Nickname Panel")]
    [SerializeField] private TMP_InputField _nicknameInput;

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

    #region Login UI
    public void Login(PointerEventData eventData)
    {
        string email = _emailInput.text;
        string password = _passwordInput.text;

        if (string.IsNullOrEmpty(email))
        {
            // TODO : �̸��� �Է��϶�� �ȳ� �˾�â ����
            Debug.LogWarning("�̸��� �������!!!!!!!!!!!!!!!!");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            // TODO : ��й�ȣ �Է��϶�� �ȳ� �˾�â ����
            Debug.LogWarning("��й�ȣ �������!!!!!!!!!!!!!!!!");
            return;
        }

        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogWarning("�α��� ���!!!!!!!");
                return;
            }
            if (task.IsFaulted)
            {
                // TODO : �α��� ���� �˾�â ����
                Debug.LogWarning($"�α��� ����! : " + task.Exception);
                return;
            }

            AuthResult result = task.Result;
            Debug.Log($"�α��� ����! (�̸� : {result.User.DisplayName} / UID : {result.User.UserId}");
            CheckUserInfo();
        });
    }

    public void SignUp_01(PointerEventData eventData)
    {
        _signupPanel.SetActive(true);
    }

    public void Settings(PointerEventData eventData)
    {
        // TODO : ���� �г� Ȱ��ȭ
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
            Debug.LogWarning("��ȿ�� �̸����� �Է����ּ���!!!!!!!!!!!!!");
            // TODO : ��ȿ�� �̸��� �Է� �˸� �˾�â ����
            return;
        }
        if (password != confirm)
        {
            Debug.LogWarning("��Ȯ�� ��й�ȣ�� �Է����ּ���!!!!!!!!!!!!!");
            // TODO : ��Ȯ�� ��й�ȣ �Է� �˸� �˾�â ����
            return;
        }

        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("ȸ�������� ��ҵǾ����ϴ�.");
                return;
            }
            if (task. IsFaulted)
            {
                Debug.LogError($"ȸ�����Կ� �����߽��ϴ�! (���� : {task.Exception})");
                return;
            }

            AuthResult result = task.Result;
            Debug.Log($"ȸ�������� �Ϸ�ƽ��ϴ�! (UID : {result.User.UserId})");
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
            Debug.LogWarning("�г��� �Է��϶��!!!!!!!!!!!!!!!!");
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
                Debug.LogError("�г��� ������ ��ҵǾ����ϴ�.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"�г��� ������ �����߽��ϴ�! (���� : {task.Exception})");
                return;
            }

            Debug.Log("�г��� ������ �Ϸ�Ǿ����ϴ�!");

            Debug.Log($"Display Name : {user.DisplayName}");
            Debug.Log($"E-Mail : {user.Email}");
            Debug.Log($"E-Mail Varified : {user.IsEmailVerified}");
            Debug.Log($"User ID : {user.UserId}");

            PhotonNetwork.LocalPlayer.NickName = nickname;
            PhotonNetwork.ConnectUsingSettings();
            _nicknamePanel.SetActive(false);
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

    private void SendVerificationMail()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("���� �̸��� �����Ⱑ ��ҵƽ��ϴ�.");
                gameObject.SetActive(false);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"�̸��� ������ �����߽��ϴ�! (���� : {task.Exception})");
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
                    Debug.LogError("���� �̸��� �����Ⱑ ��ҵƽ��ϴ�.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"�̸��� ������ �����߽��ϴ�! (���� : {task.Exception})");
                    return;
                }

                if (BackendManager.Auth.CurrentUser.IsEmailVerified == true)
                {
                    Debug.Log("�̸��� ������ �Ϸ�Ǿ����ϴ�!!!!!!!!!!!!!!!");
                    _nicknamePanel.SetActive(true);
                    _verifyPanel.SetActive(false);
                }
            });
            yield return delay;
        }
    }
}
