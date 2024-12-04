using System.Collections.Generic;
using UnityEngine;

public class StageData : MonoBehaviour
{
    public static StageData Instance = null;

    public enum StageType { STAGE1,  STAGE2 }

    [field: SerializeField]
    public ChangeableObjsSO[] ChangeableSOArr { get; private set; }

    List<IFormChangeable> changeableObjList = new List<IFormChangeable>();

    readonly string[] playerMixerName = { "Volume0", "Volume1", "Volume2",
                                          "Volume3", "Volume4", "Volume5",
                                          "Volume6", "Volume7", "Volume8", "Volume9" };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 제공받은 스테이지에 대응하는 변신물체 목록 반환
    /// </summary>
    /// <param name="stage">스테이지 타입</param>
    /// <returns>물체 so데이터 반환, 없을경우 null 반환</returns>
    public ChangeableObjsSO GetStageChangeableObj(StageType stage)
    {

        foreach(ChangeableObjsSO soData in ChangeableSOArr)
        {
            if (soData.stage == stage)
            {
                return soData;
            }
        }

        return null;

    }

    public void AddChangeableObj(IFormChangeable obj) { 
        changeableObjList.Add(obj);
    }

    public void RemoveChangeableObj(IFormChangeable obj)
    {
        if (changeableObjList.Contains(obj))
        {
            changeableObjList.Remove(obj);
        }
    }

    [ContextMenu("StartChange")]
    public void StartChangeFormSlot() {

        foreach (IFormChangeable runner in changeableObjList)
        {
            runner.StartFormChange();
        }
    }

    public string GetPlayerMixerName(int playerNumber) {

        if (playerNumber < 0 || playerNumber >= playerMixerName.Length) {
            Debug.Log("믹서 이름 배열에 대응되지 않는 숫자가 입력됨");
            return string.Empty;
        }

        return playerMixerName[playerNumber];

    }

}
