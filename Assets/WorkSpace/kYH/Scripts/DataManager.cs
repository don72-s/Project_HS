using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Realtime;
using System;
using Photon.Pun;

public class DataManager : MonoBehaviour
{
    [SerializeField] private int _level;
    public int Level {  get { return _level; } set { _level = value; } }
    
    [SerializeField] private int _exp;
    public int EXP { get { return _exp; } set { _exp = value; } }

    [SerializeField] private int _curExp;
    public int CurEXP { get { return _curExp; } set { _curExp = value; } }

    [SerializeField] private int _maxExp;
    public int MaxEXP { get { return _maxExp; } set { _maxExp = value; } }

    private bool _isOnline;

    private DatabaseReference _userDataRef;
    private DatabaseReference _onlineRef;
    private DatabaseReference _levelRef;
    private DatabaseReference _expRef;
    private DatabaseReference _curExpRef;
    private DatabaseReference _maxExpRef;

    private void OnEnable()
    {
        string uid = BackendManager.Auth.CurrentUser.UserId;
        _userDataRef = BackendManager.Database.RootReference.Child("UserData").Child(uid);
        _levelRef = _userDataRef.Child("_level");
        _onlineRef = _userDataRef.Child("_isOnline");
        _curExpRef = _userDataRef.Child("_curExp");
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

            if (snapshot.Value == null)
            {
                // 초기값으로 세팅
                UserData userData = new UserData();
                userData._email = BackendManager.Auth.CurrentUser.Email;
                userData._name = BackendManager.Auth.CurrentUser.DisplayName;
                userData._isOnline = true;
                userData._level = 1;
                userData._curExp = 0;
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

                /*// 코루틴 넣을 부분
                // TODO : n초 대기 코루틴 구현 필요
                if (userData._isOnline == false)
                {
                    Debug.LogWarning("안된다고!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    BackendManager.Auth.SignOut();
                    return;
                }

                _onlineRef.ValueChanged += (object sender, ValueChangedEventArgs e) => 
                {
                    Debug.LogWarning("다른사람이다!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    _onlineRef.SetValueAsync(false);
                };

                Debug.Log(userData._email);
                Debug.Log(userData._name);
                Debug.Log(userData._isOnline);
                Debug.Log(userData._level);
                Debug.Log(userData._curExp);
                Debug.Log(userData._maxExp);
                // 코루틴 넣을 부분 끝*/
            }
        });

        _levelRef.ValueChanged += LevelRef_ValueChanged;
        /*_curExpRef.ValueChanged += CurEXPRef_ValueChanged;
        _maxExpRef.ValueChanged += MaxEXPRef_ValueChanged;*/
    }

    private void OnDisable()
    {
        _levelRef.ValueChanged -= LevelRef_ValueChanged;
        /*_curExpRef.ValueChanged -= CurEXPRef_ValueChanged;
        _maxExpRef.ValueChanged -= MaxEXPRef_ValueChanged;*/
    }

    IEnumerator WaitingRoutine(UserData userData)
    {
        

        yield return new WaitForSeconds(3f);

        if (userData._isOnline == false)
        {
            Debug.LogWarning("안된다고!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            BackendManager.Auth.SignOut();
            yield break;
        }

        _onlineRef.ValueChanged += (object sender, ValueChangedEventArgs e) =>
        {
            Debug.LogWarning("다른사람이다!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            _onlineRef.SetValueAsync(false);
        };

        Debug.Log(userData._email);
        Debug.Log(userData._name);
        Debug.Log(userData._isOnline);
        Debug.Log(userData._level);
        Debug.Log(userData._curExp);
        Debug.Log(userData._maxExp);
    }

    private void LevelRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"값 변경 이벤트 확인 : {e.Snapshot.Value.ToString()}");
        _level = int.Parse(e.Snapshot.Value.ToString());
    }

    /*private void CurEXPRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"값 변경 이벤트 확인 : {e.Snapshot.Value.ToString()}");
        _curExp = int.Parse(e.Snapshot.Value.ToString());
    }
    
    private void MaxEXPRef_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Debug.Log($"값 변경 이벤트 확인 : {e.Snapshot.Value.ToString()}");
        _maxExp = int.Parse(e.Snapshot.Value.ToString());
    }*/

    public void LevelUp()
    {
        if (_curExp >= _maxExp)
        {
            _levelRef.SetValueAsync(_level + 1);
            _curExp = _curExp - _maxExp;
        }
    }

    /*public void CurEXPUp()
    {
        _curExpRef.SetValueAsync(_curExp + _exp);
    }*/
}

[Serializable]
public class UserData
{
    public string _email;
    public string _name;
    public bool _isOnline;
    public int _level;
    public int _curExp;
    public int _maxExp;
}
