using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
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
    List<Image> imgs = new List<Image>();
    TextMeshProUGUI countdownText;

    ObjArr[] objectArr = null;
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

        imgs.Add(changeUIBinder.GetUI<Image>("ObjectImg1"));
        imgs.Add(changeUIBinder.GetUI<Image>("ObjectImg2"));
        imgs.Add(changeUIBinder.GetUI<Image>("ObjectImg3"));

        countdownText = changeUIBinder.GetUI<TextMeshProUGUI>("CountdownText");

        countdownDelay = new WaitForSeconds(1);

        MouseLocker.Instance.MouseRealease();
    }

    public void InitObjArr() {

        ChangeableObjsSO soData = StageData.Instance.GetStageChangeableObj(
            PhotonNetwork.CurrentRoom.GetStage());
        if (soData == null)
        {
            Debug.LogError("대응되는 데이터정보가 없음!!!");
            return;
        }

        objectArr = soData.ChangeableObjs;

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
            Debug.LogWarning(idxArr[i]);
            imgs[i].sprite = objectArr[idxArr[i]].objSprite;
        }

        countdownCoroutine = StartCoroutine(CountdownCO());
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

    private void OnDisable()
    {
        MouseLocker.Instance.MouseLock();
    }

}
