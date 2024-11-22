using System.Collections.Generic;
using UnityEngine;

public class StageData : MonoBehaviour
{
    public static StageData Instance = null;

    [field: SerializeField]
    public ChangeableObjsSO ChangeableSO { get; private set; }

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

    public void AddChangeableObj(IFormChangeable obj) { 
        changeableObjList.Add(obj);
    }

    public void RemoveChangeableObj(IFormChangeable obj)
    {
        changeableObjList.Remove(obj);
    }

    [ContextMenu("StartChange")]
    public void StartChangeFormSlot() {

        //TODO : ������ Ŭ���̾�Ʈ�� ȣ�� �����ϵ��� ����.
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
