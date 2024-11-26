using Photon.Pun;
using System.Collections;
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

    [Header("총 애니매이터")]
    [SerializeField] Animator animator;

    [Header("Muzzle Point")]
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] Transform runnerMuzzlePoint;

    [Header("오디오")]
    [SerializeField] AudioSource reloadAudio;
    [SerializeField] AudioSource shootAudio;

    private void Start()
    {
        attack = 1;
        bullet = MaxBullet;
        bulletText.text = MaxBullet.ToString();
    }

    private void Update()
    {
        if (photonView.IsMine == false) return;
        Debug.DrawRay(playerController.cameraPoint.position, playerController.cameraPoint.forward, Color.red);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("재장전 중");

            if (reloadRoutine == null)
            {
                reloadRoutine = StartCoroutine(ReloadGunRoutine());
            }
        }
    }

    public void Fire()
    {
        if (bullet <= 0)
        {
            Debug.Log("총알이 없습니다.");
            bulletText.text = "0";
            return;
        }

        if (shootRoutine == null)
        {
           shootRoutine = StartCoroutine(ShootRoutine());
        }
    }
    Coroutine reloadRoutine;
    IEnumerator ReloadGunRoutine()
    {
        reloadAudio.Play();
        animator.SetTrigger("Reload");

        yield return new WaitForSeconds(2.3f);
        bullet = MaxBullet;
        bulletText.text = bullet.ToString();
        Debug.Log("재장전 완료");
        reloadRoutine = null;
    }


    Coroutine shootRoutine;

    IEnumerator ShootRoutine()
    {
        bullet--;
        bulletText.text = bullet.ToString();

        RecoilMath();
        shootAudio.Play();

        Instantiate(muzzleFlash, muzzlePoint.transform.position, muzzlePoint.transform.rotation);
        PhotonNetwork.Instantiate("MuzzleFlash", runnerMuzzlePoint.transform.position, runnerMuzzlePoint.transform.rotation);

        if (Physics.Raycast(playerController.cameraPoint.position, playerController.cameraPoint.forward, out RaycastHit hit, range, targetLayer))
        {
            Debug.Log($"{hit.transform.name} Hit!!");
            if (hit.collider.gameObject.GetComponentInParent<RunnerController>() != null)
            {
                RunnerController runnerController = hit.collider.gameObject.GetComponentInParent<RunnerController>();
                runnerController.TakeDamage(attack);
            }
        }
        yield return new WaitForSeconds(0.5f);
        shootRoutine = null;
    }

    public void RecoilMath()
    {
        animator.SetTrigger("Shoot");
    }
}
