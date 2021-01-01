using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;

    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] [Range(0.0f, 0.05f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.05f)] float mouseSmoothTime = 0.03f;

    [SerializeField] bool lockCursor = true;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    
    CharacterController playerController = null;

    Vector2 currentDirection = Vector2.zero;
    Vector2 currentDirectionVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<CharacterController>();

        // Lock the cursor in middle of the screen
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook()
    {
        // Get the mouse input
        Vector2 targetMouseDelta = new Vector2 (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Smooth the mouse movement
        currentMouseDelta = Vector2.SmoothDamp (currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        // invert the Y input of the mose for accurate looking up and down
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;

        // Lock the camera pitch so it does not rotate 360 degerees on the Y
        cameraPitch = Mathf.Clamp (cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        // Rotate the camera
        transform.Rotate (Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        // Get the direction input
        Vector2 targetDirection = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();

        // Smooth the walking
        currentDirection = Vector2.SmoothDamp (currentDirection, targetDirection, ref currentDirectionVelocity, moveSmoothTime);

        if (playerController.isGrounded)
            velocityY = 0.0f;

        velocityY += gravity * Time.deltaTime;

        // Apply speed and gravity
        Vector3 velocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x) * walkSpeed + Vector3.up * velocityY;

        playerController.Move (velocity * Time.deltaTime);
    }
}
