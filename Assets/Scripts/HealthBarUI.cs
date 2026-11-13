using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Data")]
    public FloatVariableSO currentHealth;
    public FloatVariableSO maxHealth;
    public GameEventSO onHealthChanged;
    public GameEventSO onSpellCast;

    [Header("UI")]
    public Slider slider;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip changeSound;

    private void OnEnable()
    {
        if (onHealthChanged != null)
            onHealthChanged.RegisterListener(OnHealthChangedEvent);

        if (onSpellCast != null)
            onSpellCast.RegisterListener(PlayCastSound);

        SyncSlider();
    }

    private void Start()
    {
        SyncSlider();
    }

    private void OnDisable()
    {
        if (onHealthChanged != null)
            onHealthChanged.UnregisterListener(OnHealthChangedEvent);

        if (onSpellCast != null)
            onSpellCast.UnregisterListener(PlayCastSound);
    }

    [ContextMenu("Test Energy Changed")]
    
    private void OnHealthChangedEvent()
    {
        SyncSlider();
    }

    private void PlayCastSound()
    {
        if (audioSource == null || changeSound == null)
            return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(changeSound);
    }

    private void SyncSlider()
    {
        if (slider == null || currentHealth == null ||
            maxHealth == null || maxHealth.value <= 0f)
            return;

        slider.value = Mathf.Clamp01(
            currentHealth.value / maxHealth.value);
    }
}