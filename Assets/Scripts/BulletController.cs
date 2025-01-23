using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody2D playerRb;
    private float shotSpeed = 7f;
    private float lastFire;
    private float fireRate = 2f; // as in tears per second, like in Binding of Isaac
    public GameObject bulletPrefab;
    public Animator animator;
    Vector2 shootDirection;

    void Update()
    {
        Shooting();
    }

    void Shoot(float x, float y)
    {
        Vector2 direction = new Vector2(x, y).normalized; // Richtung normieren
        GameObject bullet = Instantiate(bulletPrefab, playerRb.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = direction * shotSpeed;
    }

    void Shooting()
    {
        shootDirection.x = Input.GetAxisRaw("ShootHorizontal");
        shootDirection.y = Input.GetAxisRaw("ShootVertical");

        if (Mathf.Abs(shootDirection.x) > 0 && Mathf.Abs(shootDirection.y) > 0)
        {
            // Wenn beide Achsen aktiv sind, priorisiere eine (z. B. horizontal):
            shootDirection.y = 0; // Vertikales Schießen ignorieren
        }

        if ((shootDirection.x != 0 || shootDirection.y != 0) && Time.time > lastFire + (1 / fireRate))
        {
            Shoot(shootDirection.x, shootDirection.y);
            animator.SetFloat("Horizontal", shootDirection.x);
            animator.SetFloat("Vertical", shootDirection.y);
            lastFire = Time.time;
            shootDirection = Vector2.zero;
        }
    }
}