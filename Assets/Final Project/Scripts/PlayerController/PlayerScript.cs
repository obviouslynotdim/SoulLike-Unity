using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Movement")]
    public float movementSpeed = 5f;
    public float sprintSpeedMultiplier = 1.8f; // Multiplier for base speed when sprinting
    public float rotSpeed = 600f;
    public MainCameraController MCC;
    Quaternion requireRotation;
    private float currentSpeed; // Holds either base speed or sprint speed

    [Header("Player Actions")]
    public float jumpForce = 8f;        // Initial upward velocity for jump
    public float rollDuration = 0.5f;   // Time the roll action lasts
    public float rollSpeed = 6f;        // Speed during the roll
    private bool isRolling = false;     // Flag to lock player input during roll

    [Header("Player Animator")]
    public Animator animator;

    [Header("Player Collision & Gravity")]
    public CharacterController CC;
    public float surfaceCheckRadius = 0.1f;
    public Vector3 surfaceCheckOffset;
    public LayerMask surfaceLayer;
    bool onSurface;

    [SerializeField] float fallingSpeed;
    [SerializeField] Vector3 moveDir;

    private void Update()
    {
        // groundcheck status
        SurfaceCheck();

        if (onSurface)
        {
            if (fallingSpeed < -0.5f)
            {
                fallingSpeed = -0.5f;
            }

            // jump check
            if (Input.GetButtonDown("Jump") && !isRolling)
            {
                fallingSpeed = jumpForce;
                onSurface = false;
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            fallingSpeed += Physics.gravity.y * Time.deltaTime;
        }

        PlayerMovement();

        var velocity = moveDir * currentSpeed;
        velocity.y = fallingSpeed;

        if (!isRolling)
        {
            CC.Move(velocity * Time.deltaTime);
        }

        //Debug.Log("Player on Surface" + onSurface);
    }

    void PlayerMovement ()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float movementAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        var movementInput = (new Vector3(horizontal, 0, vertical)).normalized;
        var movementDirection = MCC.flatRotation * movementInput;

        // --- ROLL CHECK (Q Key) ---
        if (Input.GetKeyDown(KeyCode.Q) && !isRolling)
        {
            Vector3 directionToRoll;

            if (movementAmount > 0)
            {
                // Rolling while moving: use the calculated movement direction
                directionToRoll = movementDirection;
            }
            else
            {
                // Rolling while idle: use the character's forward direction
                directionToRoll = transform.forward;
            }

            StartCoroutine(RollAction(directionToRoll));
            return; // Exit to prevent normal movement/rotation/speed calculation
        }

        if (isRolling) return; // Ignore all input/movement while rolling

        // --- SPRINT LOGIC (Left Shift) ---
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = movementSpeed * (isSprinting ? sprintSpeedMultiplier : 1f);

        moveDir = movementDirection;

        if (movementAmount > 0)
        {
            requireRotation = Quaternion.LookRotation(movementDirection);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, requireRotation, rotSpeed * Time.deltaTime);

        animator.SetFloat("movementValue", movementAmount, 0.2f, Time.deltaTime);
    }

    void SurfaceCheck()
    {
        onSurface = Physics.CheckSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius, surfaceLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(surfaceCheckOffset), surfaceCheckRadius);
    }

    IEnumerator RollAction(Vector3 direction)
    {
        isRolling = true;

        // Lock player rotation to the roll direction immediately
        transform.rotation = Quaternion.LookRotation(direction);

        // Trigger the roll animation (requires a Trigger parameter named "Roll" in the Animator)
        animator.SetTrigger("Roll");

        float startTime = Time.time;

        while (Time.time < startTime + rollDuration)
        {
            // Apply roll force independently, ignoring gravity/jump during the roll
            Vector3 rollVelocity = new Vector3(direction.x, 0, direction.z) * rollSpeed * Time.deltaTime;
            CC.Move(rollVelocity);
            yield return null;
        }

        isRolling = false;
        // Reset moveDir to prevent ghost movement immediately after the roll ends
        moveDir = Vector3.zero;
    }
}
