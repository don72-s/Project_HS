using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool _hasFollowTarget; // Ÿ���� �����ϴ��� ���ϴ��� Ȯ���ϴ� ����
    private Transform _followTarget; // ī�޶� ���� Ÿ�� Ʈ������

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
