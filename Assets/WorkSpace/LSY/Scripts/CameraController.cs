using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private bool hasFollowTarget; // Ÿ���� �����ϴ��� ���ϴ��� Ȯ���ϴ� ����
    private Transform followTarget; // ī�޶� ���� Ÿ�� Ʈ������

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
