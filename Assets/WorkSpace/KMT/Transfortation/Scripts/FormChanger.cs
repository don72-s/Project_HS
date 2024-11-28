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
            Debug.Log("�̹� ����");
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
            Debug.Log("�̹� ����");
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

        //TODO : �÷��̾� ȸ����? ���� ȸ����?
        GameObject tmpObj = Instantiate(objArr[destObjidx], transform);

        //�̹� ��ü�� ������ ����
        if (curBodyObject != null)
        {
            Destroy(curBodyObject);
        }

        curBodyObject = tmpObj;

    }

}
