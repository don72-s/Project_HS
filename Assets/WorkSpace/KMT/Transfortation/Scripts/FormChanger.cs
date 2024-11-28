using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RunnerController))]
public class FormChanger : MonoBehaviourPunCallbacks, 
    IFormChangeable
{
    GameObject[] objArr;

    [SerializeField]
    GameObject ghostObj;

    [SerializeField]
    ChangeUIBinder changeCanvasPrefab;

    ChangeUIBinder changeCanvas;

    bool isAlive = true;

    [Header("Base body Object")]
    [SerializeField]
    public GameObject curBodyObject = null;

    private void Start()
    {
        changeCanvas = Instantiate(changeCanvasPrefab, transform);
        changeCanvas.gameObject.SetActive(false);
        changeCanvas.GetComponent<ObjectSlotMachine>().SetChangeable(this);

        objArr = StageData.Instance.ChangeableSO.ChangeableObjArr;
        StageData.Instance.AddChangeableObj(this);

        GetComponent<RunnerController>().OnDeadEvent.AddListener(OnDead);
    }


    public void StartFormChange() {

        photonView.RPC("StartFormChangeRpc", RpcTarget.AllViaServer);

    }

    void OnDead() {

        ghostObj.SetActive(true);
        photonView.RPC("OnDeadRpc", RpcTarget.All);

    }

    [PunRPC]
    void OnDeadRpc() {

        StageData.Instance.RemoveChangeableObj(this);
        isAlive = false;

        if (curBodyObject != null)
        {
            Destroy(curBodyObject);
        }

    }

    [PunRPC]
    void StartFormChangeRpc() {

        if (!isAlive) {
            Debug.Log("이미 죽음");
            return;
        }

        if (photonView.IsMine)
        {
            changeCanvas.GetComponent<ObjectSlotMachine>().StartSlot3();
        }

    }

    public void ChangeForm(int objIdx)
    {
        if (!photonView.IsMine)
            return;

        if (!isAlive)
        {
            Debug.Log("이미 죽음");
            return;
        }

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
