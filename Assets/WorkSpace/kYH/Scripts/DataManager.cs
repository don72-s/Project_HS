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
    [SerializeField] private string _nickname;
    public string Nickname { get { return _nickname; } set { _nickname = value; } }

    [SerializeField] private int _level;
    public int Level {  get { return _level; } set { _level = value; } }

    
}
