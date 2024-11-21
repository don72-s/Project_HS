using Photon.Pun;
using UnityEngine;

public class RunnerController : MonoBehaviourPun
{
    [Header("플레이어 움직임")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;

    [Header("플레이어 카메라")]
    [SerializeField] Vector3 offset;
    [SerializeField] private float mouseX = 5f;  
    [SerializeField] private float mouseY = 5f;

    private float interpolation; // 보간처리값
    private bool isJumped;
    private Rigidbody rb;
    private float yRotation = 0f;
    private float xRotation = 0f;

    // 네트워크 상 지연보상을 주기위한 변수
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Start()
    {
        if (photonView.IsMine == false) return;

        rb = gameObject.GetComponent<Rigidbody>();

        isJumped = false;
        interpolation = 13;

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        //Camera.main.transform.SetParent(gameObject.transform);
        Camera.main.transform.position = gameObject.transform.position + offset;
        //Camera.main.transform.LookAt(transform.position);

        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.FollowTarget = null;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        if (photonView.IsMine == false)
        {
            // Lerp를 이용해 현재 플레이어의 위치와 네트워크상에서 이동한 위치를 보간처리 해줘서 끊기지 않고 부드럽게 처리해줌
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * interpolation); // 프레임에 따라 보간속도 조절
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * interpolation);
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
        Vector3 moveInput = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        SetPosition(moveInput);
    }


    private void SetPosition(Vector3 input)
    {
        if (input == Vector3.zero) return;

        Vector3 moveDirection = transform.forward * input.z + transform.right * input.x;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float rotation = input.x * rotateSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * this.mouseX;
        xRotation -= mouseY * this.mouseY;

        //Camera.main.transform.position = transform.position + offset;
        Camera.main.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Camera.main.transform.position = transform.position - Camera.main.transform.forward * 10f + Vector3.up * 3f;
    }
}
