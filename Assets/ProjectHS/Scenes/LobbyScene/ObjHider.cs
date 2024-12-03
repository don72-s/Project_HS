using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ObjHider : MonoBehaviourPun
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            photonView.RPC("ToActiveRpc", RpcTarget.AllBuffered, Random.Range(0, 2) == 0);
        }
    }
    
    [PunRPC]
    public void ToActiveRpc(bool active)
    { 
        gameObject.SetActive(active);
    }

}
