using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTilemapController : MonoBehaviour
{
    // Список свободных от меток тайлов
    private List<Vector3Int> doorTilesPositions;
    private Tilemap tilemap;
    private List<Vector3Int> tilesData;
    public Tile doorKikedTile;

    // Start is called before the first frame update
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tilesData = new List<Vector3Int>();
        doorTilesPositions = new List<Vector3Int>();
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
                        doorTilesPositions.Add(position);
                        break;
                }
            }
            tilesData.Add(position);
        }
    }

    public void DestroyDoor()
    {
        foreach (Vector3Int doorTilePosition in doorTilesPositions)
            tilemap.SetTile(doorTilePosition, doorKikedTile);
    }

    // Вспомогательная функция для дебага. Рисует счет алгоритма по клеткам.
    void OnDrawGizmos()
    {
        //if (tilesData != null)
        //    foreach (Vector3Int record in tilesData)
        //    {
        //        UnityEditor.Handles.Label(tilemap.GetCellCenterWorld(record), "" + record.x + "," + record.y);
        //    }
    }
}
