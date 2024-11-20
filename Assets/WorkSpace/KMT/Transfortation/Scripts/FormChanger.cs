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
        //TODO : �÷��̾� ȸ����? ���� ȸ����?
        GameObject tmpObj = Instantiate(objList[destObjidx], transform);

        //�̹� ��ü�� ������ ����
        if (curBodyObject != null)
        {
            Destroy(curBodyObject);
        }

        curBodyObject = tmpObj;

    }

}
