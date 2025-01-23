using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    BossRoom,
    TreasureRoom,
    ShopRoom,
    SecretRoom,
}

public class Room : MonoBehaviour
{
    public Vector2Int RoomIndex { get; set; }
    public RoomType Type { get; set; } = RoomType.Normal;
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    [SerializeField] GameObject topMiddleWall;
    [SerializeField] GameObject bottomMiddleWall;
    [SerializeField] GameObject leftMiddleWall;
    [SerializeField] GameObject rightMiddleWall;

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            topDoor.SetActive(true);
            topDoor.GetComponent<Collider2D>().enabled = false;
            topMiddleWall.GetComponent<Collider2D>().enabled = false;
        }
        if (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
            bottomDoor.GetComponent<Collider2D>().enabled = false;
            bottomMiddleWall.GetComponent<Collider2D>().enabled = false;
        }
        if (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
            leftDoor.GetComponent<Collider2D>().enabled = false;
            leftMiddleWall.GetComponent<Collider2D>().enabled = false;
        }
        if (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
            rightDoor.GetComponent<Collider2D>().enabled = false;
            rightMiddleWall.GetComponent<Collider2D>().enabled = false;
        }
    }
}
