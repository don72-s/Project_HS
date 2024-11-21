using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RunnerController : MonoBehaviourPun
{
    [Header("�÷��̾� ������")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;

    [Header("�÷��̾� ī�޶�")]
    [SerializeField] private float mouseX = 5f;  
    [SerializeField] private float mouseY = 5f;

    [Header("�÷��̾� ü��")]
    [SerializeField] public int hp;
    [SerializeField] GameObject[] hpImages;
    [SerializeField] GameObject hpPanel;

    private float interpolation; // ����ó����
    private bool isJumped;
    private Rigidbody rb;
    private float yRotation = 0f;
    private float xRotation = 0f;

    // ��Ʈ��ũ �� ���������� �ֱ����� ����
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Start()
    {
        hp = 3;

        if (photonView.IsMine == false) return;

        rb = gameObject.GetComponent<Rigidbody>();

        isJumped = false;
        interpolation = 13;

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        Camera.main.transform.LookAt(transform.position);

        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.FollowTarget = null;

/*        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;*/
        hpPanel.gameObject.SetActive(true);

    }

    private void Update()
    {
        if (photonView.IsMine == false)
        {
            // Lerp�� �̿��� ���� �÷��̾��� ��ġ�� ��Ʈ��ũ�󿡼� �̵��� ��ġ�� ����ó�� ���༭ ������ �ʰ� �ε巴�� ó������
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * interpolation); // �����ӿ� ���� �����ӵ� ����
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

    [PunRPC]
    public void TakeDamageRpc(int damage)
    {
        if (hp < 0)
        {
            Debug.Log("player die");
            return;
        }
        hp -= damage;
        Debug.Log(hp);
        hpImages[hp].gameObject.SetActive(false);
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

        Camera.main.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Camera.main.transform.position = transform.position - Camera.main.transform.forward * 10f + Vector3.up;
    }
}
