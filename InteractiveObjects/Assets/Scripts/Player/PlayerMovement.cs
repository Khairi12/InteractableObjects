using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float backwalkSpeed = 0.5f;
    public float diagonalSpeed = 0.725f;

    public float crouchMoveSpeed = 0.5f;
    public float crouchPromptSpeed = 1.5f;
    public float crouchHeight = 1.25f;

    public float rotateSpeed = 10.0f;
    public float rotateYMin = -60f;
    public float rotateYMax = 60f;

    private Transform cameraTransform;
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private Vector3 movementVector;
    private Vector3 forwardVelocity;
    private Vector3 horizontalVelocity;
    private Vector3 outgoingVelocity;

    private float defaultHeight = 0f;
    private float outgoingSpeed = 0f;
    private float deltaRotateX = 0f;
    private float deltaRotateY = 0f;

    private void Start()
    {
        cameraTransform = GetComponentInChildren<Camera>().GetComponent<Transform>();
        playerTransform = GetComponent<Transform>();
        playerRigidbody = GetComponent<Rigidbody>();
        outgoingSpeed = moveSpeed;
        defaultHeight = cameraTransform.position.y;
    }

    private void DiagonalWalk()
    {
        if (Input.GetAxisRaw("Vertical") != 0f && Input.GetAxisRaw("Horizontal") != 0f)
            outgoingVelocity *= diagonalSpeed;
    }

    private void BackWalk()
    {
        if (Input.GetAxisRaw("Vertical") < 0f)
            outgoingVelocity *= backwalkSpeed;
    }

    private void RaiseCrouch()
    {
        if (cameraTransform.transform.position.y < defaultHeight)
        {
            GetComponent<CapsuleCollider>().height = 2f;
            GetComponent<CapsuleCollider>().center = new Vector3(0f, 0f, 0f);
            cameraTransform.transform.Translate(Vector3.up * Time.deltaTime * crouchPromptSpeed, Space.World);
        }
    }

    private void LowerCrouch()
    {
        outgoingVelocity *= crouchMoveSpeed;

        if (cameraTransform.transform.position.y > defaultHeight - crouchHeight)
        {
            GetComponent<CapsuleCollider>().height = 1f;
            GetComponent<CapsuleCollider>().center = new Vector3(0f, -0.5f, 0f);
            cameraTransform.transform.Translate(Vector3.down * Time.deltaTime * crouchPromptSpeed, Space.World);
        }
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            LowerCrouch();
        else
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.up, out hit, 2f))
                LowerCrouch();
            else
                RaiseCrouch();
        }
    }

    private void Move()
    {
        forwardVelocity = transform.forward * Input.GetAxisRaw("Vertical");
        horizontalVelocity = transform.right * Input.GetAxisRaw("Horizontal");

        outgoingVelocity = forwardVelocity + horizontalVelocity;
        outgoingSpeed = moveSpeed;

        DiagonalWalk();
        BackWalk();
        Crouch();

        playerRigidbody.velocity = outgoingVelocity * outgoingSpeed;
    }

    private void Rotate()
    {
        deltaRotateX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        deltaRotateY -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        if (deltaRotateY < rotateYMin)
            deltaRotateY = rotateYMin;
        else if (deltaRotateY > rotateYMax)
            deltaRotateY = rotateYMax;

        playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(new Vector3(0f, deltaRotateX, 0f)));
        cameraTransform.eulerAngles = new Vector3(deltaRotateY, playerTransform.localEulerAngles.y, playerTransform.localEulerAngles.z);
    }

    private void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        Rotate();
    }
}
