using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 3.5f; // Set this value as needed
    public float range = 6.5f; // Set this value as needed
    private Vector2 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);
        if (distanceTraveled > range)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if(other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Stone") || other.gameObject.CompareTag("Tear")){
            Destroy(gameObject);
        }
    }
}