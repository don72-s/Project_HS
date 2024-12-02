using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeCanvas : MonoBehaviour
{
    [SerializeField]
    VoiceVolumePanel volumePanel;
    [SerializeField]
    GameObject settingPanel;
    [SerializeField]
    GameObject button;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {

            if (MouseLocker.Instance == null)
                return;         

            if (volumePanel.gameObject.activeSelf || settingPanel.gameObject.activeSelf)
            {
                MouseLocker.Instance.MouseLock();
                volumePanel.CloseWindow();
                button.SetActive(false);
                settingPanel.SetActive(false);
            }
            else
            {
                MouseLocker.Instance.MouseRealease();
                volumePanel.OpenWindow();
                button.SetActive(true);
            }

        }

    }
}
