using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement")]
    public InputActionAsset primaryInput;
    InputAction moveInputAction;
    InputAction jumpInputAction;
    public Transform orientation;
    public int speed = 0;
    Vector2 direction;
    Vector3 moveDirection;
    public float groundDrag;

    [Header ("Drag")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public int jumpHeight;
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
    }

    void Update()
    {
        if (isGrounded())
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    bool isGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        grounded = Physics.Raycast(origin, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded) 
            return true;
        
        return false;
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (!isGrounded() && rb.linearVelocity.y < 0)
            rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * direction.y + orientation.right * direction.x;

        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
    }

    void JumpAction(InputAction.CallbackContext ctx)
    {
        if (isGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }
    }

    void OnEnable()
    {
        moveInputAction.Enable();
    }

    void OnDisable()
    {
        moveInputAction.Disable();
    }
}
