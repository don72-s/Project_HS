using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerParent : MonoBehaviourPun
{
    Speaker speaker = null;

    /// <summary>
    /// ����Ŀ�� ������������ �۾�. roomcontroller������ ȣ������ ����
    /// </summary>
    protected virtual void Start()
    {
        //speaker = SpeakerList.Instance.GetSpeaker(PlayerNumberingExtensions.GetPlayerNumber(
        //    GetComponent<PhotonView>().Owner));

        //speaker.transform.SetParent(transform);
        //speaker.transform.localPosition = Vector3.zero;
        //speaker.GetComponent<AudioSource>().spatialBlend = 1;
        //speaker.RestartPlayback();
    }

    private void OnDestroy()
    {
        if (speaker != null) {
            speaker.transform.SetParent(SpeakerList.Instance.transform);

            // ��� ������Ʈ�� enabled �Ӽ� ����
            foreach (var component in speaker.GetComponents<Component>())
            {
                if (component is Behaviour behaviour)
                {
                    behaviour.enabled = true;
                }
            }

            speaker.GetComponent<AudioSource>().spatialBlend = 0;
            speaker.RestartPlayback();
        }
    }

    public void SetActiveTo(bool active)
    {

        if (photonView.IsMine)
        {
            photonView.RPC("SetActiveToRpc", RpcTarget.All, active);
        }

    }

    [PunRPC]
    protected void SetActiveToRpc(bool active)
    {
        gameObject.SetActive(active);
    }

}
