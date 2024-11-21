using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSlotMachine : MonoBehaviour
{

    [SerializeField]
    GameObject[] objectArr;

    IFormChangeable changeableObj = null;

    ChangeUIBinder changeUIBinder;
    List<Button> buttons = new List<Button>();
    List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

    int[] idxArr;

    private void Awake()
    {
        changeUIBinder = GetComponent<ChangeUIBinder>();

        buttons.Add(changeUIBinder.GetUI<Button>("ChangeButton1"));
        buttons.Add(changeUIBinder.GetUI<Button>("ChangeButton2"));
        buttons.Add(changeUIBinder.GetUI<Button>("ChangeButton3"));

        SetButtonsEnable(false);

        texts.Add(changeUIBinder.GetUI<TextMeshProUGUI>("ObjectText1"));
        texts.Add(changeUIBinder.GetUI<TextMeshProUGUI>("ObjectText2"));
        texts.Add(changeUIBinder.GetUI<TextMeshProUGUI>("ObjectText3"));
    }


    public void SetChangeable(IFormChangeable changeable) { 
        changeableObj = changeable;
    }

    void SetButtonsEnable(bool isEnable) {

        foreach (var button in buttons)
        {
            button.enabled = isEnable;
        }

    }

    public void StartSlot3() {

        if (objectArr.Length < 3) {
            Debug.Log("3개 이상 필요.");
            return;
        }

        if (changeableObj == null) {
            Debug.Log("변신할 대상이 없음");
            return;
        }

        HashSet<int> idxes = new HashSet<int>();

        while (idxes.Count < 3) { 
        
            idxes.Add(Random.Range(0, objectArr.Length));

        }

        idxArr = idxes.ToArray();

        for (int i = 0; i < idxArr.Length; i++)
        {

            texts[i].text = objectArr[idxArr[i]].name;

        }

        SetButtonsEnable(true);

    }

    public void ChangeObjButton1() {
        changeableObj.ChangeForm(idxArr[0]);
        CloseWindow();
    }
    public void ChangeObjButton2() {
        changeableObj.ChangeForm(idxArr[1]);
        CloseWindow();
    }
    public void ChangeObjButton3() {
        changeableObj.ChangeForm(idxArr[2]);
        CloseWindow();
    }

    public void OpenWindow() {
        SetButtonsEnable(false);
        gameObject.SetActive(true);
    }

    public void CloseWindow() {
        gameObject.SetActive(false);
    }

}
