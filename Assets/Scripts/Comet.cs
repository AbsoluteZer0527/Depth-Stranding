using UnityEngine;

public class Comet : MonoBehaviour
{
    public float speed = 5.0f;
    public Vector2 speedRange = new(3f, 10f);

    private Rigidbody2D rb;
    private HazardousItem hazardousItem;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hazardousItem = GetComponent<HazardousItem>();
    }

    public void Initialize(float newDamage, float newSpeed, Vector2 direction)
    {
        speed = newSpeed;

        if (hazardousItem != null)
        {
            hazardousItem.Initialize(newDamage);
        }

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
