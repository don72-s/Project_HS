using System.Collections.Generic;
using UnityEngine;

public class StageData : MonoBehaviour
{
    public static StageData Instance = null;

    [field: SerializeField]
    public ChangeableObjsSO ChangeableSO { get; private set; }

    List<IFormChangeable> changeableObjList = new List<IFormChangeable>();

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

        //TODO : 마스터 클라이언트만 호출 가능하도록 제한.
        foreach (IFormChangeable runner in changeableObjList)
        {
            runner.StartFormChange();
        }
    }

}
