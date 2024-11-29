using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitArea : AreaTrigger
{
    [SerializeField]
    GameObject voice;

    protected override void OnEnterArea()
    {
        base.OnEnterArea();

        if (voice != null)
            Destroy(voice);

        PhotonNetwork.LeaveRoom();

    }

}
