using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBinder : MonoBehaviour
{

    //탐색용 딕셔너리
    Dictionary<string, GameObject> gameObjectDic;
    Dictionary<(string, System.Type), Component> componentDic;

    protected virtual void Awake()
    {
        Bind();
    }

    protected void Bind()
    {
        Transform[] trans = GetComponentsInChildren<Transform>(true);
        gameObjectDic = new Dictionary<string, GameObject>(trans.Length << 2);//딕셔녀리 여유공간 포함 할당.    
        componentDic = new Dictionary<(string, System.Type), Component>();

        foreach (Transform _child in trans)
        {

            if (!gameObjectDic.TryAdd(_child.gameObject.name, _child.gameObject))
            {
                Debug.Log($"이름이 겹침! : {_child.gameObject.name}");
            }
        }

    }

    //UI오브젝트 이름으로 찾아오기
    public GameObject GetUI(in string _name)
    {
        gameObjectDic.TryGetValue(_name, out GameObject ret);
        return ret;
    }

    //이름이 _name인 UI오브젝트에 있는 T타입 컴포넌트 가져오기
    public T GetUI<T>(in string _name) where T : Component
    {

        (string, Type) key = (_name, typeof(T));


        if (!componentDic.ContainsKey(key))
        {

            gameObjectDic.TryGetValue(_name, out GameObject obj);
            if (obj == null) return null;

            T comp = obj.GetComponent<T>();
            if (comp == null) return null;

            componentDic.TryAdd(key, comp);
        }

        return componentDic[key] as T;

    }

}
