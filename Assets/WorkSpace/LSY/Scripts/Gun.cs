using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float _range;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] PlayerController _playerController;

    private void Update()
    {
        Debug.DrawRay(_playerController.muzzlePoint.position, _playerController.muzzlePoint.forward, Color.red);
    }

    public void Fire(Transform origin)
    {
        RaycastHit hit;

        if (Physics.Raycast(_playerController.muzzlePoint.position, _playerController.muzzlePoint.forward, out hit, _range, _targetLayer))
        {
            Debug.Log($"{hit.transform.name} Hit!!");
        }
    }
}
