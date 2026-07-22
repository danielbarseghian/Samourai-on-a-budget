using NUnit.Framework;
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
    private float moveSpeed = 0;
    public float walkSpeed = 12;
    public float sprintSpeed = 15;
    public float airSpeed = 17;
    public int speedLimit = 0;
    Vector2 direction;
    Vector3 moveDirection;
    public float groundDrag;
    public int jumpForce;
    private PlayerState state;

    [Header ("Drag")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    Rigidbody rb;

    [Header ("Crouch")]
    InputAction crouchInputAction;
    public float crouchSpeed = 2.5f;
    private float crouchSize;
    private float baseSize;

    public enum PlayerState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    void Awake()
    {
        moveInputAction = primaryInput.FindAction("Move");
        jumpInputAction = primaryInput.FindAction("Jump");
        sprintInputAction = primaryInput.FindAction("Sprint");
        crouchInputAction = primaryInput.FindAction("Crouch");

        moveInputAction.performed += ctx => direction = ctx.ReadValue<Vector2>();
        moveInputAction.canceled += ctx => direction = Vector2.zero;

        jumpInputAction.performed += JumpAction;

        sprintInputAction.performed += SprintAction;
        sprintInputAction.canceled += UnSprintAction;

        crouchInputAction.performed += CrouchAction;
        crouchInputAction.canceled += UnCrouchAction;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityMultiplier;
        baseSize = transform.localScale.y;
        crouchSize = baseSize / 2;
    }

    void Update()
    {
        HandleState();

        rb.maxLinearVelocity = speedLimit;

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        speedText.SetText($"{rb.linearVelocity.magnitude}");

        Debug.Log($"{moveSpeed}");

        if (grounded && state != PlayerState.sprinting && state != PlayerState.crouching)
        {
            state = PlayerState.walking;
        }
        if (!grounded)
            state = PlayerState.air;
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

        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
    }

    private void HandleState()
    {
        if (state == PlayerState.walking)
            moveSpeed = walkSpeed;

        else if (state == PlayerState.sprinting)
            moveSpeed = sprintSpeed;

        else if (state == PlayerState.crouching)
            moveSpeed = crouchSpeed;

        else // He is in the air
            moveSpeed = airSpeed;

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
        if (grounded)
        {
            state = PlayerState.sprinting;
        }
    }

    private void UnSprintAction(InputAction.CallbackContext ctx)
    {
        if (grounded)
        {
            state = PlayerState.walking;
        }
    }

    private void CrouchAction(InputAction.CallbackContext ctx)
    {
        if (grounded)
        {
            // Change the scale
            transform.localScale = new Vector3(transform.localScale.x, crouchSize, transform.localScale.z);

            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            state = PlayerState.crouching;
        }
    }

    private void UnCrouchAction(InputAction.CallbackContext ctx)
    {
        if (grounded)
        {
            state = PlayerState.walking;

            transform.localScale = new Vector3(transform.localScale.x, baseSize, transform.localScale.z);
        }
        
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
        crouchInputAction.Enable();
    }

    void OnDisable()
    {
        moveInputAction.Disable();
        jumpInputAction.Disable();
        sprintInputAction.Disable();
        crouchInputAction.Disable();
    }
}
