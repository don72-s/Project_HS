using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeUIBinder : UIBinder
{

    ObjectSlotMachine objectSlotMachine;

    protected override void Awake()
    {
        base.Awake();

        objectSlotMachine = GetComponent<ObjectSlotMachine>();

        GetUI<Button>("ChangeButton1").onClick.AddListener(objectSlotMachine.ChangeObjButton1);
        GetUI<Button>("ChangeButton2").onClick.AddListener(objectSlotMachine.ChangeObjButton2);
        GetUI<Button>("ChangeButton3").onClick.AddListener(objectSlotMachine.ChangeObjButton3);

    }
}
