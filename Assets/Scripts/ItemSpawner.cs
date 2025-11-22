using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject itemPrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnRadius = 20f;
    public float despawnRadius = 30f;
    public int itemsPerChunk = 5;
    public float chunkSize = 15f;

    [Header("Item ranges")]
    public Vector2 scaleRange = new Vector2(0.5f, 2f);
    public Vector2 weightRange = new Vector2(0.1f, 5f);
    public Vector2 valueRange = new Vector2(0f, 100f);

    // Track Spawned Items and Chunks
    private Dictionary<Vector2Int, List<GameObject>> spawnedChunks = new Dictionary<Vector2Int, List<GameObject>>();
    private HashSet<Vector2Int> activeChunkCoords = new HashSet<Vector2Int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if(player == null)
                Debug.LogError("Player not found in the scene");
        }

        if(player != null)
            UpdateSpawnedChunks();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        UpdateSpawnedChunks();
        
    }

    void UpdateSpawnedChunks()
    {
        Vector2Int currChunk = GetChunkCoord(player.position);

        HashSet<Vector2Int> chunksToLoad = new HashSet<Vector2Int>();
        int chunkRadius = Mathf.CeilToInt(spawnRadius / chunkSize);

        for(int x = -chunkRadius; x <= chunkRadius; x++)
        {
            for(int y = -chunkRadius; y <= chunkRadius; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(currChunk.x + x, currChunk.y + y);
                chunksToLoad.Add(chunkCoord);
            }
        }

        foreach(var chunk in chunksToLoad)
        {
            if(!activeChunkCoords.Contains(chunk))
            {
                SpawnChunk(chunk);
                activeChunkCoords.Add(chunk);
            }
        }

        // Despawn chunks that are out of range
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();
        foreach(var chunk in activeChunkCoords)
        {
            if(!chunksToLoad.Contains(chunk))
            {
                DespawnChunk(chunk);
                chunksToUnload.Add(chunk);
            }
        }

        foreach(var chunk in chunksToUnload)
        {
            activeChunkCoords.Remove(chunk);
        }
    }

    private void SpawnChunk(Vector2Int chunkCoord)
    {
        List<GameObject> chunkItems = new List<GameObject>();
        Vector2 chunkOrigin = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);

        for(int i = 0; i < itemsPerChunk; i++)
        {
            // Random position within chunk
            Vector2 randomOff = new Vector2(
              Random.Range(-chunkSize / 2, chunkSize / 2),
              Random.Range(-chunkSize / 2, chunkSize / 2)  
            );
            Vector2 spawnPos = chunkOrigin + randomOff;

            // Random properties
            float scale = Random.Range(scaleRange.x, scaleRange.y);
            float weight = Random.Range(weightRange.x, weightRange.y);
            int value = (int) Random.Range(valueRange.x, valueRange.y + 1);

            // Spawn
            GameObject newItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            Item itemComp = newItem.GetComponent<Item>();

            if(itemComp != null)
            {
                itemComp.Initialize(scale, weight, value);
            }else
            {
                Debug.LogError("Item prefab missing Item component");
            }

            chunkItems.Add(newItem);
        }

        spawnedChunks[chunkCoord] = chunkItems;
        Random.InitState(System.Environment.TickCount);
    }

    private void DespawnChunk(Vector2Int chunkCoord)
    {
        if(spawnedChunks.ContainsKey(chunkCoord))
        {
            foreach(var item in spawnedChunks[chunkCoord])
            {
                if(item != null)
                    Destroy(item);
            }
            spawnedChunks.Remove(chunkCoord);
        }
    }

    private Vector2Int GetChunkCoord(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    private int GetChunkSeed(Vector2Int chunkCoord)
    {
        return chunkCoord.x * 73856093 ^ chunkCoord.y * 19349663;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Spawn radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, spawnRadius);

        // Despawn radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, despawnRadius);

        // Draw chunk grid
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Vector2Int currentChunk = GetChunkCoord(player.position);
        int chunkRadius = Mathf.CeilToInt(spawnRadius / chunkSize);

        for (int x = -chunkRadius; x <= chunkRadius; x++)
        {
            for (int y = -chunkRadius; y <= chunkRadius; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentChunk.x + x, currentChunk.y + y);
                Vector2 chunkCenter = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);
                
                Gizmos.DrawWireCube(new Vector3(chunkCenter.x, chunkCenter.y, 0), new Vector3(chunkSize, chunkSize, 0));
            }
        }
    }
}
