using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ChangeableObjList")]
public class ChangeableObjsSO : ScriptableObject
{
    [SerializeField]
    public GameObject[] ChangeableObjArr;

}
