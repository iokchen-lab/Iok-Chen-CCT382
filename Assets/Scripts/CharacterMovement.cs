using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovementInput))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float crouchIdleHeight = 1.2f;
    [SerializeField] private float crouchWalkHeight = 1.4f;
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private PlayerMovementInput movementInput;
    private Animator animator;
    private float standingHeight;
    private Vector3 standingCenter;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        movementInput = GetComponent<PlayerMovementInput>();
        animator = GetComponent<Animator>();
        standingHeight = controller.height;
        standingCenter = controller.center;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        Vector2 input = movementInput.MoveInput;

        Vector3 movement;

        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            movement =
                cameraForward * input.y +
                cameraRight * input.x;
        }
        else
        {
            movement = new Vector3(input.x, 0f, input.y);
        }

        bool mustRemainCrouched =
            controller.isGrounded &&
            controller.height < standingHeight - 0.01f &&
            !CanStandUp();

        bool isCrouching =
            controller.isGrounded &&
            (movementInput.IsCrouching || mustRemainCrouched);

        animator.SetBool("Crouch", isCrouching);


        if (isCrouching)
        {
            bool isCrouchMoving = movement.sqrMagnitude > 0.01f;

            float currentCrouchHeight = isCrouchMoving
                ? crouchWalkHeight
                : crouchIdleHeight;

            float standingBottom =
                standingCenter.y - standingHeight * 0.5f;

            controller.height = currentCrouchHeight;
            controller.center = new Vector3(
                standingCenter.x,
                standingBottom + currentCrouchHeight * 0.5f,
                standingCenter.z
            );
        }
        else
        {
            controller.height = standingHeight;
           controller.center = standingCenter;
        }

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        if (movement.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (movementInput.JumpPressedThisFrame && !isCrouching)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("Jump");
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        float currentSpeed;

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = movementInput.IsSprinting ? runSpeed : walkSpeed;
        }

        Vector3 velocity = movement * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private bool CanStandUp()
    {
        float scaleY = Mathf.Abs(transform.lossyScale.y);

        float radius =
            controller.radius *
            Mathf.Max(
                Mathf.Abs(transform.lossyScale.x),
                Mathf.Abs(transform.lossyScale.z)
            ) *
            0.95f;

        float currentHeight = controller.height * scaleY;
        float targetHeight = standingHeight * scaleY;

        Vector3 capsuleCenter =
        transform.TransformPoint(controller.center);

        Vector3 castOrigin =
            capsuleCenter +
            transform.up * (currentHeight * 0.5f - radius);

        float castDistance =
            targetHeight - currentHeight + 0.05f;

        // 忽略角色自己的 Player Layer，只检测环境。
        int collisionMask = ~(1 << gameObject.layer);

        bool ceilingDetected = Physics.SphereCast(
            castOrigin,
            radius,
            transform.up,
            out RaycastHit hit,
            castDistance,
            collisionMask,
            QueryTriggerInteraction.Ignore
        );

        return !ceilingDetected;
    }

}
