using UnityEngine;
using System.Collections;

/// <summary>
/// Lab 7 - Goal 3.
///
/// Smoothly animates a Shader Graph's exposed _FlashAmount property from
/// 1 (fully flashed) back to 0 (normal) over flashDuration, using a
/// coroutine so the change happens gradually across several frames
/// instead of snapping instantly.
///
/// Setup:
///   1. Build the unlit Hit Flash (or Dissolve) Shader Graph from the lab,
///      with a Float property named _FlashAmount, Exposed checked.
///   2. Assign that shader's Material to this object's Renderer.
///   3. Drag this object's own Renderer into the Target Renderer field.
///   4. Call OnHit() from wherever your interaction event fires - e.g.
///      MeleeAttackDemo's foreach loop, calling hit.GetComponent
///      <FlashOnHit>()?.OnHit() instead of (or alongside) DemoTarget.Hit().
/// </summary>
public class FlashOnHit : MonoBehaviour
{
    [Header("Shader Link")]
    [Tooltip("The Renderer whose material has the exposed _FlashAmount property.")]
    public Renderer targetRenderer;

    [Tooltip("The exact name of the exposed Float property in the Shader Graph.")]
    public string propertyName = "_FlashAmount";

    [Header("Timing")]
    [Tooltip("How long the flash takes to fade back to 0. This is a response curve, the same idea as Week 3's Attack/Decay.")]
    public float flashDuration = 0.2f;

    private Coroutine activeFlash;

    /// <summary>
    /// Call this when the object is hit. Safe to call repeatedly - a new
    /// hit restarts the flash from full strength rather than stacking.
    /// </summary>
    public void OnHit()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning($"{name}: No Target Renderer assigned on FlashOnHit.");
            return;
        }

        if (activeFlash != null)
        {
            StopCoroutine(activeFlash);
        }
        activeFlash = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float amount = 1f - (t / flashDuration);
            targetRenderer.material.SetFloat(propertyName, amount);
            yield return null;
        }

        targetRenderer.material.SetFloat(propertyName, 0f);
        activeFlash = null;
    }
}
