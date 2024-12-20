using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RunnerController : PlayerControllerParent, IPunObservable
{
    [Header("플레이어 움직임")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;

    [Header("플레이어 카메라")]
    [SerializeField] private float mouseX = 5f;
    [SerializeField] private float mouseY = 5f;
    [SerializeField] Camera runnerCamera;

    [Header("플레이어 체력")]
    [SerializeField] public int hp;
    [SerializeField] GameObject[] hpImages;
    [SerializeField] GameObject hpPanel;

    [SerializeField] FormChanger changer;

    [SerializeField] Image lockImage;

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

    private Renderer curBodyRenderer;

    public LayerMask collisionLayer;

    bool isLocking = false;
    protected override void Start()
    {
        base.Start();

        hp = 3;

        // 자신의 플레이어일 경우만 카메라 활성화
        if (photonView.IsMine)
        {
            rb = gameObject.GetComponent<Rigidbody>();
            isJumped = false;

            networkPosition = transform.position;
            networkRotation = transform.rotation;

            // 카메라 활성화 (플레이어 자신의 카메라만 활성화)
            runnerCamera.gameObject.SetActive(true);
            runnerCamera.transform.LookAt(transform.position);

            hpPanel.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(false);

            rb.useGravity = true;

        }
        else
        {
            // 다른 플레이어의 카메라는 비활성화
            runnerCamera.gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        if (photonView.IsMine == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, deltaPosition * Time.deltaTime * PhotonNetwork.SerializationRate);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, deltaRotation * Time.deltaTime * PhotonNetwork.SerializationRate);
            return;
        }

        CheckLock();
        Jump();
        Move();
    }

    private void CheckLock()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

            if (isLocking)//품
            {
                isLocking = false;
                rb.useGravity = true;
                lockImage.gameObject.SetActive(false);
            }//
            else //잠금
            {
                isLocking = true;
                rb.useGravity = false;
                lockImage.gameObject.SetActive(true);
            }

        }

    }

    private void LateUpdate()
    {
        if (photonView.IsMine == false) return;

        RotateCamera();
    }

    [PunRPC]
    public void TakeDamageRpc(int damage, Player player)
    {
        hp -= damage;
        hpImages[hp + 1].gameObject.SetActive(false);

        //TODO : 시체샷 처리 + HP 인덱스 에러 수정 필요
        if (hp <= 0)
        {
            Debug.Log("player die");
            OnDeadEvent?.Invoke();
            hp = 0;
            if(player == PhotonNetwork.LocalPlayer)
                PhotonNetwork.LocalPlayer.SetAlive(false);
            //gameObject.SetActive(false);
            return;
        }

    }


    public void TakeDamage(int damage)
    {
        photonView.RPC("TakeDamageRpc", RpcTarget.AllViaServer, damage, photonView.Owner);
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
        if (isLocking)
        {
            rb.velocity = Vector3.zero;
            return;
        }

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

        xRotation = Mathf.Clamp(xRotation, 0, 60);

        runnerCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // 카메라 위치
        Vector3 targetPosition = transform.position - runnerCamera.transform.forward * 2f + Vector3.up;

        // 카메라와 플레이어 사이의 벡터값
        Vector3 rayDir = targetPosition - transform.position;

        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), rayDir, out hit, 3f, collisionLayer))
        {
            runnerCamera.transform.position = hit.point - rayDir.normalized * 0.3f;
            Debug.Log(hit.collider.name);
        }
        else
        {
            runnerCamera.transform.position = targetPosition;
        }
    }

    /*    private void RotateCamera()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yRotation += mouseX * this.mouseX;
            xRotation -= mouseY * this.mouseY;

            xRotation = Mathf.Clamp(xRotation, 0, 60);

            runnerCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            runnerCamera.transform.position = transform.position - runnerCamera.transform.forward * 2f + Vector3.up;
        }*/

}
