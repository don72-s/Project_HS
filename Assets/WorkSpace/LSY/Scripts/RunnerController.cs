using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RunnerController : PlayerControllerParent, IPunObservable
{
    [Header("플레이어 움직임")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;

    [Header("플레이어 카메라")]
    [SerializeField] private float mouseX = 5f;  
    [SerializeField] private float mouseY = 5f;

    [Header("플레이어 체력")]
    [SerializeField] public int hp;
    [SerializeField] GameObject[] hpImages;
    [SerializeField] GameObject hpPanel;

    private bool isJumped;
    private Rigidbody rb;
    private float yRotation = 0f;
    private float xRotation = 0f;

    // 네트워크 상 지연보상을 주기위한 변수
    private Vector3 networkPosition;
    private float deltaPosition;

    private Quaternion networkRotation;
    private float deltaRotation;

    public UnityEvent OnDeadEvent = null;

    protected override void Start()
    {

        base.Start();

        hp = 3;

        if (photonView.IsMine == false) return;

        rb = gameObject.GetComponent<Rigidbody>();

        isJumped = false;

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        Camera.main.transform.LookAt(transform.position);

        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.FollowTarget = null;

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        hpPanel.gameObject.SetActive(true);

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

    [PunRPC]
    public void TakeDamageRpc(int damage)
    {
        hp -= damage;
        hpImages[hp+1].gameObject.SetActive(false);


        if (hp <= 0)
        {
            Debug.Log("player die");
            OnDeadEvent?.Invoke();
            hp = 0;
            gameObject.SetActive(false);
            return;
        }

    }

    public void TakeDamage(int damage)
    {
        photonView.RPC("TakeDamageRpc", RpcTarget.AllViaServer, damage);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(hp);
        }
        else if (stream.IsReading)
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            hp = (int)stream.ReceiveNext();

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
        if (input == 0) return;

        Vector3 moveDirection = transform.forward * input;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
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

        yRotation += mouseX * this.mouseX;
        xRotation -= mouseY * this.mouseY;

        xRotation = Mathf.Clamp(xRotation, 0, 60);

        Camera.main.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Camera.main.transform.position = transform.position - Camera.main.transform.forward * 5f + Vector3.up;
    }
}
