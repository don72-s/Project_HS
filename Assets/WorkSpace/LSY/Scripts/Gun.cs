using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviourPun
{
    [SerializeField] private float range;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] PlayerController playerController;
    [SerializeField] private int attack;

    private void Start()
    {
        attack = 1;
    }

    private void Update()
    {
        Debug.DrawRay(playerController.muzzlePoint.position, playerController.muzzlePoint.forward, Color.red);
    }


    public void Fire(Transform origin)
    {
        RaycastHit hit;

        if (Physics.Raycast(playerController.muzzlePoint.position, playerController.muzzlePoint.forward, out hit, range, targetLayer))
        {
            Debug.Log($"{hit.transform.name} Hit!!");
            if (hit.collider.gameObject.GetComponentInParent<RunnerController>() == null) return;

            RunnerController runnerController = hit.collider.gameObject.GetComponentInParent<RunnerController>();
            runnerController.TakeDamage(attack);
        }
    }
}
