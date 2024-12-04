using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomPlayerController : PlayerControllerParent, IPunObservable
{

    [Header("플레이어 움직임")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;

    [Header("플레이어 카메라")]
    [SerializeField] private float mouseX = 5f;
    [SerializeField] private float mouseY = 5f;

    public LayerMask collisionLayer;

    private bool isJumped;
    private Rigidbody rb;
    private float yRotation = 0f;
    private float xRotation = 0f;

    // 네트워크 상 지연보상을 주기위한 변수
    private Vector3 networkPosition;
    private float deltaPosition;

    private Quaternion networkRotation;
    private float deltaRotation;

    protected override void Start()
    {

        if (photonView.IsMine == false) return;

        rb = gameObject.GetComponent<Rigidbody>();

        isJumped = false;

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        Camera.main.transform.LookAt(transform.position);

        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.FollowTarget = null;

        PhotonNetwork.LocalPlayer.SetAlive(true);

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
    }


    private void LateUpdate()
    {
        if (photonView.IsMine == false) return;

        RotateCamera();
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

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumped)
        {
            isJumped = true;
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
        float z = Input.GetAxis("Vertical");

        SetPosition(z);

        if (Input.GetKey(KeyCode.A))
        {
            Rotate(-1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rotate(1);
        }
    }

    private void SetPosition(float input)
    {
        if (input == 0) 
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        Vector3 moveDirection = transform.forward * input;
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }

    private void Rotate(int direction)
    {
        float rotation = direction * rotateSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }


    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * this.mouseX * DataManager.Instance.OptionDataParams.MouseSensitivity;
        xRotation -= mouseY * this.mouseY * DataManager.Instance.OptionDataParams.MouseSensitivity;

        xRotation = Mathf.Clamp(xRotation, -20, 60);

        Camera.main.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // 카메라 위치
        Vector3 targetPosition = transform.position - Camera.main.transform.forward * 2f + Vector3.up;

        // 카메라와 플레이어 사이의 벡터값
        Vector3 rayDir = targetPosition - transform.position;

        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), rayDir, out hit, 3f, collisionLayer))
        {
            Camera.main.transform.position = hit.point - rayDir.normalized * 0.3f;
            //Debug.Log(hit.collider.name);
        }
        else
        {
            Camera.main.transform.position = targetPosition;
        }
    }

}
