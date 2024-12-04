using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ObjHider : MonoBehaviourPun
{
    private void Start()
    {

        if (!PhotonNetwork.IsMasterClient)
            return;

        Debug.Log("qqqqqqqqqq");
        StartCoroutine(WaitLoading());
 
    }

    IEnumerator WaitLoading()
    {
        while(GameManager.Instance.currentState == GameManager.GameState.InLoading)
        {
            yield return null;
        }
        Debug.Log(GameManager.Instance.currentState + "qqqqqqqqqq");
        photonView.RPC("ToActiveRpc", RpcTarget.All, Random.Range(0, 2) == 0);

    }

    [PunRPC]
    public void ToActiveRpc(bool active)
    { 
        gameObject.SetActive(active);
    }

}
