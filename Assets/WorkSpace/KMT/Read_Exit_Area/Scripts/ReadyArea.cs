using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyArea : AreaTrigger
{
    protected override void OnEnterArea()
    {
        base.OnEnterArea();
        PhotonNetwork.LocalPlayer.SetReady(true);
    }

    protected override void OnExitArea()
    {
        base.OnExitArea();
        PhotonNetwork.LocalPlayer.SetReady(false);
    }
}
