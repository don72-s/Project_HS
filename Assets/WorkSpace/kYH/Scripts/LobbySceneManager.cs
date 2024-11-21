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

public class LobbySceneManager : BaseUI
{
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

    public void Login(PointerEventData eventData)
    {

    }

    public void SignUp_01(PointerEventData eventData)
    {

    }

    public void Settings(PointerEventData eventData)
    {

    }

    public void QuitGame(PointerEventData eventData)
    {

    }

    public void SignUp_02(PointerEventData eventData)
    {

    }

    public void Cancel_01(PointerEventData eventData)
    {

    }

    public void Cancel_02(PointerEventData eventData)
    {

    }

    public void CheckNickname(PointerEventData eventData)
    {

    }

    public void Cancel_03(PointerEventData eventData)
    {

    }

    public void CreateRoom(PointerEventData eventData)
    {

    }

    public void MatchRandom(PointerEventData eventData)
    {

    }

    public void LogOut(PointerEventData eventData)
    {

    }

    public void BackToLobby(PointerEventData eventData)
    {

    }

    public void StartGame(PointerEventData eventData)
    {

    }
}
