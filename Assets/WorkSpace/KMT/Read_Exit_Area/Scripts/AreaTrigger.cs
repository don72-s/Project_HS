using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviourPun
{

    protected virtual void OnEnterArea() { }
    protected virtual void OnExitArea() { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("플레이어 들어옴");
            if (!other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("다른 플레이어");
                return;
            }
            OnEnterArea();
        }
    }
    //
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("플레이어 나감");
            if (!other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("다른 플레이어");
                return;
            }
            OnExitArea();
        }
    }
}
