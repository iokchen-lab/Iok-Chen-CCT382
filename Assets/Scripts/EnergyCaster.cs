using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyCaster : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference castAction;

    [Header("Cast Animation")]
    public Animator animator;
    public string castTrigger = "Cast";
    [Min(0f)] public float releaseDelay = 0.8f;
    [Min(0f)] public float castLockDuration = 2.7f;

    [Header("Effects")]
    public ParticleSystem castVFX;

    [Header("Energy")]
    public FloatVariableSO currentEnergy;
    public FloatVariableSO maxEnergy;
    public GameEventSO onEnergyChanged;
    public GameEventSO onSpellCast;

    [Header("Recovery")]
    [Min(0f)] public float regenDelay = 1.5f;
    [Min(0f)] public float regenPerSecond = 15f;

    [Min(0f)] public float energyCost = 20f;
    [Min(0f)] public float cooldown = 0.5f;

    [Header("Hit Detection")]
    public Transform castOrigin;
    [Min(0f)] public float castRange = 2f;
    public LayerMask hitLayers = Physics.DefaultRaycastLayers;

    private Coroutine castRoutine;
    private bool isCasting;
    public bool IsCasting => isActiveAndEnabled && isCasting;
    private float nextCastTime;
    private float nextRegenTime;
    private bool enabledHere;

    private void Awake()
    {
        if (currentEnergy != null && maxEnergy != null)
            currentEnergy.value = maxEnergy.value;
    }

    private void OnEnable()
    {
        nextCastTime = 0f;
        isCasting = false;

        if (castAction == null || castAction.action == null)
            return;

        castAction.action.performed += OnCast;

        enabledHere = !castAction.action.enabled;
        if (enabledHere)
            castAction.action.Enable();
    }

    private void Update()
    {
        if (currentEnergy == null || maxEnergy == null ||
            isCasting || Time.time < nextRegenTime || regenPerSecond <= 0f)
            return;

        float maximum = Mathf.Max(0f, maxEnergy.value);
        if (currentEnergy.value >= maximum)
            return;

        currentEnergy.value = Mathf.MoveTowards(
            currentEnergy.value, maximum, regenPerSecond * Time.deltaTime);
        if (onEnergyChanged != null)
            onEnergyChanged.Raise();
    }

    private void OnDisable()
    {
        if (castRoutine != null)
        {
            StopCoroutine(castRoutine);
            castRoutine = null;
        }
        isCasting = false;

        if (castAction == null || castAction.action == null)
            return;

        castAction.action.performed -= OnCast;

        if (enabledHere)
            castAction.action.Disable();
    }

    private void OnCast(InputAction.CallbackContext context)
    {
        if (currentEnergy == null || onEnergyChanged == null)
            return;

        if (isCasting || Time.time < nextCastTime)
            return;

        if (currentEnergy.value < energyCost)
        {
            Debug.Log("Not enough energy");
            return;
        }

        // Set the lock before raising events to prevent duplicate casts.
        isCasting = true;
        currentEnergy.value -= energyCost;
        nextCastTime = Time.time + cooldown;
        nextRegenTime = Time.time + regenDelay;
        onEnergyChanged.Raise();

        if (!isActiveAndEnabled)
        {
            isCasting = false;
            return;
        }
        castRoutine = StartCoroutine(CastSequence());
    }

    private IEnumerator CastSequence()
    {
        bool playAnimation = animator != null &&
            animator.isActiveAndEnabled && !string.IsNullOrEmpty(castTrigger);
        float delay = playAnimation ? releaseDelay : 0f;
        float lockTime = playAnimation
            ? Mathf.Max(castLockDuration, delay) : 0f;
        if (playAnimation)
        {
            animator.ResetTrigger(castTrigger);
            animator.SetTrigger(castTrigger);
        }

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        // Particle, sound event and hit check occur together at release.
        if (castVFX != null)
        {
            castVFX.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
            castVFX.Play(true);
        }
        if (onSpellCast != null)
            onSpellCast.Raise();
        CheckHit();
        nextRegenTime = Time.time + regenDelay;

        float remaining = lockTime - delay;
        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);
        isCasting = false;
        castRoutine = null;
    }

    private void CheckHit()
    {
        Vector3 origin = castOrigin != null
            ? castOrigin.position
            : transform.position + Vector3.up;

        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * castRange,
            Color.magenta, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit,
            castRange, hitLayers, QueryTriggerInteraction.Ignore))
        {
            FlashOnHit target =
                hit.collider.GetComponentInParent<FlashOnHit>();

            if (target != null)
            {
                target.OnHit();
                Debug.Log($"Spell hit: {target.name}");
            }
        }
    }
}