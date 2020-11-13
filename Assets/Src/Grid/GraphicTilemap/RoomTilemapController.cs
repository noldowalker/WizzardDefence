using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTilemapController : MonoBehaviour
{
    // Список свободных от меток тайлов
    private Vector3Int doorTilePosition;
    private Tilemap tilemap;

    public Tile doorKikedTile;

    // Start is called before the first frame update
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        CreateTileInfo();
    }

    private void CreateTileInfo()
    {

        List<Vector3> availablePlaces = new List<Vector3>();
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase currentTile = tilemap.GetTile(position);
            if (currentTile != null)
            {
                switch (currentTile.name)
                {
                    case "inside_door_closed":
                        doorTilePosition = position;
                        break;
                }
            }
        }
    }

    public void DestroyDoor()
    {
        tilemap.SetTile(doorTilePosition, doorKikedTile);
    }
}
