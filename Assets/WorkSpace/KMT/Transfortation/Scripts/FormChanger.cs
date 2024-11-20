using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class FormChanger : MonoBehaviourPunCallbacks, 
    IFormChangeable
{

    [SerializeField]
    List<GameObject> objList = new List<GameObject>();

    [SerializeField]
    ChangeUIBinder changeCanvasPrefab;

    ChangeUIBinder changeCanvas;
    GameObject curBodyObject;

    private void Start()
    {
        changeCanvas = Instantiate(changeCanvasPrefab, transform);
        changeCanvas.gameObject.SetActive(false);
        changeCanvas.GetComponent<ObjectSlotMachine>().SetChangeable(this);
        curBodyObject = null;
    }


    [ContextMenu("to Cube")]
    public void testA()
    {
        if (!photonView.IsMine)
            return;
        photonView.RPC("ChangeFormRpc", RpcTarget.All, 0);
    }

    [ContextMenu("to Capsule")]
    public void testB()
    {
        if (!photonView.IsMine)
            return;
        photonView.RPC("ChangeFormRpc", RpcTarget.All, 1);
    }

    public void ChangeForm(int objIdx)
    {
        if (!photonView.IsMine)
            return;
        photonView.RPC("ChangeFormRpc", RpcTarget.All, objIdx);
    }

    [PunRPC]
    public void ChangeFormRpc(int destObjidx)
    {
        //TODO : 플레이어 회전값? 고정 회전값?
        GameObject tmpObj = Instantiate(objList[destObjidx], transform);

        //이미 몸체가 있으면 제거
        if (curBodyObject != null)
        {
            Destroy(curBodyObject);
        }

        curBodyObject = tmpObj;

    }

}
