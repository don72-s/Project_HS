using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerParent : MonoBehaviourPun
{

    public void SetActiveTo(bool active)
    {

        if (photonView.IsMine)
        {
            photonView.RPC("SetActiveToRpc", RpcTarget.All, active);
        }

    }

    [PunRPC]
    void SetActiveToRpc(bool active)
    {
        gameObject.SetActive(active);
    }

}
