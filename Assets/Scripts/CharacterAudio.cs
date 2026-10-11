using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class CharacterAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip landingClip;

    [Header("Footstep Settings")]
    [SerializeField] private float minimumMoveSpeed = 0.2f;
    [SerializeField] private float runSpeedThreshold = 3f;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.28f;
    [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.6f;

    [Header("Landing Settings")]
    [SerializeField] private float minimumAirTime = 0.15f;
    [SerializeField, Range(0f, 1f)] private float landingVolume = 0.8f;

    private CharacterController characterController;
    private AudioSource audioSource;

    private float stepTimer;
    private float airTime;
    private bool wasGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        wasGrounded = characterController.isGrounded;
    }

    private void Update()
    {
        bool isGrounded = characterController.isGrounded;

        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        float horizontalSpeed = horizontalVelocity.magnitude;

        HandleFootsteps(isGrounded, horizontalSpeed);
        HandleLanding(isGrounded);

        wasGrounded = isGrounded;
    }

    private void HandleFootsteps(bool isGrounded, float horizontalSpeed)
    {
        if (!isGrounded || horizontalSpeed < minimumMoveSpeed)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlayFootstep();

            stepTimer = horizontalSpeed >= runSpeedThreshold
                ? runStepInterval
                : walkStepInterval;
        }
    }

    private void HandleLanding(bool isGrounded)
    {
        if (!isGrounded)
        {
            airTime += Time.deltaTime;
            return;
        }

        if (!wasGrounded && airTime >= minimumAirTime)
        {
            PlayLanding();
        }

        airTime = 0f;
    }

    private void PlayFootstep()
    {
        if (footstepClip == null)
            return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(footstepClip, footstepVolume);
    }

    private void PlayLanding()
    {
        if (landingClip == null)
            return;

        audioSource.pitch = 1f;
        audioSource.PlayOneShot(landingClip, landingVolume);
    }
}