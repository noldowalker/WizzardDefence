using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallsTilemapController : MonoBehaviour
{
    // Список свободных от меток тайлов
    private Vector3Int doorTilePosition;
    private Tilemap tilemap;

    public Tile doorKikedTile;

    // Start is called before the first frame update
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void MakeTransparent() {
        tilemap.color = new Color(1f, 1f, 1f, 0.4f);
    }

    public void MakeSolid()
    {
        tilemap.color = new Color(1f, 1f, 1f, 1f);
    }

    public void changeTransparensyOn(bool isTransparent) {
        if (isTransparent)
            MakeTransparent();
        else
            MakeSolid();
    }
}
