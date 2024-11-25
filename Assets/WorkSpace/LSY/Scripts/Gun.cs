using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviourPun
{
    [Header("총 설정")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int attack;

    [Header("총알 설정")]
    [SerializeField] public int bullet;
    [SerializeField] public int MaxBullet = 6;
    [SerializeField] TextMeshProUGUI bulletText;

    [Header("플레이어 컨트롤러")]
    [SerializeField] PlayerController playerController;

    [SerializeField] Animator animator;

    private void Start()
    {
        attack = 1;
        bullet = MaxBullet;
        bulletText.text = MaxBullet.ToString();
    }

    private void Update()
    {
        if (photonView.IsMine == false ) return;
        Debug.DrawRay(playerController.muzzlePoint.position, playerController.muzzlePoint.forward, Color.red);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("재장전 중");

            if (reloadGun == null)
            {
                reloadGun = StartCoroutine(ReloadGunRoutine());
            }
        }
    }

    public void Fire(Transform origin)
    {
        if (bullet <= 0)
        {
            Debug.Log("총알이 없습니다.");
            bulletText.text = "0";
            return;
        }

        bullet--;
        bulletText.text = bullet.ToString();

        if (Physics.Raycast(playerController.muzzlePoint.position, playerController.muzzlePoint.forward, out RaycastHit hit, range, targetLayer))
        {
            Debug.Log($"{hit.transform.name} Hit!!");
            if (hit.collider.gameObject.GetComponentInParent<RunnerController>() == null) return;

            RunnerController runnerController = hit.collider.gameObject.GetComponentInParent<RunnerController>();
            runnerController.TakeDamage(attack);
        }
    }
    Coroutine reloadGun;
    IEnumerator ReloadGunRoutine()
    {
        animator.SetTrigger("Reload");
        yield return new WaitForSeconds(2.3f);
        bullet = MaxBullet;
        bulletText.text = bullet.ToString();
        Debug.Log("재장전 완료");
        reloadGun = null;
    }
}
