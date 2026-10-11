using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PlayerHealth", menuName = "Game/Health Data")]
public class GameEventSO : ScriptableObject
{
    public float currentHealth = 100f;

    public void Subtract(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Debug.Log($"Health updated: {currentHealth}");
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= HandlePlayModeChange;
        EditorApplication.playModeStateChanged += HandlePlayModeChange;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= HandlePlayModeChange;
    }

    private void HandlePlayModeChange(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode ||
            state == PlayModeStateChange.EnteredEditMode)
        {
            currentHealth = 100f;
        }
    }
#endif
}