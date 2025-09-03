using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Values")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float decelerationSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float sprintMultiplier;
    private Vector3 velocity;
    private CharacterController controller;

    [Header("Player Camera Values")]
    [SerializeField, Range(0.1f, 9f)] private float cameraSensitivity;
    [SerializeField] private float cameraYRotationLimit;
    Vector2 cameraRotation = Vector2.zero;



    [Header("Input")]
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        lookAction.action.Enable();
        moveAction.action.Enable();
        sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
        moveAction.action.Disable();
        sprintAction.action.Disable();
    }

    private void Update()
    {
        CameraMovement();
        PlayerMovement();
    }

    private void CameraMovement() 
    {
        cameraRotation += lookAction.action.ReadValue<Vector2>() * cameraSensitivity;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -cameraYRotationLimit, cameraYRotationLimit);
        var cameraXQuaternion = Quaternion.AngleAxis(cameraRotation.x, Vector3.up);
        var cameraYQuaternion = Quaternion.AngleAxis(cameraRotation.y, Vector3.left);

        Camera.main.transform.localRotation = cameraXQuaternion * cameraYQuaternion;
    }

    private void PlayerMovement()
    {
        Vector2 movementInput = moveAction.action.ReadValue<Vector2>();
        Vector3 movementVector = new Vector3(movementInput.x, 0, movementInput.y);
        movementVector = Vector3.ClampMagnitude(movementVector, 1f);

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 flattened = Vector3.ProjectOnPlane(cameraForward, Vector3.up);
        Quaternion cameraOrientation = Quaternion.LookRotation(flattened);

        movementVector = cameraOrientation * movementVector;

        float sprintInput = sprintAction.action.ReadValue<float>();

        if (sprintInput > 0.5f)
        {
            Debug.Log(moveSpeed * sprintMultiplier);
            velocity = movementVector * (moveSpeed * sprintMultiplier);
        }
        else
        {
            velocity = movementVector * moveSpeed;
        }

        float velocityAdjustmentSpeed = 1;

        if (velocity.magnitude > controller.velocity.magnitude)
        {
            velocityAdjustmentSpeed = accelerationSpeed;
        }
        else
        {
            velocityAdjustmentSpeed = decelerationSpeed;
        }

        Vector3 currentVelocity = Vector3.Lerp(controller.velocity, velocity, velocityAdjustmentSpeed * Time.deltaTime);
        currentVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        controller.Move(currentVelocity * Time.deltaTime);
    }
}
