using UnityEngine;
using UnityEngine.Events;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private GameEventSO healthData;
    [SerializeField] private float damageAmount = 10f;

    public UnityEvent onPlayerEntered = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() == null)
            return;

        if (healthData == null)
        {
            Debug.LogWarning("Please assign Health Data.", this);
            return;
        }

        healthData.Subtract(damageAmount);

        Debug.Log("Player entered trigger. UnityEvent fired.", this);
        onPlayerEntered.Invoke();
    }
}