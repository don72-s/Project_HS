using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeCanvas : MonoBehaviour
{
    [SerializeField]
    VoiceVolumePanel volumePanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (volumePanel.gameObject.activeSelf)
            {
                volumePanel.CloseWindow();
            }
            else
            {
                volumePanel.OpenWindow();
            }

        }

    }
}
