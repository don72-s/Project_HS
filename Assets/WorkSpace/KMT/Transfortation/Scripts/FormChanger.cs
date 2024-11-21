using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class FormChanger : MonoBehaviourPunCallbacks, 
    IFormChangeable
{
    GameObject[] objArr;

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

        objArr = StageData.Instance.ChangeableSO.ChangeableObjArr;
        StageData.Instance.AddChangeableObj(this);
    }


    public void StartFormChange() {

        photonView.RPC("StartFormChangeRpc", RpcTarget.AllViaServer);

    }

    [PunRPC]
    void StartFormChangeRpc() {

        if (photonView.IsMine)
        {
            changeCanvas.GetComponent<ObjectSlotMachine>().StartSlot3();
        }

    }

    public void ChangeForm(int objIdx)
    {
        if (!photonView.IsMine)
            return;
        photonView.RPC("ChangeFormRpc", RpcTarget.AllViaServer, objIdx);
    }

    private void OnDestroy()
    {
        StageData.Instance.RemoveChangeableObj(this);
    }

    [PunRPC]
    public void ChangeFormRpc(int destObjidx)
    {
        //TODO : 플레이어 회전값? 고정 회전값?
        GameObject tmpObj = Instantiate(objArr[destObjidx], transform);

        //이미 몸체가 있으면 제거
        if (curBodyObject != null)
        {
            Destroy(curBodyObject);
        }

        curBodyObject = tmpObj;

    }

}
