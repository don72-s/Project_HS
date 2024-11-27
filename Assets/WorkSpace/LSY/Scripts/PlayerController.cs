using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class PlayerController : PlayerControllerParent, IPunObservable
{

    private Gun gun;
    [Header("플레이어 움직임")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;
    float run;

    [Header("플레이어 카메라")]
    [SerializeField] Vector3 offset;
    [SerializeField] private float yRotationRange;
    [SerializeField] Camera gunCamera;
    [SerializeField] Camera mainCamera;

    [Header("애니매이터")]
    [SerializeField] Animator playerAni;
    [SerializeField] Animator gunAni;

    [Header("오디오")]
    [SerializeField] AudioSource walkAudio;
    [SerializeField] AudioSource jumpAudio;

    [SerializeField] GameObject targetPointImage;
    [SerializeField] public Transform cameraPoint;
    [SerializeField] bool isJumped; 
    private Rigidbody rb;
    private float yRotation = 0f;

    // 네트워크 상 지연보상을 주기위한 변수
    private Vector3 networkPosition;
    private float deltaPosition;

    private Quaternion networkRotation;
    private float deltaRotation;

    protected override void Start()
    {

        base.Start();

        if (photonView.IsMine == false) return;

        gun = GetComponent<Gun>();
        rb = gameObject.GetComponent<Rigidbody>();

        isJumped = false;

        run = moveSpeed * 2;

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        mainCamera.transform.SetParent(gameObject.transform);
        mainCamera.transform.position = gameObject.transform.position + offset;

        CameraController cam = mainCamera.GetComponent<CameraController>();
        cam.FollowTarget = cameraPoint;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        targetPointImage.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        gunCamera.gameObject.SetActive(true);

    }

    private void Update()
    {
        if (photonView.IsMine == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, deltaPosition * Time.deltaTime * PhotonNetwork.SerializationRate);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, deltaRotation * Time.deltaTime * PhotonNetwork.SerializationRate);
            return;
        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = run;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = run / 2;
        }

        Jump();
        Move();
        Fire();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            deltaPosition = Vector3.Distance(transform.position, networkPosition);
            deltaRotation = Quaternion.Angle(transform.rotation, networkRotation);
        }
    }

    GameObject holdFlash;
    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Fire");
            gun.Fire();
          
        }
    }


    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumped)
        {
            isJumped = true;
            playerAni.SetTrigger("Jump1");
            photonView.RPC("PlayJumpSound", RpcTarget.AllViaServer);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumped = false;
        }
    }

    public void Move()
    {
        Vector2 rotateInput = new( rotateInput.x = Input.GetAxis("Mouse X"), rotateInput.y = Input.GetAxis("Mouse Y"));
        SetRotation(rotateInput);

        Vector3 moveInput = new( Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        SetPosition(moveInput);
    }

    private void SetRotation(Vector2 input)
    {
        if (input == Vector2.zero)
        {
            playerAni.SetBool("RightTurn", false);
            playerAni.SetBool("LeftTurn", false);
            return;
        }


        if (input.x != 0)
        {
            if (input.x < 0)
            {
                playerAni.SetBool("LeftTurn", true);
                playerAni.SetBool("RightTurn", false);
            }
            else if (input.x > 0)
            {
                playerAni.SetBool("RightTurn", true);
                playerAni.SetBool("LeftTurn", false);
            }
            transform.Rotate(Vector3.up, input.x * rotateSpeed * Time.deltaTime);
        }

        if (input.y != 0)
        {
            yRotation = yRotation + -input.y * rotateSpeed * Time.deltaTime;
            yRotation = Mathf.Clamp(yRotation, -yRotationRange, yRotationRange);

            mainCamera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        }
    }

    private void SetPosition(Vector3 input)
    {
        if (input == Vector3.zero) 
        {
            playerAni.SetBool("Move", false);
            gunAni.SetBool("Move", false);
            return; 
        }

        photonView.RPC("PlayWalkSound", RpcTarget.AllViaServer);
        playerAni.SetBool("Move", true);
        gunAni.SetBool("Move", true);

        Vector3 moveDirection = transform.forward * input.z + transform.right * input.x;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    [PunRPC]
    private void PlayJumpSound()
    {
        jumpAudio.Play();
    }

    [PunRPC]
    private void PlayWalkSound()
    {
        walkAudio.Play();
    }

}
