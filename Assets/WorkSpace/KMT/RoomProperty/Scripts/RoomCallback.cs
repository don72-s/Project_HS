using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCallback : MonoBehaviourPunCallbacks
{
    public override void OnCreatedRoom()
    {
        PhotonNetwork.CurrentRoom.SetStage(StageData.StageType.STAGE1);
        PhotonNetwork.CurrentRoom.SetTimeIdx(1);
    }

}
