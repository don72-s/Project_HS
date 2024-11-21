using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpareTester : MonoBehaviourPunCallbacks {

    public const string RoomName = "TestRoom234532342346";

    private void Start() {

        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster() {

        Debug.Log("���ӵ�");

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;

        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);

    }

    public override void OnJoinedRoom() {

        Debug.Log("�濡 ��������!");
        StartCoroutine(WaitCo());
    }

    IEnumerator WaitCo() {

        yield return new WaitForSeconds(1);
        TestGameStart();

    }

    public void TestGameStart() {

        Debug.Log("������ �����ߵ�;");

        SpawnPlayer();

    }

    void SpawnPlayer() {

        Debug.Log("�÷��̾ �����Ѵ�!");
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

    }

}

