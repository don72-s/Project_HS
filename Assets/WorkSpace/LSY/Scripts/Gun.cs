using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviourPun
{
    [Header("�� ����")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int attack;

    [Header("�Ѿ� ����")]
    [SerializeField] public int bullet;
    [SerializeField] public int MaxBullet = 6;
    [SerializeField] TextMeshProUGUI bulletText;

    [Header("�÷��̾� ��Ʈ�ѷ�")]
    [SerializeField] PlayerController playerController;

    [Header("�� �ִϸ�����")]
    [SerializeField] Animator gunAni;
    [SerializeField] Animator uiAni;

    [Header("Muzzle Point")]
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] Transform runnerMuzzlePoint;

    [Header("�����")]
    [SerializeField] AudioSource reloadAudio;
    [SerializeField] AudioSource shootAudio;

    private void Start()
    {
        attack = 1;
        bullet = MaxBullet;
        bulletText.text = $"{bullet} / {MaxBullet}";
    }

    private void Update()
    {
        if (photonView.IsMine == false) return;
        Debug.DrawRay(playerController.cameraPoint.position, playerController.cameraPoint.forward, Color.red);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("������ ��");

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
            bullet = 0;
            Debug.Log("�Ѿ��� �����ϴ�.");
            bulletText.text = $"{bullet} / {MaxBullet}";
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
        photonView.RPC("PlayReloadSound", RpcTarget.AllViaServer);
        gunAni.SetTrigger("Reload");

        yield return new WaitForSeconds(2.3f);
        bullet = MaxBullet;
        bulletText.text = $"{bullet} / {MaxBullet}";
        Debug.Log("������ �Ϸ�");
        reloadRoutine = null;
    }


    Coroutine shootRoutine;

    IEnumerator ShootRoutine()
    {
        bullet--;
        bulletText.text = $"{bullet} / {MaxBullet}";

        RecoilMath();
        uiAni.SetTrigger("Shoot");
        photonView.RPC("PlayShootSound", RpcTarget.AllViaServer);

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
        gunAni.SetTrigger("Shoot");
    }

    [PunRPC]
    private void PlayReloadSound()
    {
        reloadAudio.Play();
    }

    [PunRPC]
    private void PlayShootSound()
    {
        shootAudio.Play();
    }
}
