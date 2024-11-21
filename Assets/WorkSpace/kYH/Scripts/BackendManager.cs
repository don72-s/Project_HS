using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    private FirebaseApp app;
    public static FirebaseApp App { get { return Instance.app; } }

    private FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return Instance.auth; } }

    private FirebaseDatabase database;
    public static FirebaseDatabase Database { get { return Instance.database; } }

    private void Awake()
    {
        // �ش� ��ũ��Ʈ�� ������ ������Ʈ�� �̱������� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // �����ͺ��̽� ������ �˻� �Լ�
    private void CheckDependency()
    {
        // ���� ��� : ��û�� �Ϳ� ���� task�� �񵿱������ ȣȯ�� üũ�ϴ� ���
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                Debug.Log("Firebase ������ üũ ����!!!!!!!!!");

                DatabaseReference root = BackendManager.Database.RootReference;
                DatabaseReference userData = root.Child("UserData");

                // TODO : (���ϴ� ������ ��� ��ġ)�ڸ��� �����ͺ��̽����� ���� ��θ� �ۼ�
                // DatabaseReference data = userData.Child("(���ϴ� ������ ��� ��ġ)");
                //
                //data.SetValueAsync(3);
            }
            else
            {
                Debug.LogError($"Firebase ������ üũ ����! (���� : {task.Result})");
                app = null;
                auth = null;
                database = null;
            }
        });
    }
}
