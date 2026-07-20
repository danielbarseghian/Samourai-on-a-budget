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
        Debug.Log("LOOK");
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
        Debug.Log($"movinputx : {lookInput.x}");
        Debug.Log($"Move input y: {lookInput.y}");

        yRotation += lookInput.x * sensX;
        xRotation -= lookInput.y * sensY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
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
