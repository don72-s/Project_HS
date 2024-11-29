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
    /// �������� ���������� �����ϴ� ���Ź�ü ��� ��ȯ
    /// </summary>
    /// <param name="stage">�������� Ÿ��</param>
    /// <returns>��ü so������ ��ȯ, ������� null ��ȯ</returns>
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
            Debug.Log("�ͼ� �̸� �迭�� �������� �ʴ� ���ڰ� �Էµ�");
            return string.Empty;
        }

        return playerMixerName[playerNumber];

    }

}
