using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RunnerController : PlayerControllerParent, IPunObservable
{
    [Header("�÷��̾� ������")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;

    [Header("�÷��̾� ī�޶�")]
    [SerializeField] private float mouseX = 5f;  
    [SerializeField] private float mouseY = 5f;
    [SerializeField] Camera runnerCamera;

    [Header("�÷��̾� ü��")]
    [SerializeField] public int hp;
    [SerializeField] GameObject[] hpImages;
    [SerializeField] GameObject hpPanel;

    [SerializeField] FormChanger changer;

    private bool isJumped;
    private Rigidbody rb;
    private float yRotation = 0f;
    private float xRotation = 0f;

    // ��Ʈ��ũ �� ���������� �ֱ����� ����
    private Vector3 networkPosition;
    private float deltaPosition;

    private Quaternion networkRotation;
    private float deltaRotation;

    public UnityEvent OnDeadEvent = null;

    private Renderer curBodyRenderer;

    public LayerMask collisionLayer;

protected override void Start()
{
    base.Start();

    hp = 3;

    // �ڽ��� �÷��̾��� ��츸 ī�޶� Ȱ��ȭ
    if (photonView.IsMine)
    {
        rb = gameObject.GetComponent<Rigidbody>();
        isJumped = false;

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        // ī�޶� Ȱ��ȭ (�÷��̾� �ڽ��� ī�޶� Ȱ��ȭ)
        runnerCamera.gameObject.SetActive(true);
        runnerCamera.transform.LookAt(transform.position);

        hpPanel.gameObject.SetActive(true);
    }
    else
    {
        // �ٸ� �÷��̾��� ī�޶�� ��Ȱ��ȭ
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
            //gameObject.SetActive(false);
            curBodyRenderer = changer.curBodyObject.GetComponent<Renderer>();
            curBodyRenderer.enabled = false;
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

        yRotation += mouseX * this.mouseX;
        xRotation -= mouseY * this.mouseY;

        xRotation = Mathf.Clamp(xRotation, 0, 60);

        runnerCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // ī�޶� ��ġ
        Vector3 targetPosition = transform.position - runnerCamera.transform.forward * 2f + Vector3.up;

        // ī�޶�� �÷��̾� ������ ���Ͱ�
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
