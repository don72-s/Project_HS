using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Realtime;
using System;

public class DataManager : MonoBehaviour
{
    //[SerializeField] private string _email;
    //public string Email { get { return _email; } set { _email = value; } }
    //
    //[SerializeField] private string _nickname;
    //public string Nickname { get { return _nickname; } set { _nickname = value; } }
    //
    [SerializeField] private int _level;
    public int Level {  get { return _level; } set { _level = value; } }
    //
    //[SerializeField] private int _exp;
    //public int EXP { get { return _exp; } set { _exp = value; } }
    //
    //[SerializeField] private int _maxExp;
    //public int MaxEXP { get { return _maxExp; } set { _maxExp = value; } }

    private DatabaseReference _userDataRef;
    private DatabaseReference _levelRef;
    private DatabaseReference _expRef;
    private DatabaseReference _maxExpRef;

    private void OnEnable()
    {
        string uid = BackendManager.Auth.CurrentUser.UserId;
        _userDataRef = BackendManager.Database.RootReference.Child("UserData").Child(uid);
        _levelRef = _userDataRef.Child("_level");
        _expRef = _userDataRef.Child("_exp");
        _maxExpRef = _userDataRef.Child("_maxExp");

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
                userData._email = BackendManager.Auth.CurrentUser.Email;
                userData._name = BackendManager.Auth.CurrentUser.DisplayName;
                userData._level = 1;
                userData._exp = 0;
                userData._maxExp = 100;

                string json = JsonUtility.ToJson(userData);
                _userDataRef.SetRawJsonValueAsync(json);
            }
            else
            {
                // 읽어 온 값으로 세팅
                string json = snapshot.GetRawJsonValue();
                Debug.Log(json);
                
                UserData userData = JsonUtility.FromJson<UserData>(json);
                Debug.Log(userData._email);
                Debug.Log(userData._name);
                Debug.Log(userData._level);
                Debug.Log(userData._exp);
                Debug.Log(userData._maxExp);
            }
        });

        _levelRef.ValueChanged += LevelRef_ValueChanged;
        /*_expRef.ValueChanged += EXPRef_ValueChanged;
        _expRef.ValueChanged += MaxEXPRef_ValueChanged;*/
    }

    public void LevelUp()
    {
        _level++;
    }

    private void OnDisable()
    {
        _levelRef.ValueChanged -= LevelRef_ValueChanged;
        //_expRef.ValueChanged -= EXPRef_ValueChanged;
        //_expRef.ValueChanged -= MaxEXPRef_ValueChanged;
    }

    private void LevelRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"값 변경 이벤트 확인 : {e.Snapshot.Value.ToString()}");
        _level = int.Parse(e.Snapshot.Value.ToString());
    }
    
    //private void EXPRef_ValueChanged(object sender, ValueChangedEventArgs e)
    //{
    //    Debug.Log($"값 변경 이벤트 확인 : {e.Snapshot.Value.ToString()}");
    //    _exp = int.Parse(e.Snapshot.Value.ToString());
    //}
    //
    //private void MaxEXPRef_ValueChanged(object sender, ValueChangedEventArgs e)
    //{
    //    Debug.Log($"값 변경 이벤트 확인 : {e.Snapshot.Value.ToString()}");
    //    _maxExp = int.Parse(e.Snapshot.Value.ToString());
    //}
}

[Serializable]
public class UserData
{
    public string _email;
    public string _name;
    public int _level;
    public int _exp;
    public int _maxExp;
}
