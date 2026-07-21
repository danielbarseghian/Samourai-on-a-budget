using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement")]
    public InputActionAsset primaryInput;
    InputAction moveInputAction;
    public Transform orientation;
    public int speed = 0;
    Vector2 direction;
    Vector3 moveDirection;
    public float groundDrag;

    [Header ("Drag")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    Rigidbody rb;

    void Awake()
    {
        moveInputAction = primaryInput.FindAction("Move");

        moveInputAction.performed += ctx => direction = ctx.ReadValue<Vector2>();
        moveInputAction.canceled += ctx => direction = Vector2.zero;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        grounded = Physics.Raycast(origin, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        Debug.DrawRay(origin, Vector3.down, Color.red, playerHeight * .5f + .2f);

        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
            rb.linearDamping = 0;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * direction.y + orientation.right * direction.x;

        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
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
