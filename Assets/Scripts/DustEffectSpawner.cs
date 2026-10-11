using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DustEffectSpawner : MonoBehaviour
{
    [Header("Dust Effect")]
    [SerializeField] private GameObject dustPrefab;
    [SerializeField] private Vector3 footOffset = new Vector3(0f, 0.05f, 0f);

    [Header("Speed Detection")]
    [SerializeField] private float speedThreshold = 1f;
    [SerializeField] private float walkSpawnInterval = 0.55f;
    [SerializeField] private float runSpawnInterval = 0.2f;
    [SerializeField] private float runSpeedThreshold = 3f;

    private CharacterController characterController;
    private float nextSpawnTime;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;

        float currentSpeed = horizontalVelocity.magnitude;

        if (characterController.isGrounded &&
            currentSpeed > speedThreshold &&
            Time.time >= nextSpawnTime)
        {
            SpawnDust();
            float currentInterval =
                currentSpeed >= runSpeedThreshold
                    ? runSpawnInterval
                    : walkSpawnInterval;

            nextSpawnTime = Time.time + currentInterval;
        }
    }

    private void SpawnDust()
    {
        Vector3 spawnPosition = transform.position + footOffset;

        GameObject dust = Instantiate(
            dustPrefab,
            spawnPosition,
            dustPrefab.transform.rotation
        );

        Destroy(dust, 2f);
    }
}