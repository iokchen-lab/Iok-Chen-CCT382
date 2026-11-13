using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform interactionOrigin;
    public float maxDistance = 2f;
    public LayerMask targetLayer;

    [Header("Live Feedback (optional)")]
    public Renderer feedbackRenderer;
    public Color noHitColor = Color.white;
    public Color hitColor = Color.red;

    private RaycastHit hitInfo;
    private bool didHit;
    private Collider previousTarget;

    private void Update()
    {
        if (interactionOrigin == null)
            return;

        didHit = Physics.Raycast(
            interactionOrigin.position,
            interactionOrigin.forward,
            out hitInfo,
            maxDistance,
            targetLayer
        );

        if (feedbackRenderer != null)
        {
            feedbackRenderer.material.color =
                didHit ? hitColor : noHitColor;
        }

        Collider currentTarget = didHit ? hitInfo.collider : null;

        if (currentTarget != null && currentTarget != previousTarget)
        {
            Debug.Log(
                $"Target hit: {currentTarget.gameObject.name}",
                currentTarget.gameObject
            );
            currentTarget.GetComponent<FlashOnHit>()?.OnHit();
        }

        previousTarget = currentTarget;
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionOrigin == null)
            return;

        Gizmos.color = didHit ? Color.red : Color.green;

        Vector3 endPoint = didHit
            ? hitInfo.point
            : interactionOrigin.position +
              interactionOrigin.forward * maxDistance;

        Gizmos.DrawLine(interactionOrigin.position, endPoint);

        if (didHit)
        {
            Gizmos.DrawWireSphere(hitInfo.point, 0.15f);
        }
    }
}