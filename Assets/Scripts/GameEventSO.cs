using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(
    fileName = "NewGameEvent",
    menuName = "Game Events/Game Event")]
public class GameEventSO : ScriptableObject
{
    // 保留旧血量系统，避免已有脚本报错。
    public float currentHealth = 100f;

    private event UnityAction listeners;

    public void Raise()
    {
        listeners?.Invoke();
    }

    public void RegisterListener(UnityAction listener)
    {
        listeners -= listener;
        listeners += listener;
    }

    public void UnregisterListener(UnityAction listener)
    {
        listeners -= listener;
    }

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