using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    float xRotation;
    float yRotation;

    public Transform orientation;
    Vector2 lookInput;
    public InputActionAsset primaryInput;
    InputAction lookInputAction;


    void Awake()
    {
        lookInputAction = primaryInput.FindAction("Look");

        lookInputAction.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        lookInputAction.canceled += ctx => lookInput = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Cursor.lockState = CursorLockMode.Locked;
       Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        yRotation += lookInput.x * sensX;
        xRotation -= lookInput.y * sensY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }


    void OnEnable()
    {
        lookInputAction.Enable();
    }

    void OnDisable()
    {
        lookInputAction.Disable();
    }
}
