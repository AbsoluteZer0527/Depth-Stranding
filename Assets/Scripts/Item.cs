using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Properties")]
    public float scale = 1f;
    public float weight = 1f;
    public int value = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one * scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(float newScale, float newWeight, int newValue)
    {
        scale = newScale;
        weight = newWeight;
        value = newValue;

        transform.localScale = Vector3.one * scale;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.mass = weight;
    }

    public void Collect()
    {
        Destroy(gameObject);
    }
}
