using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{

    private Gun gun;
    [Header("플레이어 움직임")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;

    [Header("플레이어 카메라")]
    [SerializeField] Vector3 offset;

    [SerializeField] GameObject targetPointImage;
    [SerializeField] public Transform muzzlePoint;
    [SerializeField] private float yRotationRange;

    [SerializeField] Animator animator;
    [SerializeField] Camera camera;

    [SerializeField] bool isJumped; 
    private Rigidbody rb;
    private float yRotation = 0f;

    // 네트워크 상 지연보상을 주기위한 변수
    private Vector3 networkPosition;
    private float deltaPosition;

    private Quaternion networkRotation;
    private float deltaRotation;

    private void Start()
    {
        if (photonView.IsMine == false) return;

        gun = GetComponent<Gun>();
        rb = gameObject.GetComponent<Rigidbody>();

        isJumped = false;

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        Camera.main.transform.SetParent(gameObject.transform);
        Camera.main.transform.position = gameObject.transform.position + offset;

        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.FollowTarget = muzzlePoint;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        targetPointImage.SetActive(true);
        camera.gameObject.SetActive(true);

    }

    private void Update()
    {
        if (photonView.IsMine == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, deltaPosition * Time.deltaTime * PhotonNetwork.SerializationRate);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, deltaRotation * Time.deltaTime * PhotonNetwork.SerializationRate);
            return;
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


    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Fire");
            animator.SetTrigger("Aim");
            gun.Fire(muzzlePoint);
        }
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumped)
        {
            isJumped = true;
            animator.SetTrigger("Jump1");
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
            animator.SetBool("RightTurn", false);
            animator.SetBool("LeftTurn", false);
            return;
        }


        if (input.x != 0)
        {
            if (input.x < 0)
            {
                animator.SetBool("LeftTurn", true);
                animator.SetBool("RightTurn", false);
            }
            else if (input.x > 0)
            {
                animator.SetBool("RightTurn", true);
                animator.SetBool("LeftTurn", false);
            }
            transform.Rotate(Vector3.up, input.x * rotateSpeed * Time.deltaTime);
        }

        if (input.y != 0)
        {
            yRotation = yRotation + -input.y * rotateSpeed * Time.deltaTime;
            yRotation = Mathf.Clamp(yRotation, -yRotationRange, yRotationRange);

            Camera.main.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        }
    }

    private void SetPosition(Vector3 input)
    {
        if (input == Vector3.zero) {
            animator.SetFloat("Move", -1);
            return; } 
        animator.SetFloat("Move", 1);
        Vector3 moveDirection = transform.forward * input.z + transform.right * input.x;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

}
