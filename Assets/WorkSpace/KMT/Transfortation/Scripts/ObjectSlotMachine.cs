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
    int countdownSec;

    FormChanger changeableObj = null;

    ChangeUIBinder changeUIBinder;
    List<Button> buttons = new List<Button>();
    List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    TextMeshProUGUI countdownText;

    GameObject[] objectArr = null;
    int[] idxArr;

    Coroutine countdownCoroutine = null;
    WaitForSeconds countdownDelay;

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

        countdownText = changeUIBinder.GetUI<TextMeshProUGUI>("CountdownText");

        countdownDelay = new WaitForSeconds(1);
    }

    public void InitObjArr() {

        if(objectArr == null)
            objectArr = StageData.Instance.ChangeableSO.ChangeableObjArr;

    }

    public void SetChangeable(FormChanger changeable) { 
        changeableObj = changeable;
        InitObjArr();
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

        //슬롯 시작
        OpenWindow();
        MouseLocker.Instance.MouseRealease();

        HashSet<int> idxes = new HashSet<int>();

        while (idxes.Count < 3) { 
        
            idxes.Add(Random.Range(0, objectArr.Length));

        }

        idxArr = idxes.ToArray();

        for (int i = 0; i < idxArr.Length; i++)
        {

            texts[i].text = objectArr[idxArr[i]].name;

        }

        countdownCoroutine = StartCoroutine(CountdownCO());
        SetButtonsEnable(true);

    }

    public void ChangeObjButton1() {
        changeableObj.ChangeForm(idxArr[0]);
        MouseLocker.Instance.MouseLock();
        CloseWindow();
    }
    public void ChangeObjButton2() {
        changeableObj.ChangeForm(idxArr[1]);
        MouseLocker.Instance.MouseLock();
        CloseWindow();
    }
    public void ChangeObjButton3() {
        changeableObj.ChangeForm(idxArr[2]);
        MouseLocker.Instance.MouseLock();
        CloseWindow();
    }

    IEnumerator CountdownCO() {

        for (int i = countdownSec; i >= 0; i--) {

            countdownText.text = i.ToString();
            yield return countdownDelay;

        }

        ChangeObjButton1();
    }

    public void OpenWindow() {
        SetButtonsEnable(false);
        countdownText.text = "";
        gameObject.SetActive(true);
    }

    public void CloseWindow() {
        StopCoroutine(countdownCoroutine);
        countdownCoroutine = null;
        gameObject.SetActive(false);
    }

}
