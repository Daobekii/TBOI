using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject[] roomPrefabs;
    [SerializeField] GameObject startRoomPrefab;
    [SerializeField] GameObject bossRoomPrefab;
    [SerializeField] GameObject treasureRoomPrefab;
    [SerializeField] GameObject verticalRoomPrefab;
    [SerializeField] GameObject shopRoomPrefab;
    [SerializeField] GameObject secretRoomPrefab;
    [SerializeField] private int maxRooms = 12;
    [SerializeField] private int minRooms = 8;

    int roomWidth = 20;
    int roomHeight = 12;
    int gridSizeX = 10;
    int gridSizeY = 10;
    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int[,] roomGrid;
    private int roomCount;
    private bool generationComplete = false;

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();
        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2); //5-5
        Debug.Log($"Starting room generation from {initialRoomIndex}");
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            Debug.Log($"Dequeued room at {roomIndex}. Remaining Queue Count: {roomQueue.Count}");
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            // No neighbor to the left
            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));

            // No neighbor to the right
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));

            // No neighbor below
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));

            // No neighbor above
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        else if (roomCount < minRooms)
        {
            Debug.Log("Minimum room count not reached. Regenerating rooms.");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            generationComplete = true;

            AssignVerticalRooms();

            List<GameObject> potentialEndRooms = new List<GameObject>();

            foreach (var room in roomObjects)
            {
                Room roomScript = room.GetComponent<Room>();
                if (CountAdjacentRooms(roomScript.RoomIndex) == 1)
                {
                    potentialEndRooms.Add(room);
                }
            }

            potentialEndRooms.Remove(roomObjects.First());

            Debug.Log("Anzahl von potentiellen Endräumen: " + potentialEndRooms.Count);

            if (potentialEndRooms.Count < 4)
            {
                Debug.LogWarning("Less than 4 rooms with only one door. Regenerating rooms.");
                RegenerateRooms();
                return; // Verhindert, dass nachfolgender Code ausgeführt wird. Ist nötig, weil es sonst mehrere "Sonderräume" gibt und diese nicht gelöscht werden.
            }

            //Boss Room
            GameObject lastRoom = roomObjects.Last();
            GameObject bossRoomPrefabInstance = GetPrefabForRoomType(RoomType.BossRoom);
            ReplaceRoomPrefab(lastRoom, bossRoomPrefabInstance);
            Debug.Log("Boss Room placed at the end: " + lastRoom.GetComponent<Room>().RoomIndex);
            potentialEndRooms.Remove(lastRoom);

            //Treasure Room
            GameObject treasureRoom = potentialEndRooms[Random.Range(0, potentialEndRooms.Count)];
            GameObject treasureRoomPrefabInstance = GetPrefabForRoomType(RoomType.TreasureRoom);
            ReplaceRoomPrefab(treasureRoom, treasureRoomPrefabInstance);
            Debug.Log("Treasure Room placed at a branch end: " + treasureRoom.GetComponent<Room>().RoomIndex);
            potentialEndRooms.Remove(treasureRoom);

            //Shop Room
            GameObject shopRoom = potentialEndRooms[Random.Range(0, potentialEndRooms.Count)];
            GameObject shopRoomPrefabInstance = GetPrefabForRoomType(RoomType.ShopRoom);
            ReplaceRoomPrefab(shopRoom, shopRoomPrefabInstance);
            Debug.Log("Shop Room placed at a branch end: " + shopRoom.GetComponent<Room>().RoomIndex);
            potentialEndRooms.Remove(shopRoom);

            PlaceSecretRoom();
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;

        var initialRoom = Instantiate(startRoomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (x >= gridSizeX || y >= gridSizeY || x < 0 || y < 0)
        {
            Debug.LogWarning($"Room index {roomIndex} is out of bounds.");
            return false;
        }

        if (roomCount >= maxRooms || roomGrid[x, y] != 0)
        {
            Debug.LogWarning($"Room at {roomIndex} already exists or maxRooms reached.");
            return false;
        }

        // 50% Chance, den Raum zu überspringen. Brot und Butter der Random-Generation. Sonst immer wieder selbe Anordnung von Räumen.
        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
        {
            Debug.Log($"Skipping room generation at {roomIndex} due to random value.");
            return false;
        }

        int adjacentCount = CountAdjacentRooms(roomIndex);

        if (adjacentCount > 1)
        {
            // Räume sollen nur einen Nachbarn haben (zu diesem Zeitpunkt)
            Debug.Log($"Room at {roomIndex} skipped due to too many adjacent rooms.");
            return false;
        }

        // Alle Prüfungen bestanden, Raum zur Warteschlange hinzufügen
        roomQueue.Enqueue(roomIndex);
        Debug.Log($"Adding room to queue at {roomIndex}. Current Queue Count: {roomQueue.Count}");
        roomGrid[x, y] = 1;
        roomCount++;

        GameObject roomPrefabToUse = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        var newRoom = Instantiate(roomPrefabToUse, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        Debug.Log($"Generated room at {roomIndex}. Total rooms: {roomCount}");

        OpenDoors(newRoom, x, y);

        return true;
    }

    private void AssignVerticalRooms()
    {
        foreach (var room in roomObjects.ToList()) // ToList() um Modifikationen während der Iteration zu vermeiden, da sonst Fehlermeldung
        {
            Room roomScript = room.GetComponent<Room>();
            Vector2Int index = roomScript.RoomIndex;
            int x = index.x;
            int y = index.y;

            bool hasUp = y < gridSizeY - 1 && roomGrid[x, y + 1] != 0;
            bool hasDown = y > 0 && roomGrid[x, y - 1] != 0;

            bool noLeft = x <= 0 || roomGrid[x - 1, y] == 0;
            bool noRight = x >= gridSizeX - 1 || roomGrid[x + 1, y] == 0;

            if (hasUp && hasDown && noLeft && noRight)
            {
                if (Random.value < 0.5f) ReplaceRoomPrefab(room, verticalRoomPrefab);
            }
        }
    }

    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        // Nachbarn
        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room upRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room downRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // Bestimme, welche Türen geöffnet werden sollen
        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            // Es gibt einen Raum links
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            // Es gibt einen Raum rechts
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }
        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            // Es gibt einen Raum unten
            newRoomScript.OpenDoor(Vector2Int.down);
            downRoomScript.OpenDoor(Vector2Int.up);
        }
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            // Es gibt einen Raum oben
            newRoomScript.OpenDoor(Vector2Int.up);
            upRoomScript.OpenDoor(Vector2Int.down);
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(room => room.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
        {
            return roomObject.GetComponent<Room>();
        }
        return null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int count = 0;
        int x = roomIndex.x;
        int y = roomIndex.y;
        if (x > 0 && roomGrid[x - 1, y] != 0) count++; // left neighbour
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; // right neighbour
        if (y > 0 && roomGrid[x, y - 1] != 0) count++; // down neighbour
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; // up neighbour

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2));
    }

    private GameObject GetPrefabForRoomType(RoomType type)
    {
        switch (type)
        {
            case RoomType.BossRoom:
                return bossRoomPrefab;
            case RoomType.TreasureRoom:
                return treasureRoomPrefab;
            case RoomType.ShopRoom:
                return shopRoomPrefab;
            case RoomType.SecretRoom:
                return secretRoomPrefab;
            case RoomType.Normal:
            default:
                return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        }
    }

    private void ReplaceRoomPrefab(GameObject room, GameObject newPrefab)
    {
        Vector3 position = room.transform.position;
        Quaternion rotation = room.transform.rotation;

        Room roomScript = room.GetComponent<Room>();
        Vector2Int roomIndex = roomScript.RoomIndex;

        Destroy(room);

        GameObject newRoom = Instantiate(newPrefab, position, rotation);

        Room newRoomScript = newRoom.GetComponent<Room>();
        newRoomScript.RoomIndex = roomIndex;

        int roomIndexInList = roomObjects.IndexOf(room);
        if (roomIndexInList != -1)
        {
            roomObjects[roomIndexInList] = newRoom;
        }
        else
        {
            Debug.LogWarning($"Room {room.name} not found in roomObjects list.");
        }

        OpenDoors(newRoom, roomIndex.x, roomIndex.y);

        Debug.Log($"Replaced room at {roomIndex} with new prefab.");
    }

    private void PlaceSecretRoom()
    {
        List<Vector2Int> potentialSecretRoomPositions = new List<Vector2Int>();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Stelle sicher, dass das Grid-Feld leer ist
                if (roomGrid[x, y] != 0) continue;

                Vector2Int currentIndex = new Vector2Int(x, y);

                int adjacentRooms = CountAdjacentRooms(currentIndex);
                bool hasNoBossRoomNeighbor = !HasNeighbor(currentIndex, room => room.name.StartsWith(bossRoomPrefab.name));
                bool hasNoVerticalRoomNeighbor = !HasNeighbor(currentIndex, room => room.name.StartsWith(verticalRoomPrefab.name));

                if (adjacentRooms >= 2 && hasNoBossRoomNeighbor && hasNoVerticalRoomNeighbor)
                {
                    potentialSecretRoomPositions.Add(currentIndex);
                }
            }
        }

        if (potentialSecretRoomPositions.Count > 0)
        {
            Vector2Int secretRoomPosition = potentialSecretRoomPositions[Random.Range(0, potentialSecretRoomPositions.Count)];

            roomGrid[secretRoomPosition.x, secretRoomPosition.y] = 1;
            GameObject secretRoomPrefabInstance = GetPrefabForRoomType(RoomType.SecretRoom);
            var secretRoom = Instantiate(secretRoomPrefabInstance, GetPositionFromGridIndex(secretRoomPosition), Quaternion.identity);
            secretRoom.name = "SecretRoom";
            secretRoom.GetComponent<Room>().RoomIndex = secretRoomPosition;
            roomObjects.Add(secretRoom);

            //OpenDoors(secretRoom, secretRoomPosition.x, secretRoomPosition.y);

            Debug.Log($"Secret Room placed at: {secretRoomPosition}");
        }
        else
        {
            Debug.LogWarning("No suitable location for a Secret Room.");
        }
    }

    private bool HasNeighbor(Vector2Int roomIndex, System.Predicate<Room> condition)
    {
        // Alle möglichen Nachbarn
        Vector2Int[] neighbors = {
        new Vector2Int(roomIndex.x - 1, roomIndex.y), // links
        new Vector2Int(roomIndex.x + 1, roomIndex.y), // rechts
        new Vector2Int(roomIndex.x, roomIndex.y - 1), // unten
        new Vector2Int(roomIndex.x, roomIndex.y + 1)  // oben
        };

        foreach (var neighbor in neighbors)
        {
            if (neighbor.x >= 0 && neighbor.x < gridSizeX && neighbor.y >= 0 && neighbor.y < gridSizeY)
            {
                if (roomGrid[neighbor.x, neighbor.y] != 0) // Es gibt einen Raum
                {
                    Room neighborRoom = GetRoomScriptAt(neighbor);
                    if (neighborRoom != null && condition(neighborRoom)) return true;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.5f);
        Gizmos.color = gizmoColor;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }
}