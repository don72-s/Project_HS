using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    private Gun gun;

    [Header("�÷��̾� ������")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpForce;

    [Header("�÷��̾� ī�޶�")]
    [SerializeField] Vector3 offset;


    [SerializeField] public Transform muzzlePoint;
    [SerializeField] private float yRotationRange;

    private float interpolation; // ����ó����
    private bool isJumped; 
    private Rigidbody rb;
    private float yRotation = 0f;

    // ��Ʈ��ũ �� ���������� �ֱ����� ����
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Start()
    {
        if (photonView.IsMine == false) return;

        gun = GetComponent<Gun>();
        rb = gameObject.GetComponent<Rigidbody>();

        isJumped = false;
        interpolation = 13;

        networkPosition = transform.position;
        networkRotation = transform.rotation;

        Camera.main.transform.SetParent(gameObject.transform);
        Camera.main.transform.position = gameObject.transform.position + offset;

        CameraController cam = Camera.main.GetComponent<CameraController>();
        cam.FollowTarget = muzzlePoint;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        }
    }

    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Fire");
            gun.Fire(muzzlePoint);
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
        Vector2 rotateInput = new( rotateInput.x = Input.GetAxis("Mouse X"), rotateInput.y = Input.GetAxis("Mouse Y"));
        SetRotation(rotateInput);

        Vector3 moveInput = new( Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        SetPosition(moveInput);
    }

    private void SetRotation(Vector2 input)
    {
        if (input.x != 0)
        {
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
        if (input == Vector3.zero) return;

        Vector3 moveDirection = transform.forward * input.z + transform.right * input.x;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

}
