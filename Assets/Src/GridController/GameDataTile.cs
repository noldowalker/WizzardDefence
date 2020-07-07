using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameDataTile
{
    public Vector3Int LocalPlace { get; set; }

    public Vector3 CenterWorldPlace { get; set; }

    public TileBase TileBase { get; set; }

    public string Name { get; set; }

    public bool Occupied { get; set; }
    public bool Visited { get; set; }

    public GameDataTile CameFrom { get; set; }

    public int count;

    public GameDataTile()
    {
        Occupied = false;
        Visited = false;
        count = 0;
    }
}
