using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement")]
    public InputActionAsset primaryInput;
    public TextMeshProUGUI speedText;
    InputAction moveInputAction;
    InputAction jumpInputAction;
    public Transform orientation;
    public int gravityMultiplier = 0;
    public int speed = 0;
    public int speedLimit = 0;
    Vector2 direction;
    Vector3 moveDirection;
    public float groundDrag;

    [Header ("Drag")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public int jumpForce;
    bool grounded;

    Rigidbody rb;

    void Awake()
    {
        moveInputAction = primaryInput.FindAction("Move");
        jumpInputAction = primaryInput.FindAction("Jump");

        moveInputAction.performed += ctx => direction = ctx.ReadValue<Vector2>();
        moveInputAction.canceled += ctx => direction = Vector2.zero;

        jumpInputAction.performed += JumpAction;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = speedLimit;
    }

    void Update()
    {
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        speedText.SetText($"{rb.linearVelocity.magnitude}");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (!grounded && rb.linearVelocity.y < 0)
            rb.AddForce(Vector3.down * 20f * gravityMultiplier, ForceMode.Acceleration);
    }

    private void MovePlayer()
    {
        Vector3 flatForward = orientation.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        moveDirection = flatForward * direction.y + orientation.right * direction.x;

        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
    }

    void JumpAction(InputAction.CallbackContext ctx)
    {
        if (grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnEnable()
    {
        moveInputAction.Enable();
        jumpInputAction.Enable();
    }

    void OnDisable()
    {
        moveInputAction.Disable();
        jumpInputAction.Disable();
    }
}
