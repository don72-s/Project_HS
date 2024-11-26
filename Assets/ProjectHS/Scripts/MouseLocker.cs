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
    /// 마우스 잠금 해제와 카운트 추가.
    /// </summary>
    public void MouseRealease() {

        if (curMouseRelease == 0) { 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        curMouseRelease++;

    }

    /// <summary>
    /// 마우스 잠금과 카운트 감소
    /// 카운트가 남아있는경우 잠그지 않음
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
