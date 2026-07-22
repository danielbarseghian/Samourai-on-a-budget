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
    InputAction sprintInputAction;
    public Transform orientation;
    public int gravityMultiplier = 0;
    public float speed = 0;
    public int speedLimit = 0;
    Vector2 direction;
    Vector3 moveDirection;
    public float groundDrag;
    public int jumpForce;
    public float sprintMultiplier = 1.2f;

    [Header ("Drag")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    Rigidbody rb;

    void Awake()
    {
        moveInputAction = primaryInput.FindAction("Move");
        jumpInputAction = primaryInput.FindAction("Jump");
        sprintInputAction = primaryInput.FindAction("Sprint");

        moveInputAction.performed += ctx => direction = ctx.ReadValue<Vector2>();
        moveInputAction.canceled += ctx => direction = Vector2.zero;

        jumpInputAction.performed += JumpAction;

        sprintInputAction.performed += SprintAction;
        sprintInputAction.canceled += UnSprintAction;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityMultiplier;
    }

    void Update()
    {
        rb.maxLinearVelocity = speedLimit;

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        speedText.SetText($"{rb.linearVelocity.magnitude}");
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 flatForward = orientation.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        moveDirection = flatForward * direction.y + orientation.right * direction.x;

        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
    }

    private void JumpAction(InputAction.CallbackContext ctx)
    {
        if (grounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void SprintAction(InputAction.CallbackContext ctx)
    {
        speed *= sprintMultiplier;
    }

    private void UnSprintAction(InputAction.CallbackContext ctx)
    {
        speed /= sprintMultiplier;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    void OnEnable()
    {
        moveInputAction.Enable();
        jumpInputAction.Enable();
        sprintInputAction.Enable();
    }

    void OnDisable()
    {
        moveInputAction.Disable();
        jumpInputAction.Disable();
        sprintInputAction.Disable();
    }
}
