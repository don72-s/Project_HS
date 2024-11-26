using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeCanvas : MonoBehaviour
{
    [SerializeField]
    VoiceVolumePanel volumePanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {

            if (MouseLocker.Instance == null)
                return;

            if (volumePanel.gameObject.activeSelf)
            {
                MouseLocker.Instance.MouseLock();
                volumePanel.CloseWindow();
            }
            else
            {
                MouseLocker.Instance.MouseRealease();
                volumePanel.OpenWindow();
            }

        }

    }
}
