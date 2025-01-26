using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    public Camera mainCamera;
    Vector2 movement;
    Vector2 shootDirection;
    private bool isShooting = false;
    private RoomManager roomManager;

    void Start()
    {
        roomManager = FindFirstObjectByType<RoomManager>();
    }

    void Update() // This is for input
    {
        RegenerateRoomsOnKeyPress();
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

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Trigger")) 
        {
            Vector2 characterTeleportOffset = Vector2.zero;
            Vector3 cameraTeleportOffset = Vector3.zero;

            switch(other.gameObject.name)
            {
                case "TopDoorTrigger":
                    characterTeleportOffset = new Vector2(0f, 3.5f);
                    cameraTeleportOffset = new(0f, 12f);
                    break;

                case "BottomDoorTrigger":
                    characterTeleportOffset = new Vector2(0f, -3.5f);
                    cameraTeleportOffset = new(0f, -12f);
                    break;

                case "LeftDoorTrigger":
                    characterTeleportOffset = new Vector2(-4.5f, 0f);
                    cameraTeleportOffset = new(-20f, 0f);
                    break;

                case "RightDoorTrigger":
                    characterTeleportOffset = new Vector2(4.5f, 0f);
                    cameraTeleportOffset = new(20f, 0f);
                    break;
            }
            rb.position += characterTeleportOffset;
            mainCamera.transform.position += cameraTeleportOffset;
        }
    }
    private void RegenerateRoomsOnKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
           roomManager.RegenerateRooms();
           rb.position = Vector2.zero;
           mainCamera.transform.position = new Vector3(0, 0, -10);
        }
    }
}
