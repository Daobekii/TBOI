using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    Vector2 movement;
    Vector2 shootDirection;
    private bool isShooting = false;

    void Update() // This is for input
    {
        // Bewegungseingabe
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        shootDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            shootDirection = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            shootDirection = Vector2.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            shootDirection = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            shootDirection = Vector2.right;
        }

        isShooting = shootDirection != Vector2.zero;

        UpdateAnimation();
    }

    void FixedUpdate() // This is for movement
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimation()
    {
        if (isShooting)
        {
            animator.SetFloat("Horizontal", shootDirection.x);
            animator.SetFloat("Vertical", shootDirection.y);
        }
        else if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
    }
}
