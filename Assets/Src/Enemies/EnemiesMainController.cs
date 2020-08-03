using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemiesMainController : MonoBehaviour
{
    protected BaseSceneFinder scene;
    protected InterfaceTilemapController tilemap;
    protected LogController log;
 

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

        foreach (DummyController enemy in GetComponentsInChildren<DummyController>())
        {
            enemy.onEnemyDestroy += this.HandleEnemyDeath;
        }
    }

    // Действия которые запускаются каждый кадр
    void Update()
    {
        List<GameDataTile> occupiedTiles = new List<GameDataTile>();
        foreach (DummyController enemy in GetComponentsInChildren<DummyController>())
        {
            occupiedTiles.Add(this.EnemyUpdate(enemy));            
        }
        tilemap.MarkTilesAsOccupied(occupiedTiles);
    }

    private GameDataTile EnemyUpdate(DummyController enemy) {
        Vector3 enemyPosition = enemy.gameObject.transform.position;
        Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(enemyPosition);
        GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);

        if (enemy.isIdle()) { this.IdleUpdate(enemy, enemyTileData); }

        return enemyTileData;
    }

    private void IdleUpdate(DummyController enemy, GameDataTile enemyTileData) {
        List<GameDataTile> path = tilemap.FindPathToEntrance(enemyTileData);
        GameDataTile nextTile = (path != null && path[path.Count - 1] != null) ? path[path.Count - 1] : null;  

        if (nextTile != null && nextTile.IsFree())
        {
            enemy.Move(nextTile.CenterWorldPlace);
        }
    }

    private void HandleEnemyDeath(Vector3 deathPoint)
    {
        Vector3Int enemyTilePosition = tilemap.GetTilePositionByWorldCoords(deathPoint);
        GameDataTile enemyTileData = tilemap.GetTileDataByPosition(enemyTilePosition);

        tilemap.MarkTileFree(enemyTileData);
    }


}
