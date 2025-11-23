using UnityEngine;

public class CometSpawner : MonoBehaviour
{
    public Comet cometPrefab;
    public float spawnInterval = 3.0f;
    public Vector2 spawnIntervalRange = new(2f, 5f);

    public float spawnDistance = 15.0f;

    private float timeSinceLastSpawn = 0.0f;
    private float currentSpawnInterval;

    private void Start()
    {
        currentSpawnInterval = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
    }

    private void Update()
    {
        if (Player.instance == null || !Player.instance.gameStarted) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= currentSpawnInterval)
        {
            SpawnComet();
            timeSinceLastSpawn = 0.0f;
            currentSpawnInterval = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
        }
    }

    private void SpawnComet()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = (Vector3)(randomDirection * spawnDistance);

        if (Player.instance != null)
        {
            spawnPosition += Player.instance.transform.position;
        }

        Comet comet = Instantiate(cometPrefab, spawnPosition, Quaternion.identity);

        HazardousItem hazardousItem = comet.GetComponent<HazardousItem>();
        float damage = hazardousItem != null ? Random.Range(hazardousItem.damageRange.x, hazardousItem.damageRange.y) : 2f;
        float speed = Random.Range(comet.speedRange.x, comet.speedRange.y);
        Vector2 direction = -randomDirection + Random.insideUnitCircle * 0.3f;

        comet.Initialize(damage, speed, direction);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        comet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
