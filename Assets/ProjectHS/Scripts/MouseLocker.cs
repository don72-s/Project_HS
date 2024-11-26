using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLocker : MonoBehaviour
{
    public static MouseLocker Instance = null;

    int curMouseRelease = 0;

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
    /// ���콺 ��� ������ ī��Ʈ �߰�.
    /// </summary>
    public void MouseRealease() {

        if (curMouseRelease == 0) { 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        curMouseRelease++;

    }

    /// <summary>
    /// ���콺 ��ݰ� ī��Ʈ ����
    /// ī��Ʈ�� �����ִ°�� ����� ����
    /// </summary>
    public void MouseLock() {

        curMouseRelease--;

        if (curMouseRelease < 0)
            curMouseRelease = 0;

        if (curMouseRelease == 0) { 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }


}
