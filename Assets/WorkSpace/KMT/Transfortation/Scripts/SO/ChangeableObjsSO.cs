using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ChangeableObjList")]
public class ChangeableObjsSO : ScriptableObject
{
    [field: SerializeField]
    public StageData.StageType stage {  get; private set; }

    [SerializeField]
    public GameObject[] ChangeableObjArr;

}
