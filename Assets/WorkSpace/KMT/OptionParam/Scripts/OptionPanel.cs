using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    [SerializeField]
    Slider bgmSlider;
    [SerializeField]
    Slider sfxSlider;
    [SerializeField]
    Slider sensitivitySlider;


    // Start is called before the first frame update
    void Start()
    {
        bgmSlider.value = DataManager.Instance.OptionDataParams.BGM_Volume;
        sfxSlider.value = DataManager.Instance.OptionDataParams.SFX_Volume;
        sensitivitySlider.value = DataManager.Instance.OptionDataParams.MouseSensitivity;


        CloseWindow();
    }

    private void OnEnable()
    {
        bgmSlider.onValueChanged.AddListener(ChangedBgmSlider);
        sfxSlider.onValueChanged.AddListener(ChangedSfxSlider);
        sensitivitySlider.onValueChanged.AddListener(ChangedSensitivitySlider);
    }

    private void OnDisable()
    {
        bgmSlider.onValueChanged.RemoveListener(ChangedBgmSlider);
        sfxSlider.onValueChanged.RemoveListener(ChangedSfxSlider);
        sensitivitySlider.onValueChanged.RemoveListener(ChangedSensitivitySlider);
    }


    void ChangedBgmSlider(float val)
    {
        DataManager.Instance.OptionDataParams.BGM_Volume = val;
    }
    void ChangedSfxSlider(float val)
    {
        DataManager.Instance.OptionDataParams.SFX_Volume = val;
    }

    void ChangedSensitivitySlider(float val)
    {
        DataManager.Instance.OptionDataParams.MouseSensitivity = val;
    }

    

    public void CloseWindow()
    { 
        gameObject.SetActive(false);
    }
}
