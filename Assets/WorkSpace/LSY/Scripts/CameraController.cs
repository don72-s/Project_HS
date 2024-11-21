using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private bool hasFollowTarget; // 타겟이 존재하는지 안하는지 확인하는 변수
    private Transform followTarget; // 카메라가 따라갈 타겟 트랜스폼

    [SerializeField] Camera gunCamera;
    [SerializeField] GameObject player;
    public Transform FollowTarget
    {
        get => followTarget;
        set
        {
            followTarget = value;

            if (followTarget != null)
            {
                hasFollowTarget = true;
                gunCamera.gameObject.SetActive(true);
                player.gameObject.SetActive(true);
            }
            else
            {
                hasFollowTarget = false;
                gunCamera.gameObject.SetActive(false);
                player.gameObject.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        SetTransform();
    } 

    private void SetTransform()
    {
        if (!hasFollowTarget) return;
        followTarget.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
