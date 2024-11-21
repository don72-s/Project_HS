using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool _hasFollowTarget; // 타겟이 존재하는지 안하는지 확인하는 변수
    private Transform _followTarget; // 카메라가 따라갈 타겟 트랜스폼

    [SerializeField] Camera gunCamera;
    public Transform FollowTarget
    {
        get => _followTarget;
        set
        {
            _followTarget = value;

            if (_followTarget != null)
            {
                _hasFollowTarget = true;
            }
            else
            {
                _hasFollowTarget = false;
                gunCamera.gameObject.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        SetTransform();
    } 

    private void SetTransform()
    {
        if (!_hasFollowTarget) return;
        Debug.Log("aaaa");
        _followTarget.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
