using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class LSY_GameSceneTester : MonoBehaviourPunCallbacks
{

    public const string lsy_RoomName = "TestRoomlsy";

    private void Start()
    {

        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {

        Debug.Log("���ӵ�");

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;

        PhotonNetwork.JoinOrCreateRoom(lsy_RoomName, options, TypedLobby.Default);

    }

    public override void OnJoinedRoom()
    {

        Debug.Log("�濡 ��������!");
        StartCoroutine(WaitCo());
    }

    IEnumerator WaitCo()
    {

        yield return new WaitForSeconds(1);
        TestGameStart();

    }

    public void TestGameStart()
    {

        Debug.Log("������ �����ߵ�;");

        SpawnPlayer();

    }

    void SpawnPlayer()
    {
        Debug.Log("�÷��̾ �����Ѵ�!");
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5, 5));

        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
        {
            PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Runner", randomPos, Quaternion.identity);
        }
    }

}

