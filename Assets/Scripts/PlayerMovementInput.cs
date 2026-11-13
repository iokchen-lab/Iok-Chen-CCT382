using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// LAB 3 - GOAL 2 & GOAL 3
///
/// You don't need to know C# to finish this script. Read the plain-language
/// comments above each section, then fill in the ONE blank near the bottom
/// marked "TODO" - that's the only line you need to write yourself.
///
/// What this script does, in order, every time the game runs:
///   1. Find the "Move" action inside PlayerControls.inputactions.
///   2. Turn that action on so it starts listening for WASD / left stick.
///   3. Every single frame, ask it "what's the current input?" and store
///      the answer.
///   4. Hand that answer to the Animator so your 2D Freeform Directional
///      blend tree (Goal 3) knows which way to blend.
///
/// Setup reminder: this script goes on your character, alongside its
/// Animator component, and you drag PlayerControls.inputactions into the
/// "Input Actions" slot in the Inspector.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerMovementInput : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag PlayerControls.inputactions here.")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string sprintActionName = "Sprint";
    [SerializeField] private string jumpActionName = "Jump";
    [SerializeField] private string crouchActionName = "Crouch";

    [Header("Animator Parameters")]
    // These two names must exactly match the float parameters you created
    // inside the Animator (Goal 3). Capitalization matters in C# - "movex"
    // and "MoveX" are treated as two different names.
    [SerializeField] private string moveXParam = "MoveX";
    [SerializeField] private string moveYParam = "MoveY";

    [Header("Smoothing")]
    // This one number IS a response curve, in the sense from this week's
    // reading. A small number = fast Attack = tight, snappy blending.
    // A bigger number = slow Attack = floatier, smoother blending.
    // Try changing this value and see how differently the blend feels.
    [SerializeField] private float dampTime = 0.1f;

    [Header("Casting Movement Lock")]
    [SerializeField] private EnergyCaster energyCaster;

    private Vector2 rawMoveInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    [SerializeField] private Animator animator;

    /// <summary>
    /// The current input direction, updated every frame. You won't need to
    /// touch this yourself today, but this is what a future movement script
    /// would read to actually move the character (not just animate it).
    /// </summary>
    public Vector2 MoveInput
    {
        get => energyCaster != null && energyCaster.IsCasting
            ? Vector2.zero : rawMoveInput;
        private set => rawMoveInput = value;
    }
    public bool IsSprinting { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }
    public bool IsCrouching { get; private set; }

    // ------------------------------------------------------------------
    // Awake() runs ONCE, right when the game starts - before anything else.
    // This block looks inside the Input Actions asset you dragged in, finds
    // the "Player" map, and finds the "Move" action inside it. Think of it
    // as Unity double-checking everything is wired up correctly and giving
    // you a clear error message in the Console if it isn't.
    // You don't need to write anything in this block.
    // ------------------------------------------------------------------
    private void Awake()
    {
        if (energyCaster == null)
            energyCaster = GetComponent<EnergyCaster>();
        if (animator == null)
    {
        animator = GetComponent<Animator>();
    }

        if (inputActions == null)
        {
            Debug.LogError($"{name}: No InputActionAsset assigned to PlayerMovementInput. " +
                            "Drag PlayerControls.inputactions into the Input Actions field.");
            return;
        }

        var map = inputActions.FindActionMap(actionMapName, throwIfNotFound: false);
        if (map == null)
        {
            Debug.LogError($"{name}: Action map \"{actionMapName}\" not found in {inputActions.name}.");
            return;
        }

        moveAction = map.FindAction(moveActionName, throwIfNotFound: false);
        if (moveAction == null)
        {
            Debug.LogError($"{name}: Action \"{moveActionName}\" not found in map \"{actionMapName}\".");
        }

        sprintAction = map.FindAction(sprintActionName, throwIfNotFound: false);
        if (sprintAction == null)
        {
            Debug.LogError($"{name}: Action \"{sprintActionName}\" not found in map \"{actionMapName}\".");
        }

        jumpAction = map.FindAction(jumpActionName, throwIfNotFound: false);
        if (jumpAction == null)
        {
            Debug.LogError($"{name}: Action \"{jumpActionName}\" not found in map \"{actionMapName}\".");
        }

        crouchAction = map.FindAction(crouchActionName, throwIfNotFound: false);
        if (crouchAction == null)
        {
            Debug.LogError($"{name}: Action \"{crouchActionName}\" not found in map \"{actionMapName}\".");
        }
    }

    // ------------------------------------------------------------------
    // OnEnable / OnDisable turn the action's "listening" on and off,
    // matching whether this GameObject is active in the scene.
    //
    // This is the single most common thing people forget when using the
    // Input System - if you skip this, moveAction never wakes up and
    // your input will silently do nothing. It's the #1 pitfall from the
    // deck ("Actions never fire") for a reason.
    // You don't need to write anything in this block.
    // ------------------------------------------------------------------
    private void OnEnable()
    {
        moveAction?.Enable();
        sprintAction?.Enable();
        jumpAction?.Enable();
        crouchAction?.Enable();
    }
    private void OnDisable()
    {
        moveAction?.Disable();
        sprintAction?.Disable();
        jumpAction?.Disable();
        crouchAction?.Disable();
    }

    // ------------------------------------------------------------------
    // Update() runs once per frame - potentially dozens or hundreds of
    // times per second. This is where the "click" from the reading
    // actually gets read and turned into something the game can use.
    // ------------------------------------------------------------------
    private void Update()
    {
        // Ask the Move action: "what's the current WASD / left-stick value
        // right now?" It comes back as a Vector2: x is left/right, y is
        // up/down. (0,0) means no input at all.
        MoveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        JumpPressedThisFrame =
            jumpAction != null && jumpAction.WasPressedThisFrame();
        IsCrouching = crouchAction != null && crouchAction.IsPressed();

        // ------------------------------------------------------------
        // TODO (this is the one part you write):
        // Feed MoveInput into the Animator so the blend tree from Goal 3
        // can use it. You need TWO lines - one for MoveX, one for MoveY.
        //
        // The method you want is:
        //     animator.SetFloat(parameterName, value, dampTime, Time.deltaTime);
        //
        // Hints:
        //   - The parameter names are already stored for you above:
        //     moveXParam and moveYParam.
        //   - The values you want to send are MoveInput.x and MoveInput.y.
        //   - dampTime is already declared above too - just reuse it.
        //
        // Write your two lines below this comment:
        bool isMoving = MoveInput.sqrMagnitude > 0.01f;
        IsSprinting = isMoving && sprintAction != null && sprintAction.IsPressed();
        float moveAmount = isMoving ? (IsSprinting ? 1f : 0.5f) : 0f;

        animator.SetFloat(moveXParam, 0f, dampTime, Time.deltaTime);
        animator.SetFloat(moveYParam, moveAmount, dampTime, Time.deltaTime);


        // ------------------------------------------------------------
    }
}
