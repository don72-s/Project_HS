using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ChangeableObjList")]
public class ChangeableObjsSO : ScriptableObject
{
    [field: SerializeField]
    public StageData.StageType stage {  get; private set; }

    [SerializeField]
    public ObjArr[] ChangeableObjs;

}

[System.Serializable]
public struct ObjArr {

    [field: SerializeField]
    public GameObject changeableObj { get; private set; }
    [field: SerializeField]
    public Sprite objSprite { get; private set; }

}
