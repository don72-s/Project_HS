using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBinder : MonoBehaviour
{

    //Ž���� ��ųʸ�
    Dictionary<string, GameObject> gameObjectDic;
    Dictionary<(string, System.Type), Component> componentDic;

    protected virtual void Awake()
    {
        Bind();
    }

    protected void Bind()
    {
        Transform[] trans = GetComponentsInChildren<Transform>(true);
        gameObjectDic = new Dictionary<string, GameObject>(trans.Length << 2);//��ųฮ �������� ���� �Ҵ�.    
        componentDic = new Dictionary<(string, System.Type), Component>();

        foreach (Transform _child in trans)
        {

            if (!gameObjectDic.TryAdd(_child.gameObject.name, _child.gameObject))
            {
                Debug.Log($"�̸��� ��ħ! : {_child.gameObject.name}");
            }
        }

    }

    //UI������Ʈ �̸����� ã�ƿ���
    public GameObject GetUI(in string _name)
    {
        gameObjectDic.TryGetValue(_name, out GameObject ret);
        return ret;
    }

    //�̸��� _name�� UI������Ʈ�� �ִ� TŸ�� ������Ʈ ��������
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
