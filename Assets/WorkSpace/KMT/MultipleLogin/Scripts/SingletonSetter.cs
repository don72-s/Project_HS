using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonSetter : MonoBehaviour
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {

        DataManager.Create();

    }

}
