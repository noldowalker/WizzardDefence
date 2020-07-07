using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemiesMainController : MonoBehaviour
{
    protected BaseSceneFinder scene;
    protected InterfaceTilemapController tilemap;
    protected LogController log;

    // public Dictionary<string, Tile> tilesForPathDebug;
    public List<Tile> tilesForPathDebug;

    void Awake()
    {
        scene = GetComponentInParent<BaseSceneFinder>();
        if (scene == null)
        {
            LogController.ShowError(LogController.Errors.BaseSceneFinderIsNull);
            return;
        }

        tilemap = scene.GetComponentInChildren<InterfaceTilemapController>();
        if (tilemap == null)
        {
            LogController.ShowError(LogController.Errors.InterfaceTilemapControllerIsNull);
            return;
        }


        //на случай если в контейнер уже добавлены какие-то враги.
        foreach (DummyController enemy in GetComponentsInChildren<DummyController>())
        {
            Vector3 enemyPosition = enemy.gameObject.transform.position;
            Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(enemyPosition);
            GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);
            enemy.AlignToCoords(enemyTileData.CenterWorldPlace);
            tilemap.MarkTileOccupied(enemyTileData);
            tilemap.ChangeTileSprite(enemyTileData.LocalPlace, tilesForPathDebug[0]);
            List<GameDataTile> path = tilemap.FindPathToEntrance(enemyTileData);
            Debug.Log("This tile" + enemyTileData.LocalPlace);
            Debug.Log("Next tile" + path[path.Count - 1].LocalPlace);
            if (path != null)
            {
                enemy.Move(path[path.Count - 1].CenterWorldPlace);
            }

        }
    }

    void Update()
    {
        foreach (DummyController enemy in GetComponentsInChildren<DummyController>())
        {
            if (enemy.isIdle()) {
                Vector3 enemyPosition = enemy.gameObject.transform.position;
                Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(enemyPosition);
                GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);
                List<GameDataTile> path = tilemap.FindPathToEntrance(enemyTileData);
                if (path != null) {
                    Debug.Log("This tile" + enemyTileData.LocalPlace);
                    Debug.Log("Next tile" + path[path.Count - 1].LocalPlace);
                    enemy.Move(path[path.Count - 1].CenterWorldPlace);
                }
            }
        }
    }
}
