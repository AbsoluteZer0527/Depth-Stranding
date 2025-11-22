using UnityEngine;

public class HazardousItem : MonoBehaviour
{
    public float damage = 1.0f;
    public Vector2 damageRange = new(0.5f, 3f);

    public void Initialize(float newDamage)
    {
        damage = newDamage;
    }

    public void HitPlayer()
    {
        if (Player.instance != null)
        {
            Player.instance.hp -= damage;
        }

        Destroy(gameObject);
    }
}
