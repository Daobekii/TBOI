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

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            topDoor.SetActive(true);
        }
        if (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
        }
        if (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
        }
        if (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
        }
    }
}
