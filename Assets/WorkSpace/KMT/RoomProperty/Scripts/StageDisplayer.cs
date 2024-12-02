using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StageDisplayer : MonoBehaviourPunCallbacks
{

    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(waitRoomPropertyCO());
    }

    IEnumerator waitRoomPropertyCO()
    {

        while ((int)PhotonNetwork.CurrentRoom.GetStage() == -1)
        {
            yield return null;
        }

        text.text = (PhotonNetwork.CurrentRoom.GetStage()).ToString();

    }

/*    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PhotonNetwork.CurrentRoom.SetStage(StageData.StageType.STAGE2);
        }
    }*/

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

        if (propertiesThatChanged.ContainsKey(CustomProperties.STAGE))
        {
            text.text = ((StageData.StageType)propertiesThatChanged[CustomProperties.STAGE]).ToString();
        }

    }

}
