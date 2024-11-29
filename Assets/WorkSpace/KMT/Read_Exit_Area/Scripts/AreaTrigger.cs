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
            Debug.Log("�÷��̾� ����");
            if (!other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("�ٸ� �÷��̾�");
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
            Debug.Log("�÷��̾� ����");
            if (!other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("�ٸ� �÷��̾�");
                return;
            }
            OnExitArea();
        }
    }
}
