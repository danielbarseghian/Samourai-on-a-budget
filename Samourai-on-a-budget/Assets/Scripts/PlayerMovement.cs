using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
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
    public LayerMask groundMask;
    public float groundCheckDistance = .5f;
    public int jumpForce;
    private PlayerState state;
    private bool canSprint;

    [Header("Drag")]
    public float playerHeight;
    Rigidbody rb;

    [Header("Crouch")]
    InputAction crouchInputAction;
    public float crouchSpeed = 2.5f;
    private float crouchSize;
    private float baseSize;
    
    [Header("Slop Handeling")]
    public float maxSlopAngle = 40;
    private RaycastHit slopehit;

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

        bool grounded = IsGrounded();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        speedText.SetText($"{rb.linearVelocity.magnitude}");

        if (grounded && state != PlayerState.sprinting && state != PlayerState.crouching)
        {
            state = PlayerState.walking;
        }
        if (!grounded)
            state = PlayerState.air;

        // turn off gravity if we are on a slop
        rb.useGravity = !OnSlop();
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

        if (OnSlop())
        {
            rb.AddForce(GetMoveDirection() * moveSpeed, ForceMode.Force);

            // Add force down so we don't get weird movment
            rb.AddForce(Vector3.down * 80f, ForceMode.Impulse);
        }

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
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void SprintAction(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
        {
            state = PlayerState.sprinting;
        }
    }

    private void UnSprintAction(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
        {
            state = PlayerState.walking;
        }
    }

    private void CrouchAction(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
        {
            // Change the scale
            transform.localScale = new Vector3(transform.localScale.x, crouchSize, transform.localScale.z);

            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            state = PlayerState.crouching;
        }
    }

    private void UnCrouchAction(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
        {
            state = PlayerState.walking;

            transform.localScale = new Vector3(transform.localScale.x, baseSize, transform.localScale.z);
        }
        
    }

    private bool OnSlop()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopehit, playerHeight * .5f * .3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopehit.normal);
            return angle < maxSlopAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopehit.normal).normalized;
    }

    bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);

        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance + playerHeight / 2,
            groundMask
        );
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
