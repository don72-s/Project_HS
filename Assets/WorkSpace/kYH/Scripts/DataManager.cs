using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Realtime;
using System;
using static BackendManager;

public class DataManager : MonoBehaviour
{
    [SerializeField] private string _email;
    public string Email { get { return _email; } set { _email = value; } }

    [SerializeField] private string _nickname;
    public string Nickname { get { return _nickname; } set { _nickname = value; } }

    [SerializeField] private int _level;
    public int Level {  get { return _level; } set { _level = value; } }

    [SerializeField] private int _exp;
    public int EXP { get { return _exp; } set { _exp = value; } }

    private DatabaseReference _userDataRef;
    private DatabaseReference _levelRef;

    private void OnEnable()
    {
        string uid = BackendManager.Auth.CurrentUser.UserId;
        _userDataRef = BackendManager.Database.RootReference.Child("Userdata").Child(uid);
        _levelRef = _userDataRef.Child("level");

        _userDataRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // 값 읽기를 취소한 경우
            if (task.IsCanceled)
            {
                Debug.LogWarning("Getting Value Fail!");
                return;
            }
            // 값 읽기를 실패한 경우
            if (task.IsFaulted)
            {
                Debug.LogWarning($"Getting Value Fail! : {task.Exception.Message}");
                return;
            }

            Debug.Log("Getting Value Success!");
            DataSnapshot snapshot = task.Result;    // 값 읽기 성공
            //snapshot.Child("Name");
            //snapshot.Child("email");

            if (snapshot.Value == null)
            {
                // 초기값으로 세팅
                UserData userData = new UserData();
                //userData.name = BackendManager.Auth.CurrentUser.DisplayName;
                //userData.email = BackendManager.Auth.CurrentUser.Email;
                //userData.level = 1;

                //userData.list.Add("First");
                //userData.list.Add("Second");
                //userData.list.Add("Third");

                string json = JsonUtility.ToJson(userData);
                _userDataRef.SetRawJsonValueAsync(json);
            }
            else
            {
                //// 읽어 온 값으로 세팅
                //string json = snapshot.GetRawJsonValue();
                //Debug.Log(json);
                //
                //UserData userData = JsonUtility.FromJson<UserData>(json);
                //Debug.Log(userData.name);
                //Debug.Log(userData.email);
                //Debug.Log(userData.level);
                //Debug.Log(userData.list[0]);
                //Debug.Log(userData.list[1]);
                //Debug.Log(userData.list[2]);

                //level = int.Parse(snapshot.Child("level").Value.ToString());
                //Debug.Log(level);
            }
        });

        _levelRef.ValueChanged += LevelRef_ValueChanged;
    }

    private void OnDisable()
    {
        _levelRef.ValueChanged -= LevelRef_ValueChanged;
    }

    private void LevelRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"값 변경 이벤트 확인 : {e.Snapshot.Value.ToString()}");
        _level = int.Parse(e.Snapshot.Value.ToString());
    }
}
